using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITime : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    private float elapsedTime = 0;
    private bool isPaused = false;

    public bool IsPaused { get => isPaused; set => isPaused = value; }

    void Update()
    {
        if(!isPaused)
        {
            elapsedTime += Time.deltaTime;

            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
            int milliseconds = Mathf.FloorToInt((elapsedTime * 100) % 100);

            scoreText.text = minutes.ToString() + ":" + seconds.ToString() + ":" + milliseconds.ToString();
        }
    }
}
