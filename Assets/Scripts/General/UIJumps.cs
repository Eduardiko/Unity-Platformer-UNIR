using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIJumps : MonoBehaviour
{
    [SerializeField] private GameObject[] jumpUIObjects;
    [SerializeField] private Player player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int counter = 0;

        foreach (GameObject jumpUIObject in jumpUIObjects)
        {
            jumpUIObject.SetActive(false);

            if (counter < player.AvailableAirJumps)
            {
                jumpUIObject.SetActive(true);
                counter++;
            }
        }
    }
}
