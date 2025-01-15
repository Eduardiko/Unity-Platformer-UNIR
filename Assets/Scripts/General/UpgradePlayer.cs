using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePlayer : MonoBehaviour
{
    [SerializeField] private float speed = 3f;

    private void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().UpgradePlayer(PlayerUpgrades.DOUBLE_JUMP);
            Destroy(gameObject);
        }

        if (collision.gameObject.tag == "MobKiller")
            Destroy(gameObject);
    }
}
