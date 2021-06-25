using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour {

    private static EventManager _instance;

    //Events
    public event EventHandler OnJumpInitiated, OnPlayerGetsHit, 
        OnPlayerDeath, OnPlayerLittleAttack, OnPlayerHeavyAttack,
        OnEnemyAttack;

    public static EventManager Instance { get { return _instance; } }

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

        public void NotifyOfOnJumpInitiated(object sender) {
        OnJumpInitiated?.Invoke(sender, EventArgs.Empty); 
    }

    public void NotifyOfOnPlayerGetsHit(object sender) {
        OnPlayerGetsHit?.Invoke(sender, EventArgs.Empty); 
    }
    public void NotifyOfOnPlayerDeath(object sender) {
        OnPlayerDeath?.Invoke(sender, EventArgs.Empty); 
    }
    public void NotifyOfOnPlayerLittleAttack(object sender) {
        OnPlayerLittleAttack?.Invoke(sender, EventArgs.Empty);
    }
    public void NotifyOfOnPlayerHeavyAttack(object sender) {
        OnPlayerHeavyAttack?.Invoke(sender, EventArgs.Empty);
    }
    public void NotifyOfOnEnemyAttack(object sender) {
        OnEnemyAttack?.Invoke(sender, EventArgs.Empty);
    }
}