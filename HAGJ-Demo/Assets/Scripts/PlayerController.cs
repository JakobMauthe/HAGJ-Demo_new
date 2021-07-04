using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PhysicsObject {

    [Header("References")]
    public HealthBar healthBar;
    public StaminaBar staminaBar;
    public GameObject dropShadow;
    public Animator animator;
    public Transform attackPoint;
    public LayerMask enemyLayers;

    [Header("Health & Stamina")]
    [SerializeField, Range(0, 1000)] int maxHealth = 100;
    [SerializeField, Range(0, 1000)] int maxStamina = 100;

    private int currentHealth, currentStamina;
    private bool lowHealth, lowStamina;
    private WaitForSeconds regenTick = new WaitForSeconds(0.1f);
    private Coroutine regen;

    [Header("For Audio")]
    [SerializeField, Range(0, 100)] int lowHealthThreshold = 25;
    [SerializeField, Range(0, 100)] int lowStaminaThreshold = 33;    

    [Header("Movement")]
    public float maxMovementSpeed = 5f;
    public float jumpTakeOffSpeed = 5f;

    [Header("Little Attack")]
    [SerializeField, Range(0, 100)] int attackLittleDamage = 15;
    [SerializeField, Range(0, 100)] int attackLittleStaminaCost = 10;
    [SerializeField, Range(0.1f, 10f)] float attackLittleRange = 0.5f;

    [Header("Heavy Attack")]
    [SerializeField, Range(0, 100)] int attackHeavyDamage = 40;
    [SerializeField, Range(0, 100)] int attackHeavyStaminaCost = 25;
    [SerializeField, Range(0.1f, 10f)] float attackHeavyRange = 1f;

    [Header("Blocking")]
    [SerializeField, Range(0.1f, 5f)] float blockingTime = 1f;

    private bool isBlocking;

    private State state;

    public enum State {
        Alive,
        Dead,
    }    

    public bool IsBlocking => isBlocking;

    public override void Awake() {
        base.Awake();
        currentStamina = maxStamina;
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        staminaBar.SetMaxStamina(maxStamina);
        isBlocking = lowStamina = lowHealth = false;
        state = State.Alive;
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
       
        if (Input.GetMouseButtonDown(0) && !GameManager.IsGamePaused() && attackAllowed) {
            LittleAttack();
        }
        if (Input.GetMouseButtonDown(1) && !GameManager.IsGamePaused() && attackAllowed) {
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

    public void LittleAttack_calledByAnimationEvents() {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackLittleRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies) {
            enemy.GetComponent<BasicEnemyController>().TakeDamage(attackLittleDamage);
        }
    }

    public void HeavyAttack_calledByAnimationEvents() {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackHeavyRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies) {
            enemy.GetComponent<BasicEnemyController>().TakeDamage(attackHeavyDamage);
        }
    }

    public void BlockSuccesful() {
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
    public void TakeDamage(int damage) {
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