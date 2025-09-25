using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    //Ground
    [SerializeField]
    float groundSpeed = 4f;
    [SerializeField]
    float runSpeed = 6f;
    [SerializeField]
    float grAccel = 20f;

    //Air
    [SerializeField]
    float airSpeed = 3f;
    [SerializeField]
    float airAccel = 20f;

    //Jump
    [SerializeField]
    float jumpUpSpeed = 9.2f;
    [SerializeField]
    float dashSpeed = 6f;

    float wallFloorBarrier = 40f;

    //States
    bool running;
    bool jump;
    bool crouched;
    bool grounded;

    Collider ground;

    Vector3 groundNormal = Vector3.up;

    CapsuleCollider col;


    Rigidbody rb;
    Vector3 dir = Vector3.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        col.material.dynamicFriction = 0f;
        dir = Direction();

    }

    void FixedUpdate()
    {

        // Walk(dir, running ? runSpeed : groundSpeed, grAccel);
        // AirMove(dir, airSpeed, airAccel);

    }



    private Vector3 Direction()
    {
        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(hAxis, 0, vAxis);
        return rb.transform.TransformDirection(direction);
    }



    void OnCollisionStay(Collision collision)
    {
        if (collision.contactCount > 0)
        {
            float angle;

            foreach (ContactPoint contact in collision.contacts)
            {
                angle = Vector3.Angle(contact.normal, Vector3.up);
                if (angle < wallFloorBarrier)
                {
                    grounded = true;
                    groundNormal = contact.normal;
                    ground = contact.otherCollider;
                    return;
                }
            }

            if (VectorToGround().magnitude > 0.2f)
            {
                grounded = false;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.contactCount == 0)
        {
            grounded = false;
        }
    }


    Vector3 Walk(Vector3 wishDir, float maxSpeed, float acceleration)
    {

        //if (crouched) acceleration = 0.5f;
        wishDir = wishDir.normalized;
        Vector3 spid = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (spid.magnitude > maxSpeed) acceleration *= spid.magnitude / maxSpeed;
        Vector3 direction = wishDir * maxSpeed - spid;

        if (direction.magnitude < 0.5f)
        {
            acceleration *= direction.magnitude / 0.5f;
        }

        direction = direction.normalized * acceleration;
        float magn = direction.magnitude;
        direction = direction.normalized;
        direction *= magn;

        Vector3 slopeCorrection = groundNormal * Physics.gravity.y / groundNormal.y;
        slopeCorrection.y = 0f;
        //if(!crouched)
        direction += slopeCorrection;

        return direction; // Forcemode.Acceleration);

    }

    Vector3 AirMove(Vector3 wishDir, float maxSpeed, float acceleration)
    {
        float projVel = Vector3.Dot(new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z), wishDir); // Vector projection of Current velocity onto accelDir.
        float accelVel = acceleration * Time.deltaTime; // Accelerated velocity in direction of movment

        // If necessary, truncate the accelerated velocity so the vector projection does not exceed max_velocity
        if (projVel + accelVel > maxSpeed)
            accelVel = Mathf.Max(0f, maxSpeed - projVel);

        return wishDir.normalized * accelVel; // ForceMode.VelocityChange);
    }


    Vector3 VectorToGround()
    {
        Vector3 position = transform.position;
        RaycastHit hit;
        if (Physics.Raycast(position, Vector3.down, out hit, 1f))
        {
            return hit.point - position;
        }
        else
        {
            return Vector3.positiveInfinity;
        }
    }
}
