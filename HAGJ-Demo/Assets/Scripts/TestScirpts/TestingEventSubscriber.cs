using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingEventSubscriber : MonoBehaviour {
    

    void Start()    {
        EventManager.Instance.OnJumpInitiated += JumpTest_OnJumpInitiated;
    }

    private void JumpTest_OnJumpInitiated(object sender, EventArgs e) {
        Debug.Log("Jump");
    }
}