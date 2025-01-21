using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Door : Interactable
{
    [SerializeField] private Vector2 direction = Vector2.zero;
    [SerializeField] private float distance = 0f;
    [SerializeField] private float duration = 0f;


    public override void Interact()
    {
        if(IsInteractable)
        {
            IsInteractable = false;
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
            IsInteractable = true;
    }
}
