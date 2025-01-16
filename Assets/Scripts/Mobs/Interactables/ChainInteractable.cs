using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainInteractable : Interactable
{
    public Interactable[] objectsToActivate;

    public override void Interact()
    {
        if(interactable)
        {
            foreach (Interactable objectToActivate in objectsToActivate)
            {
                objectToActivate.Interact();
            }

            if(isOneUse)
                interactable = false;
        }
        
    }
}
