using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Player playerPrefab;
    [SerializeField] private GameObject healthUIPrefab;

    [SerializeField] private GameObject endLevelMenu;
    [SerializeField] private Transform healthUIGroup;
    [SerializeField] private TextMeshProUGUI scoreText;

    [HideInInspector] public static int playerScore = 0;

    private void Update()
    {
        scoreText.text = "Score: " + playerScore;

        if (playerPrefab == null)
            endLevelMenu.SetActive(true);

        foreach (RectTransform child in healthUIGroup)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < playerPrefab.health; i++)
        {
            GameObject.Instantiate(healthUIPrefab, healthUIGroup);
        }
    }
}
