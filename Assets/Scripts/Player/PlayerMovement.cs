using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// Disable all movement if enabled
    /// </summary>
    public int DisabledType = 0;

    [SerializeField]
    private LayerMask groundLayer = Physics.AllLayers;

    //Ground
    // [SerializeField]
    float groundSpeed = 4f;
    // [SerializeField]
    // float runSpeed = 6f;
    // [SerializeField]
    float grAccel = 30f;

    //Air
    // [SerializeField]
    float airSpeed = 3f;
    // [SerializeField]
    float airAccel = 20f;

    //Jump
    // [SerializeField]
    float jumpUpSpeed = 9.2f;

    float wallFloorBarrier = 50f;

    [SerializeField]
    float gravityScalar = 1f;

    // Sliding
    // [SerializeField]
    float slideBoostForce = 5f;

    // [SerializeField]
    float airBoostForce = 5f;

    private float groundFriction = 5f;
    private float airFriction = 1f;

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

    // [SerializeField]
    const float MOUSE_SENS_MULT = 0.01f;
    const float GAMEPAD_SENS_MULT = 10f;

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

    private bool showVel = false;


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

        // PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

        //print(pStats.MaxHealth);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Start()
    {

    }

    void Update()
    {
        if (DisabledType == 1) { return; }
        if (DisabledType == 2) { rb.linearVelocity = Vector3.zero; return; }

        // col.material.dynamicFriction = 0f;
        PollInput();

        // camera stuff. default if input manager dies.
        bool useMouseLook = true;
        bool invertYLook = false;
        float xSense = 7f * MOUSE_SENS_MULT;
        float ySense = 7f * MOUSE_SENS_MULT;

        if (InputManager.Instance != null)
        {
            if (InputManager.Instance.GetCurrentInputDevice() == InputManager.InputDeviceType.Gamepad)
                useMouseLook = false;

            if (useMouseLook)
            {
                invertYLook = SettingsMenu.GetMouseInvertY();

                xSense = SettingsMenu.GetMouseXSens() * MOUSE_SENS_MULT;
                ySense = SettingsMenu.GetMouseYSens() * MOUSE_SENS_MULT;
            }
            else
            {
                invertYLook = SettingsMenu.GetGamepadInvertY();

                xSense = SettingsMenu.GetGamepadXSens() * GAMEPAD_SENS_MULT;
                ySense = SettingsMenu.GetGamepadYSens() * GAMEPAD_SENS_MULT;
            }
        }

        // print(ySense + " " + xSense);


        if (invertYLook)
            camXRot += lookDelta.y * xSense * (useMouseLook ? 1f : Time.deltaTime);
        else
            camXRot -= lookDelta.y * xSense * (useMouseLook ? 1f : Time.deltaTime);

        camXRot = Mathf.Clamp(camXRot, -80, 80);

        cameraTarget.localRotation = Quaternion.Euler(camXRot, 0, 0);
        orientation.Rotate(0, lookDelta.x * ySense * (useMouseLook ? 1f : Time.deltaTime), 0);


        Vector3 camPos = cameraTarget.localPosition;
        camPos.y = GetHalfHeight() - 0.15f;
        cameraTarget.localPosition = camPos;
    }

    void OnGUI()
    {
        // GUILayoutOption[] layout = { GUILayout.MinHeight(Screen.height / 10) };
        if (!showVel) return;

        GUILayout.Label($"<color=red><size={Screen.height / 20}>" + orientation.InverseTransformDirection(rb.linearVelocity).ToString());
        GUILayout.Label($"<color=blue><size={Screen.height / 20}>" + rb.linearVelocity.magnitude.ToString("F2"));
    }

    void FixedUpdate()
    {
        if (DisabledType > 0) return;

        // Walk(dir, running ? runSpeed : groundSpeed, grAccel);
        // AirMove(dir, airSpeed, airAccel);
        CheckForGround();

        if (grounded && !isOnSteepSlope)
        {
            isJumping = false;
        }

        if (wantToCrouch)
        {

            col.height = Mathf.Max(1.0f, col.height - Time.deltaTime * 20f);
            if (!isCrouched)
            {
                CrouchBoost(); // so lazy
                AirBoost();
            }

            isCrouched = true;
        }
        else
        {
            col.height = col.height = Mathf.Min(1.8f, col.height + Time.deltaTime * 20f);
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
                rb.AddForce(groundNormalAverage * (jumpUpSpeed - Mathf.Max(0f, rb.linearVelocity.y)), ForceMode.Impulse);
                isJumping = true;
            }
        }
        else if (grounded && isCrouched)
        {
            if (rb.linearVelocity.magnitude > groundSpeed)
            {
                Vector3 velToAdd = AirMovement(dir, groundSpeed, grAccel);
                rb.AddForce(velToAdd, ForceMode.VelocityChange);

                Vector3 friction = GetFrictionVector(groundSpeed, groundFriction);
                rb.AddForce(friction, ForceMode.VelocityChange);
            }
            else
            {
                Vector3 velocityToAdd = GroundedMovement(dir, groundSpeed, grAccel);
                velocityToAdd = Vector3.ProjectOnPlane(velocityToAdd, groundNormalAverage); // so we can walk on slanted surfaces.
                rb.AddForce(velocityToAdd, ForceMode.Acceleration);
            }

            if (wantToJump && !isJumping)
            {
                rb.AddForce(groundNormalAverage * (jumpUpSpeed - Mathf.Max(0f, rb.linearVelocity.y)), ForceMode.Impulse);
                isJumping = true;
            }
        }
        else
        {
            Vector3 velToAdd = AirMovement(dir, airSpeed, airAccel);
            rb.AddForce(velToAdd, ForceMode.VelocityChange);

            Vector3 friction = GetFrictionVector(groundSpeed, airFriction);
            rb.AddForce(friction, ForceMode.VelocityChange);
            // if (isJumping) isJumping = false; // this is cursed.
        }

        if (IsGrounded)
        {
            Vector3 curentVelocityThisFrame = rb.GetAccumulatedForce() * Time.deltaTime;
            curentVelocityThisFrame.y = 0;

            if (Vector3.Dot(curentVelocityThisFrame.normalized, dir.normalized) < 0.3)
            {
                if (curentVelocityThisFrame.magnitude < dir.magnitude)
                {
                    curentVelocityThisFrame = dir.normalized;
                }
            }

            if (rb.SweepTest(transform.worldToLocalMatrix * curentVelocityThisFrame.normalized, out RaycastHit hitInfo, 1f))
                StepHandle(curentVelocityThisFrame);
        }
    }

    public void UpdateVariablesWithStats(PlayerStats stats)
    {
        if (stats == null)
        {
            Debug.LogError("No player stats! Using default values!");
            stats = new PlayerStats();
            // return;
        }

        groundSpeed = stats.GroundSpeed;
        airSpeed = stats.AirSpeed;

        grAccel = stats.GroundSpeed * stats.GroundAccelerationPercentBase;
        airAccel = stats.AirSpeed * stats.AirAccelerationPercentBase;

        jumpUpSpeed = stats.JumpForce;
        // slideBoostForce = stats.SlideBoostForce;
        // airBoostForce = stats.AirBoostForce;

        slideBoostForce = stats.GroundSpeed * stats.SlideBoostPercentage;
        airBoostForce = stats.AirSpeed * stats.AirBoostPercentage;

        groundFriction = stats.GroundFriction;
        airFriction = stats.AirFriction;
    }

    private float GetHalfHeight()
    {
        return Mathf.Max(col.height / 2f, col.radius);
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
        Collider[] results = Physics.OverlapSphere(transform.position - Vector3.down * GetHalfHeight(), col.radius - 0.05f, groundLayer); // TODO ground mask?

        bool didHit = Physics.Raycast(transform.position, -transform.up, out RaycastHit hitInfo, GetHalfHeight() + 0.05f, groundLayer);

        if (results.Length > 0 || didHit)
        {
            if (didHit)
            {
                float angle = Vector3.Angle(hitInfo.normal, Vector3.up);

                grounded = true;
                isOnSteepSlope = angle > wallFloorBarrier;
                isOnSlightSlope = (angle > 1 && angle <= wallFloorBarrier);

                return;
            }
            else
            {
                grounded = true;
                isOnSteepSlope = false;
                isOnSlightSlope = false;
                return;
            }
        }

        grounded = false;
        isOnSteepSlope = false;
        isOnSlightSlope = false;
    }

    private bool WithinGroundRange(Vector3 point)
    {
        Vector3 feetPos = transform.position - Vector3.down * GetHalfHeight();

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


            if (Vector3.Distance(contactPos, transform.position) > col.radius - 0.03f) continue;

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

        // angle = Vector3.Angle(slopeNormalAverage, Vector3.up);
        // print(angle);

        // if (angle <= wallFloorBarrier)
        // {
        //     grounded = true;
        //     isOnSteepSlope = false;
        //     if (angle > 1)
        //     {
        //         isOnSlightSlope = true;
        //     }
        // }
        // else if (VectorToGround().magnitude > 0.2f)
        // {
        //     grounded = false;
        //     isOnSteepSlope = angle > wallFloorBarrier;
        //     isOnSlightSlope = (angle > 1 && angle <= wallFloorBarrier);
        // }
        // else
        // {
        //     grounded = false;
        //     isOnSteepSlope = angle > wallFloorBarrier;
        //     isOnSlightSlope = (angle > 1 && angle <= wallFloorBarrier);
        // }

    }

    void OnCollisionExit(Collision collision)
    {
        // if (ground.Contains(collision.collider)) ground.Remove(collision.collider);

        // if (collision.contactCount == 0)
        // {
        //     grounded = false;
        //     isOnSteepSlope = false;
        //     isOnSlightSlope = false;
        // }
    }

    private void StepHandle(Vector3 moveDirectionThisFrame)
    {
        moveDirectionThisFrame.y = 0;

        // Presuming that we casting in the move direction.
        // We also presume that something was hit, like a curb or a wall.

        float stepHeight = 0.3f;
        float stepDepthAllowed = 0.2f;

        float halfHeight = col.height / 2f;

        Vector3 halfAsVector = Vector3.up * GetHalfHeight();


        Vector3 point1 = transform.position + halfAsVector;
        Vector3 point2 = point1 - (halfAsVector * 2f);

        if (Physics.CapsuleCast(point1, point2, col.radius, Vector3.up, stepHeight)) return; // cant step up with something above our head.
        print("No air stopping");
        Vector3 airPoint = transform.position + (Vector3.up * stepHeight);

        point1 = airPoint + halfAsVector;
        point2 = point1 - (halfAsVector * 2f);

        // can we move over the step.
        // float depthCheck = (stepDepthAllowed > moveDirectionThisFrame.magnitude) ? stepDepthAllowed : moveDirectionThisFrame.magnitude;
        float depthCheck = stepDepthAllowed;


        if (Physics.CapsuleCast(point1, point2, col.radius, moveDirectionThisFrame.normalized, depthCheck)) return;
        print("No dir stopping");

        airPoint += moveDirectionThisFrame.normalized * depthCheck;

        point1 = airPoint + halfAsVector;
        point2 = point1 - (halfAsVector * 2f);

        RaycastHit[] hits = Physics.CapsuleCastAll(point1, point2, col.radius, Vector3.down, stepHeight);

        float currentY = float.MinValue;

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.gameObject.CompareTag(Constants.PlayerTag)) continue;

            print(hit.normal + " " + hit.point + " " + hit.barycentricCoordinate + " " + hit.transform.gameObject.name);
            Debug.DrawLine(hit.point, hit.point + Vector3.up, Color.green, 10f);


            if (hit.point.y > currentY) currentY = hit.point.y;
        }


        Vector3 pos = transform.position;

        float amountToAdd = currentY - (transform.position + (Vector3.down * halfHeight)).y;
        print(hits.Length);
        pos.y += Mathf.Max(amountToAdd, 0f);

        transform.position = pos;
    }

    Vector3 GetFrictionVector(float maxSpeed, float friction)
    {
        if (rb.linearVelocity.magnitude <= maxSpeed)
        {
            return Vector3.zero;
        }

        Vector3 counterDir = (-rb.linearVelocity).normalized;

        Vector3 currentVelSpeedNoY = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (currentVelSpeedNoY.magnitude > maxSpeed) friction *= currentVelSpeedNoY.magnitude / maxSpeed; // Increase the accel when overspeed.

        float projVel = Vector3.Dot(new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z), counterDir); // Vector projection of Current velocity onto accelDir.
        float accelVel = friction * Time.deltaTime; // Accelerated velocity in direction of movment

        // If necessary, truncate the accelerated velocity so the vector projection does not exceed max_velocity
        if (projVel + accelVel > maxSpeed)
            accelVel = Mathf.Max(0f, maxSpeed - projVel);

        return counterDir * accelVel;
    }


    Vector3 GroundedMovement(Vector3 wishDir, float maxSpeed, float acceleration)
    {
        wishDir = wishDir.normalized;
        Vector3 currentVelSpeedNoY = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (currentVelSpeedNoY.magnitude > maxSpeed) acceleration *= currentVelSpeedNoY.magnitude / maxSpeed; // Increase the accel when overspeed.

        Vector3 forceNeededForDesiredVelocity = wishDir * maxSpeed - currentVelSpeedNoY;

        if (forceNeededForDesiredVelocity.magnitude < 0.5f)
        {
            acceleration *= forceNeededForDesiredVelocity.magnitude / 0.5f; // slows down the accel? 
            //I presume because we reach our target speed. We want to override the current vel since we are on the ground.
        }

        Vector3 accelForce = forceNeededForDesiredVelocity.normalized * acceleration; // turn the force needed into a acceleration.
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

        rb.AddForce(GetBoostVector(boostDir, slideBoostForce), ForceMode.Impulse);

        // if (canSlideBoost) StartCoroutine(HandleCrouchBoostCoolDown());
    }

    void AirBoost()
    {
        if (grounded || !canAirBoost || appliedAirBoost) return;

        appliedAirBoost = true;

        Vector3 boostDir = dir;
        Vector3 vel = rb.linearVelocity;
        vel.y = 0f;
        float dotProduct = Vector3.Dot(vel, boostDir.normalized);


        rb.AddForce(GetBoostVector(boostDir, airBoostForce), ForceMode.VelocityChange);
    }

    // IEnumerator HandleCrouchBoostCoolDown()
    // {
    //     canSlideBoost = false;
    //     yield return new WaitForSeconds(1f);
    //     canSlideBoost = true;
    // }

    public void DisablePlayerMovement(int state)
    {
        DisabledType = state;
    }
}
