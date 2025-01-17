using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State<T> : MonoBehaviour
{
    // No usar� <T> templates porque solo lo usar� en enemigos, pero s� como va :D

    protected T controller;

    public virtual void OnEnterState(T controller)
    {
        this.controller = controller;
    }
    public abstract void OnUpdateState();
    public abstract void OnExitState();
}
