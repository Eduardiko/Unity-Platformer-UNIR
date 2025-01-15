using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Swordfish : Mob
{
    [SerializeField] private GameObject propulsorBack;

    void Update()
    {
        if (ReadyToGetDestroyed && mobRenderer != null && !mobRenderer.isVisible)
                Destroy(gameObject);

        if (!canAct)
            return;

        Move(Vector3.right);
        if (propulsorBack != null)
            propulsorBack.SetActive(true);
    }
}
