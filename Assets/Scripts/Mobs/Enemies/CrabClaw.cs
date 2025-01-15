using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabClaw : Mob
{

    [SerializeField] private float timeBetweenBullets = 2f;
    [SerializeField] private float maxZRotation;
    [SerializeField] private float minRotationSpeed = 1f;
    [SerializeField] private float maxRotationSpeed = 5f;
    [SerializeField] private Vector3 pivotOffset;

    private float timeToShoot = 0;
    private float initialRotation; 
    private float targetRotation; 
    private float currentRotationSpeed;

    private void Start()
    {
        initialRotation = transform.eulerAngles.z;
        SetNewTargetRotation();
    }

    private void Update()
    {
        if (ReadyToGetDestroyed && !mobRenderer.isVisible)
            Destroy(gameObject);

        if (!canAct)
            return;

        ShootBehaviour();
        RotationBehaviour();
    }

    private void RotationBehaviour()
    {
        float step = currentRotationSpeed * Time.deltaTime;
        float deltaZ = Mathf.DeltaAngle(transform.localEulerAngles.z, targetRotation);

        if (Mathf.Abs(deltaZ) < step)
        {
            RotateAroundPivot(targetRotation - transform.localEulerAngles.z);
            SetNewTargetRotation();
            return;
        }

        RotateAroundPivot(Mathf.Sign(deltaZ) * step);
    }

    private void ShootBehaviour()
    {
        spawnPosition = transform.position;

        if (timeToShoot > 0)
            timeToShoot -= Time.deltaTime;
        else
        {
            timeToShoot = timeBetweenBullets;
            Shoot(transform.eulerAngles + new Vector3(0f,0f,125f));
        }
    }

    private void SetNewTargetRotation()
    {
        targetRotation = Random.Range(initialRotation, initialRotation + maxZRotation);
        currentRotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
    }

    private void RotateAroundPivot(float angle)
    {
        // Calculate the pivot point as an offset from the current position
        Vector3 pivotPoint = transform.position + pivotOffset;

        // Rotate around the calculated pivot point
        transform.RotateAround(pivotPoint, Vector3.forward, angle);
    }
}
