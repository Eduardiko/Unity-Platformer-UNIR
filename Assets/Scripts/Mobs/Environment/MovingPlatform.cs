using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector2[] patrolPoints;
    public float patrolSpeed = 2f;

    private int currentPatrolIndex = 0;

    private Vector2 targetPoint;

    void Start()
    {
        gameObject.transform.position = patrolPoints[0];
        targetPoint = patrolPoints[1];
    }

    void Update()
    {
        gameObject.transform.position = Vector2.MoveTowards(gameObject.transform.position, targetPoint, patrolSpeed * Time.deltaTime);

        if (Vector2.Distance(gameObject.transform.position, targetPoint) < 0.1f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            targetPoint = patrolPoints[currentPatrolIndex];
        }
    }
}
