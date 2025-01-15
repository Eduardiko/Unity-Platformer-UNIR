using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMovement : MonoBehaviour
{
    [SerializeField] private Transform otherBackgroundTransform;
    [SerializeField] private float speed;

    private Vector3 offset;

    private void Start()
    {
        offset = otherBackgroundTransform.position - transform.position;
        offset.x = Mathf.Abs(offset.x);
        offset.y = Mathf.Abs(offset.y);
        offset.z = Mathf.Abs(offset.z);
    }

    private void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        //hardcoded af :(, he tenido problemas con el renderer
        if (transform.position.x < -19f)
            transform.position = otherBackgroundTransform.position + offset;
    }

}
