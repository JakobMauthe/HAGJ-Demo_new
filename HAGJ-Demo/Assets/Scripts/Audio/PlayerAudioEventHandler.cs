using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Processes calls from the player game object (collisions, animations) and sends to audio engine.
/// </summary>

public class PlayerAudioEventHandler : MonoBehaviour
{
    public void HeavyAttackAudioTrigger()
    {
        AudioManager.Instance.TriggerPlayerAttackAudio("heavy");
    }
    public void LightAttackAudioTrigger()
    {
        AudioManager.Instance.TriggerPlayerAttackAudio("light");
    }

    public void PlayerTakeDamageTrigger()
    {

    }

    public void PlayerAttackCollisionTrigger(Collision coll)
    {

    }


}
