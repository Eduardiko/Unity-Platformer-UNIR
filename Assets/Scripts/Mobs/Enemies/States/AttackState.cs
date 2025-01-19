using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AttackState : State<Enemy>
{
    [SerializeField] private float unChaseDistance = 10f;
    [SerializeField] private float shootTime = 1f;
    [SerializeField] private GameObject bullet;

    private float shootTimer = 0f;

    public override void OnEnterState(Enemy enemycontroller)
    {
        base.OnEnterState(enemycontroller);
    }

    public override void OnUpdateState()
    {
        if ((controller.PlayerTarget.transform.position - transform.position).magnitude >= unChaseDistance)
        {
            State<Enemy> chaseOut = controller.ChangeState(controller.ChaseState);
            if (chaseOut == null)
                controller.ChangeState(controller.PatrolState);
        }

        if (shootTimer <= 0f)
            Attack();
        else
            shootTimer -= Time.deltaTime;

    }

    public override void OnExitState()
    {
    }

    private void Attack()
    {
        shootTimer = shootTime;

        Vector3 directionToTarget = (controller.PlayerTarget.transform.position - transform.position).normalized;

        float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
        Quaternion rotationToTarget = Quaternion.Euler(0, 0, angle);

        GameObject newBullet = Instantiate(bullet, transform.position, rotationToTarget);

    }
}
