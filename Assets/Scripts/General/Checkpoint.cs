using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsToReturn;
    [SerializeField] private int maxAirJumps;
    [SerializeField] private bool canDash;

    private Dictionary<GameObject, Vector2> initialPositions;
    private Player player;

    private void Start()
    {
        initialPositions = new Dictionary<GameObject, Vector2>();

        foreach (GameObject objectToReturn in objectsToReturn)
        {
            initialPositions.Add(objectToReturn, new Vector2(objectToReturn.transform.position.x, objectToReturn.transform.position.y));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player = collision.gameObject.GetComponent<Player>();
            player.LastCheckpoint = this;
            AudioManager.Instance.PlaySFX(7);
        }
    }

    public void ResetArea()
    {
        foreach (GameObject objectToReturn in objectsToReturn)
        {
            objectToReturn.SetActive(true);
            
            if(objectToReturn.TryGetComponent<Interactable>(out Interactable _interactable))
            {
                _interactable.StopAllCoroutines();
                _interactable.IsInteractable = true;
                _interactable.GoNegative = 1;
            } 
            else if (objectToReturn.TryGetComponent<Enemy>(out Enemy _enemy))
            {
                _enemy.Health = 3;
            }

            objectToReturn.transform.position = initialPositions[objectToReturn];
        }

        player.transform.position = gameObject.transform.position;
        player.MaxAirJumps = maxAirJumps;
        player.IsDashUnlocked = canDash;
    }
}
