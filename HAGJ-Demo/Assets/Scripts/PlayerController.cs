using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PhysicsObject {

    public Animator animator;

    //Health and Stamina

    public HealthBar healthBar;
    public StaminaBar staminaBar;

    [SerializeField, Range(0,1000)]
    float maxHealth = 100f;

    [SerializeField, Range(0, 1000)]
    int maxStamina = 100;   
    
    public float currentHealth;
    public int currentStamina;

    private WaitForSeconds regenTick = new WaitForSeconds(0.1f);
    private Coroutine regen;

    //Movement
    public float maxMovementSpeed = 5f;
    public float jumpTakeOffSpeed = 5f;


    //Attacking 
    public Transform attackPoint;
    public LayerMask enemyLayers;
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

    public static PlayerController Instance { get; private set; }

    public override void Awake() {
        base.Awake();
        Instance = this;
        currentStamina = maxStamina;
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        staminaBar.SetMaxStamina(maxStamina);
    }

    public override void Update() {
        base.Update();
        animator.SetFloat("MovementSpeed", Mathf.Abs(velocity.x));
        
        if (Input.GetMouseButtonDown(0)&& !PauseMenu.gameIsPaused) {
            LittleAttack();
        }
        if (Input.GetMouseButtonDown(1) && !PauseMenu.gameIsPaused) {
            HeavyAttack();
        }
    }

    void LittleAttack() {        
        if (currentStamina < attackLittleStaminaCost) {
            //not enough stamina; maybe play sound or higlight staminabar
            return;
        }
        animator.SetTrigger("HeavyAttack");
        UseStamina(attackLittleStaminaCost);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackLittleRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies) {
            enemy.GetComponent<BasicEnemyController>().TakeDamage((float)attackLittleDamage);
        }
    }

    void HeavyAttack() {
        if (currentStamina < attackHeavyStaminaCost) {
            //not enough stamina; maybe play sound or higlight staminabar
            return;
        }
        animator.SetTrigger("Attack2");
        UseStamina(attackHeavyStaminaCost);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackHeavyRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies) {
            enemy.GetComponent<BasicEnemyController>().TakeDamage((float)attackHeavyDamage);
        }
    }
    void UseStamina(int amount) {
        if (amount <= currentStamina) {
            currentStamina -= amount;
            staminaBar.SetStamina(currentStamina);

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

    protected override void ComputeVelocity() {
        Vector2 move = Vector2.zero;

        move.x = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && grounded) {
            velocity.y = jumpTakeOffSpeed;
        }
        else if (Input.GetButtonUp("Jump")) {
            velocity.y = velocity.y * .5f;
        }
        targetVelocity = move * maxMovementSpeed;
    }    
    
    private IEnumerator PassiveRegenStamina() {
        yield return new WaitForSeconds(2f);
        while (currentStamina < maxStamina) {
            currentStamina += maxStamina / 100;
            staminaBar.SetStamina(currentStamina);
            yield return regenTick;
        }
        regen = null;
    }
    public void TakeDamage(float damage) {
        if (damage >= currentHealth) {
            // Loose State
        }
        else {
            currentHealth -= damage;
            //Play MCHurtAnimation
            healthBar.SetHealth(currentHealth);
        }
    }
}