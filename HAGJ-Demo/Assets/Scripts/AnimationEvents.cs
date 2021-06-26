using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    public void LittleAttack() {
        EventManager.Instance.NotifyOfOnPlayerLittleAttack(this);
        //Notify of player little attack
    }

    public void HeavyAttack() {
        EventManager.Instance.NotifyOfOnPlayerHeavyAttack(this);
        //Notify of player heavy attack
    }

    public void EnemyAttack() {
        EventManager.Instance.NotifyOfOnEnemyAttack(transform.position);
        //Notify of enemy attack
    }
}
