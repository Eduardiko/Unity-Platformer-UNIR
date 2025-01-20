using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public Interactable[] objectsToActivate;

    public void Interact()
    {
        foreach (Interactable objectToActivate in objectsToActivate)
        {
            objectToActivate.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Interact();
        
        gameObject.SetActive(false);

        AudioManager.Instance.PlaySFX(5);

    }
}
