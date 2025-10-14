using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    //Ground
    [SerializeField]
    float groundSpeed = 4f;
    // [SerializeField]
    // float runSpeed = 6f;
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

    [SerializeField]
    float gravityScalar = 1f;

    // Sliding
    [SerializeField]
    float SlideBoostForce = 5f;

    [SerializeField]
    float AirBoostForce = 5f;

    public bool IsGrounded { get => grounded; }

    bool grounded;
    bool isJumping;
    bool isOnSteepSlope;
    bool isOnSlightSlope;

    bool isCrouched = false;

    bool canSlideBoost = true;
    bool appliedSlideBoost = false;
    bool canAirBoost = true;
    bool appliedAirBoost = false;


    Vector3 groundNormalAverage = Vector3.up;

    CapsuleCollider col;
    Rigidbody rb;

    [SerializeField]
    Transform cameraTarget;

    [SerializeField]
    Transform orientation;

    [SerializeField]
    float sens = 1f;

    Vector2 lookDelta = Vector2.zero;
    float camXRot = 0f;

    // Input
    Vector3 dir = Vector3.zero;
    bool wantToJump;
    bool wantToCrouch;

    InputAction movementInput;
    InputAction jumpInput;
    InputAction crouchInput;
    InputAction lookInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        movementInput = InputSystem.actions.FindAction("Move");
        jumpInput = InputSystem.actions.FindAction("Jump");
        crouchInput = InputSystem.actions.FindAction("Crouch");
        lookInput = InputSystem.actions.FindAction("Look");

        // var bindings = BindingFlags.Public | BindingFlags.Instance;

        // // PlayerStats stats = new PlayerStats();

        // FieldInfo[] propertyInfos = typeof(PlayerStats).GetFields(bindings);

        // foreach (var propertyInfo in propertyInfos)
        // {
        //     print(propertyInfo.Name);
        // }

        // propertyInfos[0].SetValue(this, 10);

        PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

        print(pStats.MaxHealth);
    }

    void Start()
    {

    }

    void Update()
    {
        // col.material.dynamicFriction = 0f;
        PollInput();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // camera stuff.
        camXRot -= lookDelta.y * sens;

        camXRot = Mathf.Clamp(camXRot, -80, 80);

        cameraTarget.localRotation = Quaternion.Euler(camXRot, 0, 0);

        orientation.Rotate(0, lookDelta.x * sens, 0);
    }

    void OnGUI()
    {
        // GUILayoutOption[] layout = { GUILayout.MinHeight(Screen.height / 10) };

        GUILayout.Label($"<color=red><size={Screen.height / 20}>" + orientation.InverseTransformDirection(rb.linearVelocity).ToString());
        GUILayout.Label($"<color=blue><size={Screen.height / 20}>" + rb.linearVelocity.magnitude.ToString("F2"));
    }

    void FixedUpdate()
    {
        // Walk(dir, running ? runSpeed : groundSpeed, grAccel);
        // AirMove(dir, airSpeed, airAccel);

        if (wantToCrouch)
        {
            col.height = Mathf.Max(0.6f, col.height - Time.deltaTime * 20f);
            if (!isCrouched)
            {
                CrouchBoost(); // so lazy
                AirBoost();
            }

            isCrouched = true;
        }
        else
        {
            col.height = Mathf.Min(1.8f, col.height + Time.deltaTime * 20f);
            isCrouched = false;
            appliedSlideBoost = false;
        }

        // Gravity.
        if (isOnSteepSlope)
        {
            float gravOnSlope = Physics.gravity.magnitude * gravityScalar * 3f;


            Vector3 gravAlongSlope = (Vector3.down * gravOnSlope).normalized + Vector3.ProjectOnPlane(Vector3.down * gravOnSlope, groundNormalAverage).normalized;



            rb.AddForce(gravAlongSlope.normalized * gravOnSlope, ForceMode.Acceleration);

            Debug.DrawLine(transform.position, transform.position + gravAlongSlope, Color.green, 10f);
            Debug.DrawLine(transform.position, transform.position + Vector3.down, Color.red, 10f);
        }
        else
        {
            rb.AddForce(GetGravityVector(), ForceMode.Acceleration);
        }

        if (grounded)
        {
            appliedAirBoost = false;
        }

        if (grounded && !isCrouched)
        {
            Vector3 velocityToAdd = GroundedMovement(dir, groundSpeed, grAccel);
            velocityToAdd = Vector3.ProjectOnPlane(velocityToAdd, groundNormalAverage); // so we can walk on slanted surfaces.
            rb.AddForce(velocityToAdd, ForceMode.Acceleration);

            // counter slope sliding when not inputing anything and dont have any vel, aka play is stopped so stop the player.
            if (isOnSlightSlope && rb.linearVelocity.magnitude < 0.2f && dir.magnitude <= 0.1f)
            {
                rb.AddForce(-rb.linearVelocity, ForceMode.VelocityChange);
            }

            // jumping
            if (wantToJump && !isJumping)
            {
                rb.AddForce(groundNormalAverage * jumpUpSpeed, ForceMode.Impulse);
                isJumping = true;
            }
        }
        else if (grounded && isCrouched)
        {
            if (rb.linearVelocity.magnitude > groundSpeed)
            {
                Vector3 velToAdd = AirMovement(dir, groundSpeed, grAccel);
                rb.AddForce(velToAdd, ForceMode.VelocityChange);
            }
            else
            {
                Vector3 velocityToAdd = GroundedMovement(dir, groundSpeed, grAccel);
                velocityToAdd = Vector3.ProjectOnPlane(velocityToAdd, groundNormalAverage); // so we can walk on slanted surfaces.
                rb.AddForce(velocityToAdd, ForceMode.Acceleration);
            }

            if (wantToJump && !isJumping)
            {
                rb.AddForce(groundNormalAverage * jumpUpSpeed, ForceMode.Impulse);
                isJumping = true;
            }
        }
        else
        {
            Vector3 velToAdd = AirMovement(dir, airSpeed, airAccel);
            rb.AddForce(velToAdd, ForceMode.VelocityChange);

            if (isJumping) isJumping = false; // this is cursed.
        }


    }

    private void PollInput()
    {
        Vector2 inputVector = movementInput.ReadValue<Vector2>();
        Vector3 inputInWorld = new Vector3(inputVector.x, 0, inputVector.y);

        dir = orientation.transform.TransformDirection(inputInWorld);

        wantToJump = jumpInput.IsPressed();

        wantToCrouch = crouchInput.IsPressed();

        lookDelta = lookInput.ReadValue<Vector2>();
    }

    private void CheckForGround()
    {
        Collider[] results = Physics.OverlapSphere(transform.position - Vector3.down * (col.height / 2f), col.radius - 0.03f); // TODO ground mask?

        foreach (Collider collider in results)
        {

        }
    }

    private bool WithinGroundRange(Vector3 point)
    {
        Vector3 feetPos = transform.position - Vector3.down * (col.height / 2f);

        float radius = col.radius - 0.01f;

        return Vector3.Distance(point, feetPos) < radius;

    }

    Vector3 GetGravityVector()
    {
        if (grounded && !isOnSteepSlope)
        {
            Vector3 gravityVector = Physics.gravity - Vector3.ProjectOnPlane(Physics.gravity, groundNormalAverage);
            return gravityVector * gravityScalar;
        }
        else
        {
            return Physics.gravity * gravityScalar;
        }
    }

    void OnCollisionStay(Collision collision)
    {

        Vector3 slopeNormalAverage = Vector3.zero;

        float angle;
        int validContacts = 0;
        foreach (ContactPoint contact in collision.contacts)
        {

            Vector3 contactPos = contact.point;
            if (WithinGroundRange(contactPos)) continue;

            contactPos.y = transform.position.y;


            if (Vector3.Distance(contactPos, transform.position) > col.radius) continue;

            angle = Vector3.Angle(slopeNormalAverage, Vector3.up);


            if (angle > 85) continue;
            validContacts++;
            slopeNormalAverage += contact.normal;

        }
        slopeNormalAverage /= validContacts;
        groundNormalAverage = slopeNormalAverage;

        if (validContacts == 0)
        {
            slopeNormalAverage = Vector3.up;
            groundNormalAverage = slopeNormalAverage;
        }

        angle = Vector3.Angle(slopeNormalAverage, Vector3.up);
        // print(angle);

        if (angle <= wallFloorBarrier)
        {
            grounded = true;
            isOnSteepSlope = false;
            if (angle > 1)
            {
                isOnSlightSlope = true;
            }
        }
        else if (VectorToGround().magnitude > 0.2f)
        {
            grounded = false;
            isOnSteepSlope = angle > wallFloorBarrier;
            isOnSlightSlope = (angle > 1 && angle <= wallFloorBarrier);
        }
        else
        {
            grounded = false;
            isOnSteepSlope = angle > wallFloorBarrier;
            isOnSlightSlope = (angle > 1 && angle <= wallFloorBarrier);
        }

    }

    void OnCollisionExit(Collision collision)
    {
        // if (ground.Contains(collision.collider)) ground.Remove(collision.collider);

        if (collision.contactCount == 0)
        {
            grounded = false;
            isOnSteepSlope = false;
            isOnSlightSlope = false;
        }
    }


    Vector3 GroundedMovement(Vector3 wishDir, float maxSpeed, float acceleration)
    {
        wishDir = wishDir.normalized;
        Vector3 currentVelSpeedNoY = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (currentVelSpeedNoY.magnitude > maxSpeed) acceleration *= currentVelSpeedNoY.magnitude / maxSpeed; // what the fuck. increase the accel if overspeed. But you need to to counter?

        Vector3 foceNeededForDesiredForce = wishDir * maxSpeed - currentVelSpeedNoY;

        if (foceNeededForDesiredForce.magnitude < 0.5f)
        {
            acceleration *= foceNeededForDesiredForce.magnitude / 0.5f; // slows down the accel? 
            //I presume because we reach our target speed. We want to override the current vel since we are on the ground.
        }

        Vector3 accelForce = foceNeededForDesiredForce.normalized * acceleration; // turn the force needed into a acceleration.
        float magn = accelForce.magnitude; // this makes no sense.
        accelForce = accelForce.normalized; // because you did this.
        accelForce *= magn; // already.

        return accelForce; // Forcemode.Acceleration);

    }

    Vector3 AirMovement(Vector3 wishDir, float maxSpeed, float acceleration)
    {
        wishDir.Normalize();

        float projVel = Vector3.Dot(new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z), wishDir); // Vector projection of Current velocity onto accelDir.
        float accelVel = acceleration * Time.deltaTime; // Accelerated velocity in direction of movment

        // If necessary, truncate the accelerated velocity so the vector projection does not exceed max_velocity
        if (projVel + accelVel > maxSpeed)
            accelVel = Mathf.Max(0f, maxSpeed - projVel);

        return wishDir.normalized * accelVel; // ForceMode.VelocityChange);
    }

    Vector3 GetBoostVector(Vector3 wishDir, float boostSpeed)
    {
        wishDir.Normalize();

        float projVel = Vector3.Dot(new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z), wishDir); // Vector projection of Current velocity onto accelDir.
        float boostVel = boostSpeed; // Accelerated velocity in direction of movment

        // If necessary, truncate the accelerated velocity so the vector projection does not exceed max_velocity
        if (projVel + boostVel > boostSpeed)
            boostVel = Mathf.Max(0f, boostSpeed - projVel);

        return wishDir.normalized * boostVel; // ForceMode.VelocityChange);
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

    void CrouchBoost()
    {
        if (!grounded || !canSlideBoost || appliedSlideBoost) return;

        appliedSlideBoost = true;

        Vector3 boostDir = dir;

        rb.AddForce(GetBoostVector(boostDir, SlideBoostForce), ForceMode.Impulse);

        // if (canSlideBoost) StartCoroutine(HandleCrouchBoostCoolDown());
    }

    void AirBoost()
    {
        if (grounded || !canAirBoost || appliedAirBoost) return;

        appliedAirBoost = true;

        Vector3 boostDir = dir;

        rb.AddForce(GetBoostVector(boostDir, AirBoostForce), ForceMode.Impulse);
    }

    // IEnumerator HandleCrouchBoostCoolDown()
    // {
    //     canSlideBoost = false;
    //     yield return new WaitForSeconds(1f);
    //     canSlideBoost = true;
    // }
}
