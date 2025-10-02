using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//by    _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|
// © 2025 domibron


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
