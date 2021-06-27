using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowGameObject : MonoBehaviour
{
    [SerializeField] Transform objectToFollow;

    private void LateUpdate()
    {
        transform.position = objectToFollow.position;
    }
}
