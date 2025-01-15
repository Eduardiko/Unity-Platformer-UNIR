using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeUp : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    void Start()
    {
        transform.eulerAngles = new Vector3(0f, 0f, 90f);
    }

    private void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().Heal(10);
            Destroy(gameObject);
        }

        if (collision.gameObject.tag == "MobKiller")
            Destroy(gameObject);
    }
}
