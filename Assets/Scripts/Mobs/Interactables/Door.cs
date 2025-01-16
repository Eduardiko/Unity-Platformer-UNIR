using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Door : Interactable
{
    public bool isOneUse = false;
    public Vector2 direction = Vector2.zero;
    public float distance = 0f;
    public float duration = 0f;

    private bool interactable = true;
    private int goNegative = 1;


    public override void Interact()
    {
        if(interactable)
        {
            interactable = false;
            StartCoroutine(TranslateOverTime(direction * goNegative, distance, duration));
            goNegative *= -1;
        }
    }

    private IEnumerator TranslateOverTime(Vector2 direction, float distance, float duration)
    {
        Vector2 startPosition = transform.position;
        Vector2 targetPosition = startPosition + direction.normalized * distance;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float progress = elapsedTime / duration;

            transform.position = Vector2.Lerp(startPosition, targetPosition, progress);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        transform.position = targetPosition;

        if(!isOneUse)
            interactable = true;
    }
}
