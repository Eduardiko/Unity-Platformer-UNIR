using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockDash : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().IsDashUnlocked = true;
            gameObject.SetActive(false);
            AudioManager.Instance.PlaySFX(1);
        }
    }
}
