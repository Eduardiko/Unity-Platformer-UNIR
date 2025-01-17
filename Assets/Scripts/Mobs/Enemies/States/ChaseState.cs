using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State<Enemy>
{
    [SerializeField] private float chaseVelocity = 10f;
    [SerializeField] private float unChaseDistance = 15f;
    [SerializeField] private float attackDistance = 5f;

    public override void OnEnterState(Enemy enemycontroller)
    {
        base.OnEnterState(enemycontroller);
    }
    public override void OnUpdateState()
    {
        transform.position = Vector2.MoveTowards(transform.position, controller.PlayerTarget.transform.position, chaseVelocity * Time.deltaTime);

        if((controller.PlayerTarget.transform.position - transform.position).magnitude >= unChaseDistance)
            controller.ChangeState(controller.PatrolState);
        else if ((controller.PlayerTarget.transform.position - transform.position).magnitude <= attackDistance && attackDistance != 0)
            controller.ChangeState(controller.AttackState);
    }

    public override void OnExitState()
    {
    }
}
