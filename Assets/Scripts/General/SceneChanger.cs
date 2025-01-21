using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private string onCollisionSceneName = "";
    [SerializeField] private UITime uiTime;

    private void Start()
    {
        StartCoroutine(FadeOut());
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
            ReloadScene();
    }

    public void ChangeScene(string sceneName)
    {
        StartCoroutine(FadeToBlackBeforeLoad(sceneName));
    }

    public void ReloadScene()
    {
        StartCoroutine(FadeToBlackBeforeLoad(SceneManager.GetActiveScene().name)); 
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Destroy(collision.gameObject.GetComponent<PlayerInput>());
            collision.gameObject.GetComponent<Rigidbody2D>().velocity /= 4;
            fadeDuration = 3f;
            ChangeScene(onCollisionSceneName);
            AudioManager.Instance.PlaySFX(6);
        }
    }

    public IEnumerator FadeToBlackBeforeLoad(string sceneName)
    {
        if(uiTime != null)
        {
            uiTime.IsPaused = true;
        }

        if (fadeImage == null)
            yield break;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);

            Color color = fadeImage.color;
            color.a = alpha;
            fadeImage.color = color;

            yield return null;

        }

        SceneManager.LoadScene(sceneName);
    }

    public IEnumerator FadeOut()
    {
        if (fadeImage == null)
            yield break;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1 - (elapsedTime / fadeDuration));

            Color color = fadeImage.color;
            color.a = alpha;
            fadeImage.color = color;

            yield return null;
        }
    }

}