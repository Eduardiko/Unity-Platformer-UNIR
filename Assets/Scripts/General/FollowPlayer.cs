using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float smoothTime = 0.3f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (player == null)
            return;

        Vector3 targetPosition = player.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
