using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


//by    _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|
// © 2025 domibron

/// <summary>
/// Used to replace the player movement so they can move through walls.
/// </summary>
public class NoClipPlayerController : MonoBehaviour
{
    /// <summary>
    /// Local store of the input vector.
    /// </summary>
    private Vector2 inputVector;

    /// <summary>
    /// Is the player pressing sprint key.
    /// </summary>
    private bool isSprinting;

    /// <summary>
    /// A store of the local velocity.
    /// </summary>
    private Vector3 currentMovement;

    // This if from the input system
    public void OnMove(InputValue value)
    {
        inputVector = value.Get<Vector2>();
    }

    // This if from the input system
    public void OnSprint(InputValue value)
    {
        isSprinting = value.isPressed;
    }

    // Update is called once per frame
    void Update()
    {
        /// convert the input to x y to x z.
        Vector3 InputDirection = new Vector3(inputVector.x, 0f, inputVector.y);
        // turn the input vector from a local movement vector into a world space vector.
        Vector3 WorldDirection = transform.TransformDirection(InputDirection);
        WorldDirection.Normalize();

        // hard set values for now.
        float speed = 3f;

        if (isSprinting)
        {
            speed = 9f;
        }

        // we set the local vel with the input.
        currentMovement.x = WorldDirection.x * speed;
        currentMovement.z = WorldDirection.z * speed;

        // we move the player.
        transform.Translate(currentMovement * Time.deltaTime);
    }
}
