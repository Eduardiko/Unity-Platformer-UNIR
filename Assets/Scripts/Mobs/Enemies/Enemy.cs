using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private PatrolState patrolState;
    private AttackState attackState;
    private ChaseState chaseState;

    private State<Enemy> currentState;

    private Player playerTarget;

    private int health = 3;

    public PatrolState PatrolState { get => patrolState; }
    public AttackState AttackState { get => attackState; }
    public ChaseState ChaseState { get => chaseState; }
    public Player PlayerTarget { get => playerTarget; set => playerTarget = value; }
    public int Health { get => health; set => health = value; }

    void Start()
    {
        patrolState = GetComponent<PatrolState>();
        attackState = GetComponent<AttackState>();
        chaseState = GetComponent<ChaseState>();

        currentState = patrolState;
        patrolState.OnEnterState(this);

        playerTarget = null;
    }

    void Update()
    {
        if(currentState != null)
            currentState.OnUpdateState();
    }

    public void ChangeState(State<Enemy> state)
    {
        if(currentState != null)
        {
            currentState.OnExitState();
            currentState = state;
            currentState.OnEnterState(this);
        }
    }

    public void ApplyDamage()
    {
        health--;

        if (health <= 0)
            Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
