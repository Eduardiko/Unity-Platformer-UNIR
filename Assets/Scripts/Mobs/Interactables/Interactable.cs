using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    protected bool isOneUse = false;
    protected bool isInteractable = true;
    protected int goNegative = 1;

    public bool IsInteractable { get => isInteractable; set => isInteractable = value; }
    public int GoNegative { get => goNegative; set => goNegative = value; }

    public abstract void Interact();

    //public abstract void ResetInteractable();
}
