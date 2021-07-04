using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyController : PhysicsObject {

    [Header("References")]
    public HealthBar healthBar;
    public Animator animator;
    public GameObject prefab_DeadBody;
    public Transform attackPoint;
    public LayerMask playerLayer;

    [Header("Health")]
    [SerializeField, Range(0, 100)] int startHealth = 100;

    protected float currentHealth;

    [Header("Patrolling or Guard")]
    public bool guardOnly;
    [SerializeField, Range(-30f, 30f)] float patrolToPoint = 10f;

    [Header("Movement")]
    [SerializeField, Range(0.1f, 10f)] float enemyMaxPatrollingSpeed = 1.5f;
    [SerializeField, Range(1f, 100f)] float targetChaseRange = 10f;
    [SerializeField, Range(0.1f, 10f)] float enemyMaxChasingSpeed = 2.5f;
    private Vector3 startingPosition, toPatrolPosition;
    private bool patrolFirstWayActive = true;
    private float enemyMaxMovementSpeed = 1.5f;

    [Header("Attacking")]
    [SerializeField, Range(0, 100)] int attackDamage = 15;
    [SerializeField, Range(0.1f, 10f)] float startAttackRange = 2f;
    [SerializeField, Range(0.1f, 10f)] float attackRange = 2.5f;
    [SerializeField, Range(0.1f, 10f)] float attackRate = 0.75f;  

    private float nextAttackTime;

    [Header("Blocking")]
    [SerializeField, Range(0.1f, 5f)] float blockedDuration = 1f;

    private float notBlockedTime;
    private float notHurtTime;

    private GameObject player;
    private State state;

    private enum State {
        Patrol,
        ChaseTarget,
        Attack,
        Blocked,
        Hurt,
        Guard,
        BackToGuardPosition,
        Dying,
    }    

    public float Health => currentHealth;

    void OnEnable() {
        player = GameObject.FindWithTag("Player");
        startingPosition = transform.position;        //setting the StartingPosition
        toPatrolPosition = startingPosition + new Vector3(patrolToPoint, 0f, 0f); // setting the PatrolPosition
        currentHealth = startHealth;
        healthBar.SetMaxHealth(startHealth);
        if (guardOnly) {
            state = State.Guard;
        }
        else {
            state = State.Patrol;
        }
    }

    private void FindTarget() {
        if (Vector3.Distance(transform.position, player.transform.position) < targetChaseRange) {
            state = State.ChaseTarget;
            EventManager.Instance.NotifyOfOnEnemyStartChase(new Vector2 (transform.position.x, transform.position.y));
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
        float reachedPositionDistance = 1f;
        switch (state) {
            case State.Guard:
                FindTarget();
                break;
            case State.Patrol:
                if (patrolFirstWayActive) {
                    MoveToPatrolPosition();
                }
                else if (!patrolFirstWayActive) {
                    MoveToStartingPosition();
                }                
                if (Vector3.Distance(transform.position, toPatrolPosition) < reachedPositionDistance) {
                    patrolFirstWayActive = false;
                }
                else if (Vector3.Distance(transform.position, startingPosition) < reachedPositionDistance) {
                    patrolFirstWayActive = true;
                }
                FindTarget();
                break;
            case State.ChaseTarget:
                if (Vector3.Distance(transform.position, player.transform.position) < startAttackRange) {
                    state = State.Attack;
                    break;
                }
                MoveToPlayerPosition();
                if (Vector3.Distance(transform.position, player.transform.position) > targetChaseRange) {
                    if (guardOnly) {
                        state = State.BackToGuardPosition;
                    }
                    else {
                        state = State.Patrol;
                    }
                    break;
                }
                break;
            case State.Attack:
                if (Time.time > nextAttackTime) {
                    Attack();
                    nextAttackTime = Time.time + attackRate;
                }
                if (Vector3.Distance(transform.position, player.transform.position) > startAttackRange) {
                    state = State.ChaseTarget;
                    break;
                }
                break;
            case State.Blocked:
                if (Time.time< notBlockedTime) {
                    state = State.ChaseTarget;
                }
                break;
            case State.Hurt:
                if (Time.time < notHurtTime) {
                    state = State.ChaseTarget;
                }
                break;
            case State.BackToGuardPosition:                
                MoveToStartingPosition();
                if (Vector3.Distance(transform.position, startingPosition) < (reachedPositionDistance)) {
                    state = State.Guard;
                    animator.SetFloat("MovementSpeed", 0f);
                }                
                FindTarget();
                break;
            case State.Dying:
                break;
        }
    }

    public void Attack() {
        enemyMaxMovementSpeed = 0f;
        animator.SetFloat("MovementSpeed", enemyMaxMovementSpeed);
        animator.SetTrigger("Attack");
        
    }

    public void EnemyAttackInitiated() {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
        foreach (Collider2D enemy in hitEnemies) {
            PlayerController playerController = enemy.GetComponent <PlayerController>();
            if (playerController.IsBlocking) {
                state = State.Blocked;
                animator.SetTrigger("Blocked");
                playerController.BlockSuccesful();
                EventManager.Instance.NotifyOfOnBlockInitiated(new Vector2(transform.position.x, transform.position.y));
                notBlockedTime = Time.time + blockedDuration;
            }
            else {
                playerController.TakeDamage(attackDamage);
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
        if (guardOnly) {
            if (transform.position.x < startingPosition.x) {
                move.x = 1f;
            }
            else {
                move.x = -1f;
            }
        }
        else { 
            if (toPatrolPosition.x > startingPosition.x) {
                move.x = -1f;
            }
            else {
                move.x = 1f;
            }
        }
        targetVelocity = move * enemyMaxMovementSpeed;
    }

    public void MoveToPlayerPosition() {
        enemyMaxMovementSpeed = enemyMaxChasingSpeed;
        animator.SetFloat("MovementSpeed", enemyMaxMovementSpeed);
        Vector2 move = Vector2.zero;
        if (transform.position.x > player.transform.position.x) {
            move.x = -1f;
        }
        else {
            move.x = 1f;
        }
        targetVelocity = move * enemyMaxMovementSpeed;
    }

    public void TakeDamage(int damage) {
        if (damage >= currentHealth) {
            state = State.Dying;
            Die();
        } 
        else {
            currentHealth -= damage;
            EventManager.Instance.NotifyOfOnEnemyGetsHit(new Vector2(transform.position.x, transform.position.y));
            animator.SetTrigger("Hurt"); //Play EenemyHurtAnimation
            notHurtTime = Time.time + 0.5f;
            state = State.Hurt;
            healthBar.SetHealth(currentHealth);
        }
    }

    public void Die() {
        animator.SetTrigger("Death");
        EventManager.Instance.NotifyOfOnEnemyDie(new Vector2(transform.position.x, transform.position.y));        
        healthBar.gameObject.SetActive(false);
        Invoke("CreateACorpse", 1f);   //Instantiate dead body after 1 sec
        Invoke("DestroyGameObject", 1f);  //destroy the GameObject      
    }

    private void DestroyGameObject() {
        Destroy(gameObject);
    }

    //Instantiate dead body prefab at position of enemy
    private void CreateACorpse() {
        Vector3 corpsePosition = new Vector3(transform.position.x, transform.position.y + 2.4f, 0);
        if (facingRight) {
            Instantiate(prefab_DeadBody, corpsePosition, Quaternion.identity);
        }
        else {
            Instantiate(prefab_DeadBody, corpsePosition, Quaternion.Euler(0, 180, 0));
        }
    }
}