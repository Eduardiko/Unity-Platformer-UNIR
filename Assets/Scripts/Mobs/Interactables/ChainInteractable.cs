using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainInteractable : Interactable
{
    public Interactable[] objectsToActivate;

    public override void Interact()
    {
        if(IsInteractable)
        {
            foreach (Interactable objectToActivate in objectsToActivate)
            {
                objectToActivate.Interact();
            }

            if(isOneUse)
                IsInteractable = false;
        }
        
    }
}
