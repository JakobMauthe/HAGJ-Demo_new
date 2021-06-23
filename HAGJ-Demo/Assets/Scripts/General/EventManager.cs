using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour {

    private static EventManager _instance;

    //Events
    public event EventHandler OnJumpInitiated, OnPlayerGetsHit, OnPlayerDeath;


    public static EventManager Instance { get { return _instance; } }

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        }
        else {
            _instance = this;
        }
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
}