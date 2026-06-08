using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// The player movement controller.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// The player movement disable type.
    /// </summary>
    public enum DisabledType
    {
        None,
        MovementOnly,
        MouseOnly,
        All,
    }

    /// <summary>
    /// The current disable state of the player movement.
    /// </summary>
    private DisabledType disabledState = DisabledType.None;

    /// <summary>
    /// ALl the layers that the player can run and jump on.
    /// </summary>
    [SerializeField]
    private LayerMask groundLayer = Physics.AllLayers;

    //Ground
    /// <summary>
    /// The speed the player will move on the ground when sprinting. Stats set this.
    /// </summary>
    float runSpeed = 4f;

    /// <summary>
    /// The speed the player will move on the ground when they are not sprinting.
    /// </summary>
    float walkSpeed = 2f;

    /// <summary>
    /// How fast the player accelerate on the ground. Stats set this.
    /// </summary>
    float grAccel = 30f;

    //Air
    /// <summary>
    /// The speed the player moves in the air.
    /// </summary>
    float airSpeed = 3f;

    /// <summary>
    /// How fast the player accelerate in the air.
    /// </summary>
    float airAccel = 20f;

    //Jump
    /// <summary>
    /// The speed the player receives when they press jump whilst on the ground.
    /// </summary>
    float jumpUpSpeed = 9.2f;

    /// <summary>
    /// The degrees at which a floor is considered a wall. Floor angle compared to vector up.
    /// </summary>
    float wallFloorBarrier = 60f;

    /// <summary>
    /// Gravity scale for making the player feel heavier or lighter.
    /// </summary>
    [SerializeField]
    float gravityScalar = 1f;

    // Sliding
    /// <summary>
    /// The slide boost the player gets when the slide on the ground. Stats sets this.
    /// </summary>
    float slideBoostForce = 5f;

    /// <summary>
    /// The boost force when the player slide in the air. Stats set this.
    /// </summary>
    float airBoostForce = 5f;

    /// <summary>
    /// The friction from the ground surface. Nerfs the slide boost.
    /// </summary>
    private float groundFriction = 5f;

    /// <summary>
    /// The friction whilst in the air.
    /// </summary>
    private float airFriction = 1f;

    /// <summary>
    /// Is the player grounded.
    /// </summary>
    bool grounded;

    /// <summary>
    /// Is the player performing a jump.
    /// </summary>
    bool isJumping;

    /// <summary>
    /// Is the player on a steep slope.
    /// </summary>
    bool isOnSteepSlope;

    /// <summary>
    /// Is the player on a slight slope.
    /// </summary>
    bool isOnSlightSlope;

    /// <summary>
    /// Is the player currently crouched.
    /// </summary>
    bool isCrouched = false;

    /// <summary>
    /// Is the player currently sprinting.
    /// </summary>
    bool isSprinting = false;



    /// <summary>
    /// Can the player slide boost.
    /// </summary>
    bool canSlideBoost = true;

    /// <summary>
    /// Has the player already performed a slide boost.
    /// </summary>
    bool appliedSlideBoost = false;

    /// <summary>
    /// Can the player air slide boost.
    /// </summary>
    bool canAirBoost = true;

    /// <summary>
    /// Has the player already applied the air slide boost.
    /// </summary>
    bool appliedAirBoost = false;


    /// <summary>
    /// The normal up vector for a perfectly level ground.
    /// </summary>
    Vector3 groundNormalAverage = Vector3.up;

    /// <summary>
    /// The attached capsule collider to adjust for crouching.
    /// </summary>
    CapsuleCollider col;

    /// <summary>
    /// The attached rigidbody.
    /// </summary>
    Rigidbody rb;


    /// <summary>
    /// The camera target to move the camera to.
    /// </summary>
    [SerializeField]
    Transform cameraTarget;

    /// <summary>
    /// The current rotation of the player. (Rotating this and not the rigid body.)
    /// </summary>
    [SerializeField]
    Transform orientation;

    /// <summary>
    /// The default mouse sensitivity.
    /// </summary>
    const float MOUSE_SENS_MULT = 0.01f;

    /// <summary>
    /// The default gamepad sensitivity.
    /// </summary>
    const float GAMEPAD_SENS_MULT = 10f;


    /// <summary>
    /// Look input vector.
    /// </summary>
    Vector2 lookDelta = Vector2.zero;

    /// <summary>
    /// The current camera vertical look.
    /// </summary>
    float camXRot = 0f;

    // Input
    Vector3 dir = Vector3.zero;
    bool wantToJump;
    bool wantToCrouch;
    bool wantToSprint;

    // Movement Binds.
    InputAction movementInput;
    InputAction jumpInput;
    InputAction crouchInput;
    InputAction lookInput;
    InputAction sprintInput;


    /// <summary>
    /// Debug to show the velocity on screen.
    /// </summary>
    private bool showVel = false;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        movementInput = InputSystem.actions.FindAction("Move");
        jumpInput = InputSystem.actions.FindAction("Jump");
        crouchInput = InputSystem.actions.FindAction("Crouch");
        lookInput = InputSystem.actions.FindAction("Look");
        sprintInput = InputSystem.actions.FindAction("Sprint");

        // TODO: Remove this. This should not be here and be moved to a dedicated script.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    void Update()
    {
        PollInput();
        if (disabledState == DisabledType.MovementOnly) { rb.linearVelocity = Vector3.zero; }
        if (disabledState == DisabledType.MouseOnly || disabledState == DisabledType.All) { return; }

        // col.material.dynamicFriction = 0f;

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
        if (disabledState == DisabledType.MovementOnly || disabledState == DisabledType.All) return;

        // Walk(dir, running ? runSpeed : groundSpeed, grAccel);
        // AirMove(dir, airSpeed, airAccel);
        CheckForGround();

        if (grounded && !isOnSteepSlope)
        {
            isJumping = false;
        }

        if (wantToSprint)
        {
            if (!isSprinting)
            {
                CrouchBoost(); // so lazy
                AirBoost();
            }
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }

        if (wantToCrouch)
        {

            col.height = Mathf.Max(1.0f, col.height - Time.deltaTime * 7f);
            // if (!isCrouched && isSprinting)
            // {
            //     CrouchBoost(); // so lazy
            //     AirBoost();
            // }

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
            Vector3 velocityToAdd = GroundedMovement(dir, isSprinting ? runSpeed : walkSpeed, grAccel);
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
                rb.AddForce(Vector3.up * (jumpUpSpeed - Mathf.Max(0f, rb.linearVelocity.y)), ForceMode.Impulse);
                isJumping = true;
            }
        }
        else if (grounded && isCrouched)
        {
            if (rb.linearVelocity.magnitude > walkSpeed)
            {
                Vector3 velToAdd = AirMovement(dir, walkSpeed, grAccel);
                rb.AddForce(velToAdd, ForceMode.VelocityChange);

                Vector3 friction = GetFrictionVector(walkSpeed, groundFriction);
                rb.AddForce(friction, ForceMode.VelocityChange);
            }
            else
            {
                Vector3 velocityToAdd = GroundedMovement(dir, walkSpeed, grAccel);
                velocityToAdd = Vector3.ProjectOnPlane(velocityToAdd, groundNormalAverage); // so we can walk on slanted surfaces.
                rb.AddForce(velocityToAdd, ForceMode.Acceleration);
            }

            // WHAT? duplicate jumping.
            if (wantToJump && !isJumping)
            {
                // rb.AddForce(groundNormalAverage * (jumpUpSpeed - Mathf.Max(0f, rb.linearVelocity.y)), ForceMode.Impulse);
                rb.AddForce(Vector3.up * (jumpUpSpeed - Mathf.Max(0f, rb.linearVelocity.y)), ForceMode.Impulse);
                isJumping = true;
            }
        }
        else
        {
            Vector3 velToAdd = AirMovement(dir, airSpeed, airAccel);
            rb.AddForce(velToAdd, ForceMode.VelocityChange);

            Vector3 friction = GetFrictionVector(runSpeed, airFriction);
            rb.AddForce(friction, ForceMode.VelocityChange);
            // if (isJumping) isJumping = false; // this is cursed.
        }


        if (IsGrounded())
        {
            // if (rb.SweepTest(transform.worldToLocalMatrix * dir.normalized, out RaycastHit hitInfo, 1f))
            StepHandle(dir.normalized);
        }
    }

    /// <summary>
    /// Set the local variables from the player stats.
    /// </summary>
    /// <param name="stats">The stats to read from.</param>
    public void UpdateVariablesWithStats(PlayerStats stats)
    {
        if (stats == null)
        {
            Debug.LogError("No player stats! Using default values!");
            stats = new PlayerStats();
            // return;
        }

        walkSpeed = stats.WalkSpeed;
        runSpeed = stats.GroundRunSpeedStat.GetCurrentValue();
        airSpeed = stats.AirSpeed;

        grAccel = stats.GroundRunSpeedStat.GetCurrentValue() * stats.GroundAccelerationPercentBase;
        airAccel = stats.AirSpeed * stats.AirAccelerationPercentBase;

        jumpUpSpeed = stats.JumpForce;
        // slideBoostForce = stats.SlideBoostForce;
        // airBoostForce = stats.AirBoostForce;

        slideBoostForce = stats.GroundRunSpeedStat.GetCurrentValue() * stats.SlideBoostPercentageStat.GetCurrentValue();
        airBoostForce = stats.AirSpeed * stats.AirBoostPercentageStat.GetCurrentValue();

        groundFriction = stats.GroundFriction;
        airFriction = stats.AirFriction;
    }


    /// <summary>
    /// The the half height of the current player's height.
    /// </summary>
    /// <returns></returns>
    private float GetHalfHeight()
    {
        return Mathf.Max(col.height / 2f, col.radius);
    }

    /// <summary>
    /// Updates the variables with the input states.
    /// </summary>
    private void PollInput()
    {
        Vector2 inputVector = movementInput.ReadValue<Vector2>();
        Vector3 inputInWorld = new Vector3(inputVector.x, 0, inputVector.y);

        dir = orientation.transform.TransformDirection(inputInWorld);

        wantToJump = jumpInput.IsPressed();

        wantToSprint = sprintInput.IsPressed();

        wantToCrouch = crouchInput.IsPressed();

        lookDelta = lookInput.ReadValue<Vector2>();
    }


    /// <summary>
    /// Checks if the player is stood on solid ground, if it is sloped and updates the variables.
    /// </summary>
    private void CheckForGround()
    {
        Collider[] results = Physics.OverlapSphere(transform.position - Vector3.down * GetHalfHeight(), col.radius - 0.05f, groundLayer); // TODO ground mask?

        bool didHit = Physics.Raycast(transform.position, -transform.up, out RaycastHit hitInfo, GetHalfHeight() + 0.1f, groundLayer);

        if (results.Length > 0 || didHit)
        {
            if (didHit)
            {
                float angle = Vector3.Angle(hitInfo.normal, Vector3.up);
                // print(angle);
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

    /// <summary>
    /// Is the target point close to the player's feet.
    /// </summary>
    /// <param name="point">The point to check.</param>
    /// <returns>True if within ground range.</returns>
    private bool WithinGroundRange(Vector3 point)
    {
        Vector3 feetPos = transform.position - Vector3.down * GetHalfHeight();

        float radius = col.radius - 0.01f;

        return Vector3.Distance(point, feetPos) < radius;

    }

    /// <summary>
    /// Get the current gravity vector to use in this current state.
    /// </summary>
    /// <returns></returns>
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

    }

    /// <summary>
    /// Handles stepping up steps and small ledges.
    /// </summary>
    /// <param name="moveDirectionThisFrame">The direction the movement is this frame.</param>
    private void StepHandle(Vector3 moveDirectionThisFrame)
    {
        moveDirectionThisFrame.y = 0;

        Vector3 pointAtFeet = transform.position + (Vector3.up * 0.05f) + (Vector3.down * GetHalfHeight());

        float stepHeight = 0.5f;

        float minStepAllowed = 0.1f;

        float minStepWithRadius = col.radius + minStepAllowed;

        int rayCount = 5; // dont go below 2

        float heightIncrement = stepHeight / (float)(rayCount - 1f);

        // print("H: " + heightIncrement);

        bool canStep = false;
        int iteration = 0;

        for (int i = 0; i < rayCount; i++)
        {
            bool rayRes = Physics.Raycast(pointAtFeet + (Vector3.up * (heightIncrement * i)), moveDirectionThisFrame.normalized, out RaycastHit hitInfo, minStepWithRadius, groundLayer, QueryTriggerInteraction.Ignore);
            Debug.DrawLine(pointAtFeet + (Vector3.up * (heightIncrement * i)), (pointAtFeet + (Vector3.up * (heightIncrement * i))) + (moveDirectionThisFrame.normalized * minStepWithRadius), Color.blue, 10f);

            if (rayRes)
            {
                // print(hitInfo.transform.gameObject.name);
                Debug.DrawLine(pointAtFeet + (Vector3.up * (heightIncrement * i)), hitInfo.point, Color.red, 10f);
            }

            if (i == 0 && !rayRes)
            {
                // print("cannot step on air");
                break; // we dont need to step.
            }
            else if (i == 0 && rayRes)
            {
                if (Vector3.Angle(hitInfo.normal, Vector3.up) < 80f || Vector3.Angle(hitInfo.normal, Vector3.up) > 100f)
                {
                    // print("failed angle check");
                    break;
                }
            }


            if (!rayRes)
            {
                // print("can step");
                canStep = true;
                iteration = i;
                break;
            }
        }

        if (!canStep)
        {
            // print("Cannot step up a wall");
            return;
        }

        // print("able to step");

        Vector3 upAmount = (Vector3.up * (heightIncrement * iteration));

        transform.position += upAmount;
    }

    /// <summary>
    /// Get the friction vector.
    /// </summary>
    /// <param name="maxSpeed">The max speed.</param>
    /// <param name="friction">The friction to apply.</param>
    /// <returns>The vector that will apply friction.</returns>
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

    /// <summary>
    /// Get the vector to apply to the rigidbody for the movement.
    /// </summary>
    /// <param name="wishDir">The desired direction to move in.</param>
    /// <param name="maxSpeed">The max speed.</param>
    /// <param name="acceleration">The acceleration.</param>
    /// <returns>The vector to apply this frame.</returns>
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

    /// <summary>
    /// Get the vector to apply to the rigidbody for the air movement.
    /// </summary>
    /// <param name="wishDir">The desired direction to move in.</param>
    /// <param name="maxSpeed">The max speed.</param>
    /// <param name="acceleration">The acceleration.</param>
    /// <returns></returns>
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


    /// <summary>
    /// Get the boost force vector.
    /// </summary>
    /// <param name="wishDir">The wish direction for the boost.</param>
    /// <param name="boostSpeed">The speed the boost will be.</param>
    /// <returns>Vector to apply to the rigidbody.</returns>
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

    //TODO: remove this.
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

    /// <summary>
    /// Crouch boost.
    /// </summary>
    void CrouchBoost()
    {
        if (!grounded || !canSlideBoost || appliedSlideBoost) return;

        appliedSlideBoost = true;

        Vector3 boostDir = dir;

        rb.AddForce(GetBoostVector(boostDir, slideBoostForce), ForceMode.Impulse);

        // if (canSlideBoost) StartCoroutine(HandleCrouchBoostCoolDown());
    }

    /// <summary>
    /// Air boost.
    /// </summary>
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

    /// <summary>
    /// Is the player currently on the ground.
    /// </summary>
    /// <returns></returns>
    public bool IsGrounded()
    {
        return grounded;
    }

    /// <summary>
    /// Set the current disabled state of the movement.
    /// </summary>
    /// <param name="disabledState">The desired disabled state.</param>
    public void SetDisabledState(DisabledType disabledState = DisabledType.None)
    {
        this.disabledState = disabledState;
    }

    /// <summary>
    /// Get the current disabled state of the player movement.
    /// </summary>
    /// <returns>The current disable state.</returns>
    public DisabledType GetDisabledState()
    {
        return disabledState;
    }
}
