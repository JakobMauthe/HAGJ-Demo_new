using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyController : PhysicsObject {

    private enum State {
        Patrol,
        ChaseTarget,
        Attack,
    }
    private State state;

    public float startHealth = 100;
    public HealthBar healthBar;
    protected float currentHealth;

    private Vector3 startingPosition, toPatrolPosition;
    private bool patrolFirstWayActive = true;
    public float enemyMaxMovementSpeed = 4f;

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

    public float Health => currentHealth;
   

    void OnEnable() {
        startingPosition = transform.position;
        toPatrolPosition = GetRandomPatrollingPosition();
        toPatrolPosition = startingPosition - new Vector3(20f, 0f, 0f); // delete me
        currentHealth = startHealth;
        healthBar.SetMaxHealth(startHealth);
        state = State.Patrol;
    }

    private void FindTarget() {
        if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) < targetChaseRange) {
            state = State.ChaseTarget;
        }
    }

    public Vector3 GetRandomPatrollingPosition() {
        return startingPosition + GetRandomDirection() * Random.Range(10f, 50f);
    }

    //Get Random Normalized x direction
    public Vector3 GetRandomDirection() {        
        return new Vector3(Random.Range(-1f, 1f), transform.position.y).normalized;
    }

    protected override void ComputeVelocity() {
        Debug.Log(state);
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
        }
    }

    public void Attack() {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
        foreach (Collider2D enemy in hitEnemies) {
            enemy.GetComponent<PlayerController>().TakeDamage((float)attackDamage);
        }
    }

    public void MoveToPatrolPosition() {
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
            //Play EenemyHurtAnimation
            healthBar.SetHealth(currentHealth);
        }
    }

    public void Die() {
        //Play Deathanimation
        //Instantiate dead body
        Destroy(gameObject);
    }
}