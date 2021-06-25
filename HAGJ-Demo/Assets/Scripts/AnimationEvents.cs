using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    public void LittleAttack() {
        EventManager.Instance.NotifyOfOnPlayerLittleAttack(gameObject);
        //Notify of player little attack
    }

    public void HeavyAttack() {
        EventManager.Instance.NotifyOfOnPlayerHeavyAttack(gameObject);
        //Notify of player heavy attack
    }

    public void EnemyAttack() {
        EventManager.Instance.NotifyOfOnEnemyAttack(gameObject);
        //Notify of enemy attack
    }
}
