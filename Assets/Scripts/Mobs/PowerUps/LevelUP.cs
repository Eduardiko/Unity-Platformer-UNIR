using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUP : MonoBehaviour
{
    [SerializeField] private PlayerUpgrades upgradeType;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            collision.gameObject.GetComponent<Player>().UpgradePlayer(upgradeType);
            gameObject.SetActive(false);
        }

    }
}
