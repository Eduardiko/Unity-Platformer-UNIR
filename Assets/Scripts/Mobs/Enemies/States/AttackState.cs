using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State<Enemy>
{
    public override void OnEnterState(Enemy enemycontroller)
    {
        base.OnEnterState(enemycontroller);
    }

    public override void OnExitState()
    {
    }

    public override void OnUpdateState()
    {
        print("ATTACK");
    }
}
