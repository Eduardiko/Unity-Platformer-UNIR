using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State<T> : MonoBehaviour
{
    // No usaré <T> templates porque solo lo usaré en enemigos, pero sé como va :D

    protected T controller;

    public virtual void OnEnterState(T controller)
    {
        this.controller = controller;
    }
    public abstract void OnUpdateState();
    public abstract void OnExitState();
}
