using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    protected bool isOneUse = false;
    protected bool interactable = true;

    public abstract void Interact();

    //public abstract void ResetInteractable();
}
