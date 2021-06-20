using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyController : PhysicsObject {

    public float startHealth = 100;

    public HealthBar healthBar;

    protected float currentHealth;

    public float Health => currentHealth;

    void Awake() {
        currentHealth = startHealth;
        healthBar.SetMaxHealth(startHealth);
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