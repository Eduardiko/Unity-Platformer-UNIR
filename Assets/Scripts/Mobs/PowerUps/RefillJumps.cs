using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefillJumps : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            Player player = collision.gameObject.GetComponent<Player>();
            player.AvailableAirJumps = player.MaxAirJumps;
            gameObject.SetActive(false);
            AudioManager.Instance.PlaySFX(2);
        }
    }
}
