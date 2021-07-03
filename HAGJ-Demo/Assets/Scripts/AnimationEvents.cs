using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour {
    private GameObject player;

    private void OnEnable() {
        player = GameObject.FindWithTag("Player");
    }

    public void LittleAttack() {  //Notify of player little attack
        EventManager.Instance.NotifyOfOnPlayerLittleAttack(this);
        player.GetComponent<PlayerController>().LittleAttack_calledByAnimationEvents();
    }

    public void HeavyAttack() { //Notify of player heavy attack   
        EventManager.Instance.NotifyOfOnPlayerHeavyAttack(this);
        player.GetComponent<PlayerController>().HeavyAttack_calledByAnimationEvents();
    }

    public void EnemyAttack() { //Notify of enemy attack        
        EventManager.Instance.NotifyOfOnEnemyAttack(transform.position);
        transform.GetComponentInParent<BasicEnemyController>().EnemyAttackInitiated();    }
}
