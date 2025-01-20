using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // list positions
    // list reactivate
    // return stats to player -> max jumps, canDash, 

    [SerializeField] private GameObject[] objectsToReturn;
    private Dictionary<GameObject, Vector2> initialPositions;

    [SerializeField] private int maxAirJumps;
    [SerializeField] private bool canDash;

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

            objectToReturn.transform.position = initialPositions[objectToReturn];
        }

        player.transform.position = gameObject.transform.position;
        player.MaxAirJumps = maxAirJumps;
        player.IsDashUnlocked = canDash;
    }
}
