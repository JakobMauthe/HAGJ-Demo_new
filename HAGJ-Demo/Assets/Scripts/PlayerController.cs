using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PhysicsObject {

    public enum State {
        Alive,
        Dead,
    }

    private State state;

    public Animator animator;

    //Health and Stamina

    public HealthBar healthBar;
    public StaminaBar staminaBar;

    public GameObject dropShadow;        

    [SerializeField, Range(0,1000)]
    float maxHealth = 100f;

    [SerializeField, Range(0, 1000)]
    int maxStamina = 100;

    [Header("For Audio")]
    [SerializeField, Range(0, 100)] int lowHealthThreshold = 25;
    [SerializeField, Range(0, 100)] int lowStaminaThreshold = 33;

    private float currentHealth;
    private int currentStamina;

    private bool lowHealth, lowStamina;

    private WaitForSeconds regenTick = new WaitForSeconds(0.1f);
    private Coroutine regen;

    //Movement
    public float maxMovementSpeed = 5f;
    public float jumpTakeOffSpeed = 5f;


    //Attacking & blocking
    public Transform attackPoint;
    public LayerMask enemyLayers;
    private bool isBlocking;

    [SerializeField, Range(0.1f, 5f)]
    float blockingTime = 1f;
    
    //Little Attack
    [SerializeField, Range(0, 100)]
    int attackLittleDamage = 15;

    [SerializeField, Range(0, 100)]
    int attackLittleStaminaCost = 10;

    public float attackLittleRange = 0.5f;
    //Heavy Attack
    [SerializeField, Range(0, 100)]
    int attackHeavyDamage = 40;

    [SerializeField, Range(0, 100)]
    int attackHeavyStaminaCost = 25;

    public float attackHeavyRange = 1f;

    public float Health => currentHealth;
    public int Stamina => currentStamina;

    public bool IsBlocking => isBlocking;

    public static PlayerController Instance { get; private set; }

    public override void Awake() {
        base.Awake();
        Instance = this;
        currentStamina = maxStamina;
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        staminaBar.SetMaxStamina(maxStamina);
        isBlocking = false;
        lowStamina = lowHealth = false;
        state = State.Alive;

        EventManager.Instance.OnPlayerLittleAttack += LittleAttack_OnLittleAtackInitiated;
        EventManager.Instance.OnPlayerHeavyAttack += HeavyAttack_OnHeavyAtackInitiated;
        EventManager.Instance.OnBlockInitiated += Blocked_OnBlockInitiated;
    }


    private void OnDestroy() {
        EventManager.Instance.OnPlayerLittleAttack -= LittleAttack_OnLittleAtackInitiated;
        EventManager.Instance.OnPlayerHeavyAttack -= HeavyAttack_OnHeavyAtackInitiated;
        EventManager.Instance.OnBlockInitiated -= Blocked_OnBlockInitiated;
    }


    public override void Update() {
        base.Update();
        bool attackAllowed = true;
        animator.SetFloat("MovementSpeed", Mathf.Abs(velocity.x));
        if (state == State.Dead) {
            return;
        }
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Atack1") ||
           animator.GetCurrentAnimatorStateInfo(0).IsName("Player_HeavyAttack") ||
           animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Atack2")
           ) {
            attackAllowed = false;
        }
       
        if (Input.GetMouseButtonDown(0) && !PauseMenu.gameIsPaused && attackAllowed) {
            LittleAttack();
        }
        if (Input.GetMouseButtonDown(1) && !PauseMenu.gameIsPaused && attackAllowed) {
            HeavyAttack();
        }
        if (Input.GetKeyDown(KeyCode.Q) && !isBlocking) {
            Block();
        }
        if (Input.GetKeyDown(KeyCode.K)) {
            GameManager.PlayerDied();
        }
    }

    void LittleAttack() {        
        if (currentStamina < attackLittleStaminaCost) {
            //not enough stamina; maybe play sound or higlight staminabar
            return;
        }
        int randomAttack = UnityEngine.Random.Range(0, 2);
        if (randomAttack == 0) {
            animator.SetTrigger("Attack1");
        }
        else {
            animator.SetTrigger("Attack2");
        }
        UseStamina(attackLittleStaminaCost);        
    }

    void HeavyAttack() {
        if (currentStamina < attackHeavyStaminaCost) {
            //not enough stamina; maybe play sound or higlight staminabar
            return;
        }
        animator.SetTrigger("HeavyAttack");
        UseStamina(attackHeavyStaminaCost);        
    }

    private void LittleAttack_OnLittleAtackInitiated(object sender, EventArgs e) {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackLittleRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies) {
            enemy.GetComponent<BasicEnemyController>().TakeDamage((float)attackLittleDamage);
        }
    }

    private void HeavyAttack_OnHeavyAtackInitiated(object sender, EventArgs e) {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackHeavyRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies) {
            enemy.GetComponent<BasicEnemyController>().TakeDamage((float)attackHeavyDamage);
        }
    }

    private void Blocked_OnBlockInitiated(Vector2 somePositionIdontNeed) {
        animator.SetTrigger("Blocked");
    }

    void UseStamina(int amount) {
        if (amount <= currentStamina) {
            currentStamina -= amount;
            staminaBar.SetStamina(currentStamina);
            if (currentStamina < lowStaminaThreshold) {
                EventManager.Instance.NotifyOfOnStaminaLow(this);
                lowStamina = true;
            }            

            if (regen != null) {
                StopCoroutine(regen);
            }
            regen = StartCoroutine(PassiveRegenStamina());
        }
        else {
            //Not enough Stamina
        }
    }

    void OnDrawGizmosSelected() {
        if (attackPoint == null) {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackLittleRange);
        Gizmos.DrawWireSphere(attackPoint.position, attackHeavyRange);
    }

    protected override void SetGroundedForAnimation(bool groundedForAnimation) {
        animator.SetBool("grounded", groundedForAnimation);
    }

    protected override void ComputeVelocity() {
        if (state == State.Dead) {
            return;
        }
        
        Vector2 move = Vector2.zero;
        move.x = Input.GetAxis("Horizontal");

        if (grounded && !dropShadow.activeInHierarchy) {
            dropShadow.SetActive(true);
        }

        if (Input.GetButtonDown("Jump") && grounded) {
            if (currentStamina > 10) {
                velocity.y = jumpTakeOffSpeed;
                animator.SetTrigger("Jump");
                EventManager.Instance.NotifyOfOnJumpInitiated(this);
                UseStamina(attackLittleStaminaCost);

                dropShadow.SetActive(false);          
            }   
        }
        else if (Input.GetButtonUp("Jump")) {
            velocity.y = velocity.y * .5f;
        }
        targetVelocity = move * maxMovementSpeed;        
    }    
    
    private IEnumerator PassiveRegenStamina() {
        yield return new WaitForSeconds(1.5f);
        while (currentStamina < maxStamina) {
            currentStamina += maxStamina / 100;
            staminaBar.SetStamina(currentStamina);
            if (currentStamina > lowStaminaThreshold && lowStamina) {
                EventManager.Instance.NotifyOfOnStaminaNotLow(this);
                lowStamina = false;
            }
            yield return regenTick;
        }
        regen = null;
    }
    public void TakeDamage(float damage) {
        if (damage >= currentHealth) {
            if(state != State.Dead) {
                state = State.Dead;
                EventManager.Instance.NotifyOfOnPlayerDeath(this);
                EventManager.Instance.NotifyOfOnHealthNotLow(this);
                GameManager.PlayerDied();               
            }            
        }
        else {
            currentHealth -= damage;
            if (currentHealth < lowHealthThreshold) {
                EventManager.Instance.NotifyOfOnHealthLow(this);
                lowHealth = true;
            }
            else if (currentHealth > lowHealthThreshold && lowHealth) {
                EventManager.Instance.NotifyOfOnHealthNotLow(this);
                lowHealth = false;
            }
            EventManager.Instance.NotifyOfOnPlayerGetsHit(this);
            animator.SetTrigger("Hurt");
            healthBar.SetHealth(currentHealth);
        }
    }

    public void Block() {
        isBlocking = true;
        animator.SetTrigger("Block");
        StartCoroutine(BlockCountdown());
    }

    IEnumerator BlockCountdown() {
        yield return new WaitForSeconds(blockingTime);
        isBlocking = false;
    }
}