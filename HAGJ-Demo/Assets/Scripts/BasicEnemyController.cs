using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyController : PhysicsObject {

    public float startHealth = 100;

    protected float currentHealth;

    public float Health => currentHealth;

    void Awake() {
        currentHealth = startHealth;
    }

    public void DealDamage(float damage) {
        if (damage > currentHealth) {
            Die();
        } 
        else {
            currentHealth -= damage;
        }
    }

    public void Die() {
        Destroy(gameObject);
    }
}
