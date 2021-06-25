using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyController : PhysicsObject {

    private enum State {
        Patrol,
        ChaseTarget,
        Attack,
        Blocked,
    }
    private State state;

    public Animator animator;

    public float startHealth = 100;
    public HealthBar healthBar;
    protected float currentHealth;

    //Movement
    private Vector3 startingPosition, toPatrolPosition;
    private bool patrolFirstWayActive = true;
    
    
    private float enemyMaxMovementSpeed = 1.5f;
    public float enemyMaxPatrollingSpeed = 1.5f;
    public float enemyMaxChasingSpeed = 2.5f;

    [SerializeField, Range(1f, 100f)]
    float targetChaseRange = 10f;


    //attacking
    [SerializeField, Range(0f, 100f)]
    float attackDamage = 10f;

    [SerializeField, Range(0.1f, 10f)]
    float startAttackRange = 2f;

    [SerializeField, Range(0.1f, 10f)]
    float attackRange = 2.5f;

    [SerializeField, Range(0.1f, 10f)]
    float attackRate = 0.75f;    

    public Transform attackPoint;
    public LayerMask playerLayer;

    private float nextAttackTime;

    //blocked

    private float notBlockedTime;

    [SerializeField, Range(0.1f, 5f)]
    float blockedDuration = 1f;


    public float Health => currentHealth;
   

    void OnEnable() {
        startingPosition = transform.position;
        toPatrolPosition = GetRandomPatrollingPosition();
        toPatrolPosition = startingPosition - new Vector3(20f, 0f, 0f); // delete me
        currentHealth = startHealth;
        healthBar.SetMaxHealth(startHealth);
        state = State.Patrol;
        EventManager.Instance.OnEnemyAttack += EnemyAttack_OnEnemyAttackInitiated;
    }

    private void FindTarget() {
        if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) < targetChaseRange) {
            state = State.ChaseTarget;
        }
    }

    public Vector3 GetRandomPatrollingPosition() {
        return startingPosition + GetRandomDirection() * UnityEngine.Random.Range(10f, 50f);
    }

    //Get Random Normalized x direction
    public Vector3 GetRandomDirection() {        
        return new Vector3(UnityEngine.Random.Range(-1f, 1f), transform.position.y).normalized;
    }

    protected override void ComputeVelocity() {
        switch (state) {
            case State.Patrol:
                if (patrolFirstWayActive) {
                    MoveToPatrolPosition();
                }
                else if (!patrolFirstWayActive) {
                    MoveToStartingPosition();
                }
                float reachedPositionDistance = 1f;
                if (Vector3.Distance(transform.position, toPatrolPosition) < reachedPositionDistance) {
                    patrolFirstWayActive = false;
                }
                else if (Vector3.Distance(transform.position, startingPosition) < reachedPositionDistance) {
                    patrolFirstWayActive = true;
                }
                FindTarget();
                break;
            case State.ChaseTarget:
                if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) < startAttackRange) {
                    state = State.Attack;
                    break;
                }
                MoveToPlayerPosition();
                if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) > targetChaseRange) {
                    state = State.Patrol;
                    break;
                }
                break;
            case State.Attack:
                if (Time.time > nextAttackTime) {
                    Attack();
                    nextAttackTime = Time.time + attackRate;
                }
                if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) > startAttackRange) {
                    state = State.ChaseTarget;
                    break;
                }
                break;
            case State.Blocked:
                if (Time.time< notBlockedTime) {
                    state = State.ChaseTarget;
                }
                break;
        }
    }

    public void Attack() {
        enemyMaxMovementSpeed = 0f;
        animator.SetFloat("MovementSpeed", enemyMaxMovementSpeed);
        animator.SetTrigger("Attack");
        
    }

    private void EnemyAttack_OnEnemyAttackInitiated(object sender, EventArgs e) {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
        foreach (Collider2D enemy in hitEnemies) {
            PlayerController playerController = enemy.GetComponent < PlayerController>();
            if (playerController.IsBlocking) {
                state = State.Blocked;
                animator.SetTrigger("Blocked");
                notBlockedTime = Time.time + blockedDuration;
            }
            else {
                playerController.TakeDamage((float)attackDamage);
            }
        }
    }

    public void MoveToPatrolPosition() {
        enemyMaxMovementSpeed = enemyMaxPatrollingSpeed;
        animator.SetFloat("MovementSpeed", enemyMaxMovementSpeed);
        Vector2 move = Vector2.zero;

        if (toPatrolPosition.x < startingPosition.x) {
            move.x = -1f;
        }
        else {
            move.x = 1f;
        }
        targetVelocity = move * enemyMaxMovementSpeed;
    }

    public void MoveToStartingPosition() {
        enemyMaxMovementSpeed = enemyMaxPatrollingSpeed;
        animator.SetFloat("MovementSpeed", enemyMaxMovementSpeed);
        Vector2 move = Vector2.zero;

        if (toPatrolPosition.x > startingPosition.x) {
            move.x = -1f;
        }
        else {
            move.x = 1f;
        }
        targetVelocity = move * enemyMaxMovementSpeed;
    }

    public void MoveToPlayerPosition() {
        enemyMaxMovementSpeed = enemyMaxChasingSpeed;
        animator.SetFloat("MovementSpeed", enemyMaxMovementSpeed);
        Vector2 move = Vector2.zero;
        if (transform.position.x > PlayerController.Instance.transform.position.x) {
            move.x = -1f;
        }
        else {
            move.x = 1f;
        }
        targetVelocity = move * enemyMaxMovementSpeed;

    }

    public void TakeDamage(float damage) {
        if (damage >= currentHealth) {
            Die();
        } 
        else {
            currentHealth -= damage;
            animator.SetTrigger("Hurt"); //Play EenemyHurtAnimation
            healthBar.SetHealth(currentHealth);
        }
    }

    public void Die() {
        //Play Deathanimation
        //Instantiate dead body
        Destroy(gameObject);
    }
}