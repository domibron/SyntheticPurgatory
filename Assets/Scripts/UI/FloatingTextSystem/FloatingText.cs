using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FloatingText : MonoBehaviour
{
    public Vector3 moveDirection = Vector3.up;

    public float moveSpeed = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        //moveDirection = Random.insideUnitSphere;
        transform.LookAt(transform.position - (Camera.main.transform.position - transform.position));
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(moveDirection.normalized * moveSpeed * Time.deltaTime, Space.World);
    }
}
