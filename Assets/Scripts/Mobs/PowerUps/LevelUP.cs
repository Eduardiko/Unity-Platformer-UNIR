using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUP : MonoBehaviour
{
    public PlayerUpgrades upgradeType;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.GetComponent<Player>().UpgradePlayer(upgradeType);
        Destroy(gameObject);
    }
}
