using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Used to replace the player movement so they can move through walls.
/// </summary>
public class NoClipPlayerController : MonoBehaviour
{
    /// <summary>
    /// Local store of the input vector.
    /// </summary>
    private Vector3 inputVector;

    /// <summary>
    /// Is the player pressing sprint key.
    /// </summary>
    private bool isSprinting;

    /// <summary>
    /// A store of the local velocity.
    /// </summary>
    private Vector3 currentMovement;

    private Rigidbody rb;
    private Transform camera;

    InputAction move;
    InputAction jump;
    InputAction crouch;
    InputAction sprint;

    // doesnt work with synth perg.
    // This if from the input system
    // public void OnMove(InputValue value)
    // {
    //     inputVector.x = value.Get<Vector2>().x;
    //     inputVector.z = value.Get<Vector2>().y;
    // }


    // public void OnJump(InputValue value)
    // {
    //     if (value.Get<bool>())
    //         inputVector.y = 1f;
    // }

    // public void OnCrouch(InputValue value)
    // {
    //     if (value.Get<bool>())
    //         inputVector.y = -1f;
    // }

    // // This if from the input system
    // public void OnSprint(InputValue value)
    // {
    //     isSprinting = value.isPressed;
    // }

    void Awake()
    {
        move = InputSystem.actions.FindAction("Move");
        jump = InputSystem.actions.FindAction("Jump");
        crouch = InputSystem.actions.FindAction("Crouch");
        sprint = InputSystem.actions.FindAction("Sprint");
    }

    void Start()
    {
        camera = Camera.main.transform;

        rb = transform.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        inputVector = move.ReadValue<Vector2>();
        inputVector.z = (jump.IsPressed() ? 1 : 0) + (crouch.IsPressed() ? -1 : 0);
        isSprinting = sprint.IsPressed();

        Vector3 InputDirection = new Vector3(inputVector.x, inputVector.z, inputVector.y);
        // turn the input vector from a local movement vector into a world space vector.
        Vector3 WorldDirection = (camera ?? transform).TransformDirection(InputDirection);
        WorldDirection.Normalize();

        // hard set values for now.
        float speed = 3f;

        if (isSprinting)
        {
            speed = 9f;
        }

        // we set the local vel with the input.
        currentMovement = WorldDirection * speed;

        // we move the player.
        if (rb)
            rb.MovePosition(transform.position + currentMovement * Time.deltaTime);
        else
            transform.Translate(currentMovement * Time.deltaTime);
    }
}
