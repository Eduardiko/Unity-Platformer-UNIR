using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public Interactable[] objectsToActivate;
    private bool activable = true;

    public void Interact()
    {
        activable = false;

        foreach (Interactable objectToActivate in objectsToActivate)
        {
            objectToActivate.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activable)
            Interact();
        Destroy(gameObject);
    }
}
