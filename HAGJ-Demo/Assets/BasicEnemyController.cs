using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyConroller : PhysicsObject {

    public float startHealth = 100;

    protected float health;

    public float Health => health;

    public void DealDamage(float Damage) {

    }

    public void Die() {
        Destroy(gameObject);
    }
}
