using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pufferfish : Mob
{
    [SerializeField] private float timeBetweenBullets = 2f;
    private float timeToShoot = 0;

    void Update()
    {
        if (ReadyToGetDestroyed && mobRenderer != null && !mobRenderer.isVisible)
            Destroy(gameObject);

        if (!canAct)
            return;

        spawnPosition = transform.position;

        if (timeToShoot > 0)
            timeToShoot -= Time.deltaTime;
        else
        {
            timeToShoot = timeBetweenBullets;
            Shoot();
        }
    }
}
