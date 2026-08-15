using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// The player movement controller.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    #region Disabling
    /// <summary>
    /// The player movement disable type.
    /// </summary>
    public enum DisabledType : byte // Can use byte to reduce the size if we use a few values. (ideally used for structs)
    {
        None,
        MovementOnly,
        MouseOnly,
        All,
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

    const float k_slopeToSteepSlopeThreshold = 50;

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

    /// <summary>
    /// Is the player character currently crouched.
    /// </summary>
    bool m_isCrouched = false;

    /// <summary>
    /// Is the player character currently sprinting.
    /// </summary>
    bool m_isSprinting = false;


    bool m_isJumping = false;

    #endregion

    [SerializeField]
    float maxSpeed = 10;

    [SerializeField]
    float accel = 10;


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
    }

    void FixedUpdate()
    {
        if (CurrentDisabledState == DisabledType.MovementOnly) { m_rb.linearVelocity = Vector3.zero; } // This seems like it can be abused.
        if (CurrentDisabledState == DisabledType.MovementOnly || CurrentDisabledState == DisabledType.All) return;

        // Walk(dir, running ? runSpeed : groundSpeed, grAccel);
        // AirMove(dir, airSpeed, airAccel);

        IsGrounded = CheckIsGrounded();

        m_groundNormalAverage = UpdateGroundNormalAverage();
        UpdateSlopeState(m_groundNormalAverage);

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
        if (CurrentDisabledState == DisabledType.MouseOnly || CurrentDisabledState == DisabledType.All) { return; }

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


    private void Movement()
    {
        if (IsGrounded)
        {
            // Ground movement.
            if (m_currentSlopeState == SlopeState.FlatGround)
            {
                // Normal movement.
                m_rb.AddForce(GetImmediateChangeVel(Utils.GetLevelVectorY(m_rb.linearVelocity), m_inputWishDirWorld, accel, maxSpeed), ForceMode.Acceleration);
            }
            else if (m_currentSlopeState == SlopeState.SteepSlope)
            {
                // Counter gravity.
            }
            else
            {
                // Slide along slope.
            }
        }
        else
        {
            // Air movement.
        }
    }

    private Vector3 GetImmediateChangeVel(Vector3 vel, Vector3 wish, float accelRate, float maxSpeed)
    {
        wish.Normalize();

        accelRate = Mathf.Max(k_minAccelRate, accelRate);

        Vector3 neededChange = (wish * maxSpeed) - vel;

        float calculatedAccel = maxSpeed / accelRate;

        // Stops over speed.        
        if (vel.magnitude > maxSpeed) calculatedAccel *= vel.magnitude / maxSpeed;

        // reduces the overall needed accel to reach target speed.
        if (neededChange.magnitude < calculatedAccel * 0.05f)
        {
            calculatedAccel *= neededChange.magnitude / (calculatedAccel * 0.05f);
        }


        return neededChange.normalized * calculatedAccel;
    }


    private void ResetJumpWhenGrounded()
    {
        if (IsGrounded && m_currentSlopeState != SlopeState.SteepSlope)
        {
            m_isJumping = false;
        }
    }



    private void ApplyGravity()
    {
        m_rb.AddForce(GetGravityVector(), ForceMode.Acceleration);
    }

    private bool CheckIsGrounded()
    {
        return Physics.CheckSphere(transform.position + (Vector3.down * GetHalfHeight()), m_col.radius - (m_col.radius * 0.2f), m_groundLayer);
    }

    private Vector3 UpdateGroundNormalAverage()
    {
        const float RANGE = 0.2f;
        const int MAX_CHECKS = 9;

        Vector3 collectedAverage = Vector3.zero;
        Vector3 middleSample = Vector3.zero;

        Vector3 checkPos = GetWorldFeetPos();
        Vector3 down = -transform.up;

        RaycastHit hit;

        for (int i = 0; i < MAX_CHECKS; i++)
        {
            // Check in the center of the player.
            if (i == 0)
            {
                if (Physics.Raycast(checkPos, down, out hit, RANGE, m_groundLayer, QueryTriggerInteraction.Ignore))
                {
                    middleSample = hit.normal;

                    if (Vector3.Angle(middleSample, Vector3.up) > k_floorToSlopeThreshold)
                    {
                        return middleSample; // ! Returns out of function early.
                    }
                }
                else
                {
                    middleSample = Vector3.up;
                }

                continue;
            }

            // Check in the 8 directions around the player.
            checkPos += transform.forward * (m_col.radius - (m_col.radius * 0.2f));

            // Rotate the vector (in our case with a max of 9, remove the center, so 8. We check ever 45 deg.)
            checkPos = Quaternion.AngleAxis(360 / (MAX_CHECKS - 1), -down) * checkPos;

            if (Physics.Raycast(checkPos, down, out hit, RANGE, m_groundLayer, QueryTriggerInteraction.Ignore))
            {
                collectedAverage += hit.normal;
            }
            else
            {
                collectedAverage += Vector3.up;
            }
        }

        return collectedAverage / (MAX_CHECKS - 1);
        // m_groundNormalAverage = collectedAverage / MAX_CHECKS;
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
        return Mathf.Max(m_col.height / 2f, m_col.radius);
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
