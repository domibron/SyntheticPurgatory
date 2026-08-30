using UnityEngine;
using UnityEngine.InputSystem;

// [System.Flags]
// public enum DaysOfWeek
// {
//     None = 0,
//     Sunday = 1 << 0,
//     Monday = 1 << 1,
//     Tuesday = 1 << 2,
//     Wednesday = 1 << 3,
//     Thursday = 1 << 4,
//     Friday = 1 << 5,
//     Saturday = 1 << 6,

//     Weekdays = Monday | Tuesday | Wednesday | Thursday | Friday,
//     Weekend = Saturday | Sunday,
// }

/// <summary>
/// The player movement controller.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    // =============
    //   VARIABLES
    // =============


    #region Disabling
    /// <summary>
    /// The player movement disable type.
    /// </summary>
    public enum DisabledType : byte // Can use byte to reduce the size if we use a few values. (ideally used for structs)
    {
        // Use byte to reduce the size of the enum since we only use a handful of values.
        // Ideally use this for structs and data packing to optimise it for memory.

        None = 0b_0000_0000, // 1 << 0 shift zero to the left.
        MovementOnly = 0b_0000_0001, // 1 << 1 shift one to the left.
        LookOnly = 0b_0000_0010, // 1 << 2 shift two to the left.

        // Combine both bit values. 00101 | 01100 = 01101.
        All = MovementOnly | LookOnly,

        // You can also use ^ since its a logical or. 00101 ^ 01100 = 01001.
    }

    /// <summary>
    /// The current disable state of the player movement.
    /// </summary>
    public DisabledType CurrentDisabledState { get; set; } = DisabledType.None;

    #endregion



    #region Non changing

    /// <summary>
    /// ALl the layers that the player can run and jump on.
    /// </summary>
    [SerializeField]
    LayerMask m_groundLayer = Physics.AllLayers;

    /// <summary>
    /// Gravity scale for making the player feel heavier or lighter.
    /// </summary>
    [SerializeField]
    float m_gravityScalar = 1f;

    /// <summary>
    /// The attached capsule collider to adjust for crouching.
    /// </summary>
    CapsuleCollider m_col;

    /// <summary>
    /// The attached rigidbody.
    /// </summary>
    Rigidbody m_rb;

    /// <summary>
    /// The camera target to move the camera to.
    /// </summary>
    [SerializeField]
    Transform m_cameraTarget;

    /// <summary>
    /// The current rotation of the player. (Rotating this and not the rigid body.)
    /// </summary>
    [SerializeField]
    Transform m_orientation;

    const float k_slopeToSteepSlopeThreshold = 40;

    const float k_floorToSlopeThreshold = 1;

    const float k_minAccelRate = 0.01f;

    #endregion



    #region Input related

    //TODO: remove from imp. default input should be set correctly.
    /// <summary>
    /// The default mouse sensitivity.
    /// </summary>
    const float k_mouseSensitivityMult = 0.01f;

    /// <summary>
    /// The default gamepad sensitivity.
    /// </summary>
    const float k_gamepadSensitivityMult = 10f;

    /// <summary>
    /// Look input vector.
    /// </summary>
    Vector2 m_lookDelta = Vector2.zero;

    /// <summary>
    /// The current camera vertical look.
    /// </summary>
    float m_camXRot = 0f;

    // Input
    Vector3 m_inputWishDirWorld = Vector3.zero;
    bool m_isJumpHeld;
    bool m_isCrouchHeld;
    bool m_isSprintHeld;

    // Movement Binds.
    InputAction m_movementInput;
    InputAction m_jumpInput;
    InputAction m_crouchInput;
    InputAction m_lookInput;
    InputAction m_sprintInput;


    #endregion



    #region States

    /// <summary>
    /// The normal up vector for a perfectly level ground.
    /// </summary>
    Vector3 m_groundNormalAverage = Vector3.up;

    /// <summary>
    /// Is the player grounded.
    /// </summary>
    public bool IsGrounded { get; private set; }


    private enum SlopeState : byte
    {
        FlatGround,
        SlightSlope,
        SteepSlope,
    }

    SlopeState m_currentSlopeState = SlopeState.FlatGround;

    // Was going to use a timer to reset the ground detection to prevent the player from 
    // being stuck in a floating / sliding state.
    // const float k_lastKnowDuration = 0.1f;

    // float m_currentLastKnown = 0;

    /// <summary>
    /// Is the player character currently crouched.
    /// </summary>
    bool m_isCrouched = false;

    /// <summary>
    /// Is the player character currently sprinting.
    /// </summary>
    bool m_isSprinting = false;


    bool m_isJumping = false;


    const float k_waitBeforeEnableJumpReset = 0.4f;

    float m_currentWaitUntilJumpRestAllowed = 0f;


    #endregion



    #region IKD what to do with atm


    [SerializeField]
    float m_maxSpeed = 10;

    [SerializeField]
    float m_accelRate = 0.1f;


    #endregion




    // =============
    //   FUNCTIONS
    // =============

    #region Monobehaviours


    void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
        m_col = GetComponent<CapsuleCollider>();

        m_movementInput = InputSystem.actions.FindAction("Move");
        m_jumpInput = InputSystem.actions.FindAction("Jump");
        m_crouchInput = InputSystem.actions.FindAction("Crouch");
        m_lookInput = InputSystem.actions.FindAction("Look");
        m_sprintInput = InputSystem.actions.FindAction("Sprint");

        // TODO: Remove this. This should not be here and be moved to a dedicated script.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        PollInput();
        HandleCameraMovement();

        if (m_currentWaitUntilJumpRestAllowed > 0)
        {
            m_currentWaitUntilJumpRestAllowed -= Time.deltaTime;
        }


    }

    void FixedUpdate()
    {
        if (CurrentDisabledState == DisabledType.MovementOnly) { m_rb.linearVelocity = Vector3.zero; } // This seems like it can be abused.
        if (CurrentDisabledState != DisabledType.None) return;

        IsGrounded = CheckIsGrounded();

        Vector3 groundNormalSample = SampleGroundNormal();
        if (IsGrounded && groundNormalSample != Vector3.zero)
        {
            m_groundNormalAverage = groundNormalSample;
            UpdateSlopeState(m_groundNormalAverage);
        }


        ApplyGravity();

        ResetJumpWhenGrounded();

        Movement();

        HandleStepping();
    }

    void OnGUI()
    {
        GUILayout.Label($"<color=blue><size={Screen.height / 20}>" + m_rb.linearVelocity.magnitude.ToString("F2"));
    }


    #endregion



    #region Camera

    private void HandleCameraMovement()
    {
        if (CurrentDisabledState == DisabledType.LookOnly || CurrentDisabledState == DisabledType.All) { return; }

        bool useMouseLook = true;
        bool invertYLook = false;
        float xSense = 7f * k_mouseSensitivityMult;
        float ySense = 7f * k_mouseSensitivityMult;

        if (InputManager.Instance != null)
        {
            if (InputManager.Instance.GetCurrentInputDevice() == InputManager.InputDeviceType.Gamepad)
                useMouseLook = false;

            if (useMouseLook)
            {
                invertYLook = SettingsMenu.GetMouseInvertY();

                xSense = SettingsMenu.GetMouseXSens() * k_mouseSensitivityMult;
                ySense = SettingsMenu.GetMouseYSens() * k_mouseSensitivityMult;
            }
            else
            {
                invertYLook = SettingsMenu.GetGamepadInvertY();

                xSense = SettingsMenu.GetGamepadXSens() * k_gamepadSensitivityMult;
                ySense = SettingsMenu.GetGamepadYSens() * k_gamepadSensitivityMult;
            }
        }



        // print(ySense + " " + xSense);


        if (invertYLook)
            m_camXRot += m_lookDelta.y * xSense * (useMouseLook ? 1f : Time.deltaTime);
        else
            m_camXRot -= m_lookDelta.y * xSense * (useMouseLook ? 1f : Time.deltaTime);

        m_camXRot = Mathf.Clamp(m_camXRot, -80, 80);

        m_cameraTarget.localRotation = Quaternion.Euler(m_camXRot, 0, 0);
        m_orientation.Rotate(0, m_lookDelta.x * ySense * (useMouseLook ? 1f : Time.deltaTime), 0);


        Vector3 camPos = m_cameraTarget.localPosition;
        camPos.y = GetHalfHeight() - 0.15f;
        m_cameraTarget.localPosition = camPos;
    }

    #endregion



    #region Movement

    private void Movement()
    {
        // ? Isnt this just input handling?

        if (IsGrounded && !m_isJumping)
        {
            // Ground movement.
            if (m_currentSlopeState == SlopeState.FlatGround)
            {
                // Normal movement.
                m_rb.AddForce(GetImmediateChangeVel(Utils.GetLevelVectorY(m_rb.linearVelocity), m_inputWishDirWorld, m_accelRate, m_maxSpeed), ForceMode.Acceleration);
            }
            else if (m_currentSlopeState == SlopeState.SlightSlope)
            {
                // Counter gravity.
                m_rb.AddForce(-Vector3.ProjectOnPlane(GetGravityVector(), m_groundNormalAverage), ForceMode.Acceleration);

                // Movement on slight slope.
                Vector3 normalMovement = GetImmediateChangeVel(m_rb.linearVelocity, m_inputWishDirWorld, m_accelRate, m_maxSpeed);
                m_rb.AddForce(Vector3.ProjectOnPlane(normalMovement, m_groundNormalAverage).normalized * normalMovement.magnitude, ForceMode.Acceleration);
            }
            else
            {
                // Slide along slope.
                Vector3 gravVec = GetGravityVector();
                m_rb.AddForce(Vector3.ProjectOnPlane(gravVec, m_groundNormalAverage).normalized * gravVec.magnitude, ForceMode.Acceleration);
            }
        }
        else
        {
            // Air movement.

        }


        // Jumping
        if (m_isJumpHeld && CanPlayerJump())
        {
            m_rb.AddForce(GetJumpVector(m_rb.linearVelocity, 10, GetGravityVector()), ForceMode.VelocityChange);


            m_isJumping = true;
            m_currentWaitUntilJumpRestAllowed = k_waitBeforeEnableJumpReset;
        }
    }


    /// <summary>
    /// Get a vector velocity to apply from the desired parameters.
    /// </summary>
    /// <param name="currentVel">The current velocity of the player.</param>
    /// <param name="wishDir">The wish direction for the player to go.</param>
    /// <param name="accelRate">How fast to accelerate the player towards the new direction.</param>
    /// <param name="maxSpeed">The maximum / target speed for the player to reach.</param>
    /// <returns>The resulting velocity to apply onto the player.</returns>
    private Vector3 GetImmediateChangeVel(Vector3 currentVel, Vector3 wishDir, float accelRate, float maxSpeed)
    {
        wishDir.Normalize();

        accelRate = Mathf.Max(k_minAccelRate, accelRate);

        Vector3 neededChange = (wishDir * maxSpeed) - currentVel;

        float calculatedAccel = maxSpeed / accelRate;

        // Stops over speed.
        if (currentVel.magnitude > maxSpeed) calculatedAccel *= currentVel.magnitude / maxSpeed;

        // reduces the overall needed accel to reach target speed.
        if (neededChange.magnitude < calculatedAccel * 0.05f)
        {
            calculatedAccel *= neededChange.magnitude / (calculatedAccel * 0.05f);
        }


        return neededChange.normalized * calculatedAccel;
    }


    #endregion

    #region Jumping

    private Vector3 GetJumpVector(Vector3 currentVel, float jumpForce, Vector3 gravityVector)
    {
        // Player jumps one slightly over the ground, thus causing a "jump" but the player does not jump and is forced to wait the jump check cooldown.
        return new Vector3(0, -Mathf.Min(currentVel.y, 0) + jumpForce + (gravityVector.y * Time.fixedDeltaTime), 0);
    }


    /// <summary>
    /// Reset's the jumping parameters to allow the player to jump again when touching valid ground.
    /// </summary>
    private void ResetJumpWhenGrounded()
    {
        if (IsGrounded && m_currentSlopeState != SlopeState.SteepSlope && m_currentWaitUntilJumpRestAllowed <= 0)
        {
            m_isJumping = false;
        }
        else if (m_rb.linearVelocity.y < 0 && m_currentWaitUntilJumpRestAllowed <= 0)
        {
            m_isJumping = false;
        }
    }

    private bool CanPlayerJump()
    {
        if (IsGrounded && !m_isJumping && m_currentSlopeState != SlopeState.SteepSlope)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    #endregion



    #region Ground and Gravity


    /// <summary>
    /// Applies gravity to the rigidbody.
    /// </summary>
    private void ApplyGravity()
    {
        m_rb.AddForce(GetGravityVector(), ForceMode.Acceleration);
    }

    /// <summary>
    /// Does a sphere check below the player to see if they are standing on valid ground.
    /// </summary>
    /// <returns>True if there is ground below the player.</returns>
    private bool CheckIsGrounded()
    {
        float rad = GetNearMaxRadius();
        return Physics.CheckSphere(GetWorldFeetPos() + (Vector3.down * rad / 2f), rad, m_groundLayer);
    }

    /// <summary>
    /// Samples the surface below the player and returns the average of the surfaces combined normal.
    /// </summary>
    /// <returns>The average normal from all surfaces.</returns>
    private Vector3 SampleGroundNormal()
    {
        // ? Possible parameters to be moved.
        const float k_range = 0.2f;
        const int k_groundAverageSampleSize = 10;

        // Storage for all hits from sampling the shere cast. Can contain null / invalid data.
        RaycastHit[] hitsBuffer = new RaycastHit[k_groundAverageSampleSize];


        // Amount of all valid hits in the buffer.
        int count = Physics.SphereCastNonAlloc(GetWorldFeetPos() + (Vector3.up * (m_col.radius + 0.01f)), GetNearMaxRadius(), Vector3.down, hitsBuffer, k_range, m_groundLayer, QueryTriggerInteraction.Ignore);

        // Does the magic math for the average for the ground.
        if (count > 0)
        {
            Vector3 sample = Vector3.zero;

            for (int i = 0; i < count; i++)
            {
                sample += hitsBuffer[i].normal;
            }

            return sample / count;
        }

        // Return a null value since there is no surface normals we can get.
        return Vector3.zero;
    }


    private void UpdateSlopeState(Vector3 groundNormalAvg)
    {
        float angle = Vector3.Angle(groundNormalAvg.normalized, Vector3.up);

        if (angle > k_slopeToSteepSlopeThreshold)
        {
            m_currentSlopeState = SlopeState.SteepSlope;
        }
        else if (angle > k_floorToSlopeThreshold)
        {
            m_currentSlopeState = SlopeState.SlightSlope;
        }
        else
        {
            m_currentSlopeState = SlopeState.FlatGround;
        }
    }


    #endregion



    #region Stepping
    private void HandleStepping()
    {
        if (IsGrounded)
        {
            // if (rb.SweepTest(transform.worldToLocalMatrix * dir.normalized, out RaycastHit hitInfo, 1f))
            StepHandle(m_inputWishDirWorld.normalized);
        }
    }

    // TODO: Add slope support?
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

        float minStepWithRadius = m_col.radius + minStepAllowed;

        int rayCount = 5; // dont go below 2

        float heightIncrement = stepHeight / (float)(rayCount - 1f);

        // print("H: " + heightIncrement);

        bool canStep = false;
        int iteration = 0;

        for (int i = 0; i < rayCount; i++)
        {
            bool rayRes = Physics.Raycast(pointAtFeet + (Vector3.up * (heightIncrement * i)), moveDirectionThisFrame.normalized, out RaycastHit hitInfo, minStepWithRadius, m_groundLayer, QueryTriggerInteraction.Ignore);
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

        transform.position += Vector3.up * (heightIncrement * iteration);
    }

    #endregion



    #region Utility

    /// <summary>
    /// The the half height of the current player's height.
    /// </summary>
    /// <returns></returns>
    private float GetHalfHeight()
    {
        return Mathf.Max(m_col.height, m_col.radius) / 2f;
    }

    private Vector3 GetWorldFeetPos()
    {
        return transform.position - (transform.up * GetHalfHeight());
    }

    /// <summary>
    /// Get the current gravity vector to use in this current state.
    /// </summary>
    /// <returns></returns>
    Vector3 GetGravityVector()
    {
        return Physics.gravity * m_gravityScalar;
    }

    float GetNearMaxRadius()
    {
        return m_col.radius - (m_col.radius * 0.2f);
    }

    #endregion



    #region Input

    /// <summary>
    /// Updates the variables with the input states.
    /// </summary>
    private void PollInput()
    {
        Vector2 inputVector = m_movementInput.ReadValue<Vector2>();
        Vector3 inputInWorld = new Vector3(inputVector.x, 0, inputVector.y);

        m_inputWishDirWorld = m_orientation.transform.TransformDirection(inputInWorld);

        m_isJumpHeld = m_jumpInput.IsPressed();

        m_isSprintHeld = m_sprintInput.IsPressed();

        m_isCrouchHeld = m_crouchInput.IsPressed();

        m_lookDelta = m_lookInput.ReadValue<Vector2>();
    }

    #endregion



    #region Stat setting

    /// <summary>
    /// Set the local variables from the player stats.
    /// </summary>
    /// <param name="stats">The stats to read from.</param>
    public void UpdateVariablesWithStats(PlayerStats stats)
    {
        // if (stats == null)
        // {
        //     Debug.LogError("No player stats! Using default values!");
        //     stats = new PlayerStats();
        //     // return;
        // }

        // walkSpeed = stats.WalkSpeed;
        // runSpeed = stats.GroundRunSpeedStat.GetCurrentValue();
        // airSpeed = stats.AirSpeed;

        // grAccel = stats.GroundRunSpeedStat.GetCurrentValue() * stats.GroundAccelerationPercentBase;
        // airAccel = stats.AirSpeed * stats.AirAccelerationPercentBase;

        // jumpUpSpeed = stats.JumpForce;
        // // slideBoostForce = stats.SlideBoostForce;
        // // airBoostForce = stats.AirBoostForce;

        // slideBoostForce = stats.GroundRunSpeedStat.GetCurrentValue() * stats.SlideBoostPercentageStat.GetCurrentValue();
        // airBoostForce = stats.AirSpeed * stats.AirBoostPercentageStat.GetCurrentValue();

        // groundFriction = stats.GroundFriction;
        // airFriction = stats.AirFriction;

        Debug.LogWarning("Player stats have not been implemented onto the player!");
    }

    #endregion


}
