using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : State<Enemy>
{
    [SerializeField] private Transform patrolRoute;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float searchPlayerRadius;
    [SerializeField] private float patrolVelocity = 10f;

    private List<Vector2> patrolPoints = new List<Vector2>();
    private Vector2 currentDestination = new Vector2();
    private int currentPatrolIndex = 0;

    public override void OnEnterState(Enemy enemycontroller)
    {
        base.OnEnterState(enemycontroller);

        if(patrolPoints.Count == 0)
        {
            foreach (Transform patrolPoint in patrolRoute)
            {
                patrolPoints.Add(patrolPoint.position);
            }
        }

        currentDestination = patrolPoints[currentPatrolIndex];
    }

    public override void OnUpdateState()
    {
        transform.position = Vector2.MoveTowards(transform.position, currentDestination, patrolVelocity * Time.deltaTime);
        if(currentDestination == (Vector2)transform.position)
            SetNextPatrolPoint();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, searchPlayerRadius, playerLayer);
        if (colliders.Length > 0)
        {
            controller.PlayerTarget = colliders[0].gameObject.GetComponent<Player>();
            controller.ChangeState(controller.ChaseState);
        }
            
    }

    private void SetNextPatrolPoint()
    {
        currentPatrolIndex++;
        if (currentPatrolIndex > patrolPoints.Count - 1)
            currentPatrolIndex = 0;

        currentDestination = patrolPoints[currentPatrolIndex];
    }

    public override void OnExitState()
    {
        currentPatrolIndex = 0;
    }

}
