using UnityEngine;

// TODO: finish writing comments in this script.

/// <summary>
/// Tower crane script to rotate and move pieces of the crane to align the boom to a target point. Like a IK script but for a tower crane.
/// </summary>
[ExecuteInEditMode]
public class Crane : MonoBehaviour
{
    /// <summary>
    /// The current point being tracked.
    /// </summary>
    [Header("Crane Target"), SerializeField]
    private Transform currentTargetPoint;

    /// <summary>
    /// The default transform target for the crane to move to. You can move this point or override the current target point to move the crane.
    /// </summary>
    [SerializeField]
    private Transform defaultTargetPoint; // is this default because this is overridden in the code.

    /// <summary>
    /// The crane's current target point / "boom IK target position". The crane will align to this point. This has all the lerping applied.
    /// </summary>
    [SerializeField]
    private Transform followPoint;



    [Header("Crane Lerping"), SerializeField]
    bool enableLerping = true;

    [SerializeField]
    float rate = 1f;

    [Space, SerializeField]
    bool lerpRotation = true;

    [SerializeField]
    float rotationRateDegreesPerSecond = 10f;



    [Header("Crane Top"), SerializeField]
    private Transform craneTop;

    [SerializeField]
    private float rotationOffset = 180f;




    [Header("Crane Arm"), SerializeField]
    bool allowExtension = true;

    [SerializeField]
    float extensionStartDistance = 10f; // 10m from end of inner arm

    [SerializeField]
    private Transform extendableArm;

    [SerializeField]
    private Transform extendableArmMin;

    [SerializeField]
    private Transform extendableArmMax;



    [Header("Crane Carriage"), SerializeField]
    private Transform carriage;

    [SerializeField]
    private Transform carriageMin;

    [SerializeField]
    private Transform carriageMax;



    [Header("Crane Boom (The Hook)"), SerializeField]
    private Transform boom;

    [SerializeField]
    private Transform boomMin; // Top of boom.

    [SerializeField]
    private Transform boomCable;


    [Space, SerializeField]
    private bool boomHasMax = false;

    [SerializeField]
    private float boomMaxDropDistance = 50f;


    [Space, SerializeField]
    private bool isOverridingDropAmountBoom = false;

    [SerializeField]
    private float boomDropOverrideAmount = 0;


    [Space, SerializeField]
    private bool isOverridingBoomOffset = false;

    [SerializeField]
    private float boomHeightOffsetAmount = 0; // how much to rise the boom to account for offset.


    private bool isUpdatingCrane = true;

    private Vector3 actualTarget;

    // Update is called once per frame
    void Update()
    {
        if (isUpdatingCrane)
        {
            // print($"{GetDistanceFromTargetWithOffsets()}\n{GetDistanceFromRealTarget()}\n{GetYDistance()}\n{GetXZDistance()}");
            UpdateCrane();
        }
    }

    #region Crane Updating Functions
    private void UpdateCrane()
    {
        FixCurrentTargetIfNull();

        Vector3 targetPos = LerpFollowPointAndGetTarget();

        UpdateCraneRotation(targetPos);

        // carriage needs to be updated before arm so arm can adjust to the carriage.
        GetCarriageBounds(out Vector3 carriageMaxWhenRetracted, out Vector3 armRetractedPoint, out Vector3 carriageFullMax);

        UpdateCarriage(targetPos, carriageFullMax);

        UpdateCraneExtendableArm(carriageMaxWhenRetracted, armRetractedPoint, carriageFullMax);

        UpdateCraneBoom();
    }

    private void FixCurrentTargetIfNull()
    {
        if (currentTargetPoint == null) // if default target point is null, does this break?
        {
            Debug.LogWarning($"{nameof(currentTargetPoint)} is null. Fixing.");
            currentTargetPoint = defaultTargetPoint;
        }
    }

    private Vector3 LerpFollowPointAndGetTarget()
    {
        Vector3 currentTargetPos = currentTargetPoint.position;

        if (isOverridingDropAmountBoom) // might need to control speed or be able to detect if the boom is at target pos.
        {
            float newYPos = boomMin.position.y - boomDropOverrideAmount;

            currentTargetPos.y = newYPos;
        }

        actualTarget = currentTargetPos; // saving the output for some checks. because we modify the current target but we need to save the value somewhere.

        if (enableLerping)
        {

            Vector3 moveAmount = (currentTargetPos - followPoint.position).normalized * rate * Time.deltaTime;

            if (Vector3.Distance(followPoint.position, currentTargetPos) < moveAmount.magnitude)
            {
                followPoint.position = currentTargetPos;
            }
            else
            {
                followPoint.position += moveAmount;
            }
        }
        else
        {
            followPoint.position = currentTargetPos;
        }



        // convert to local.
        Vector3 targetPos = followPoint.position - transform.position;
        return targetPos;
    }

    private void UpdateCraneRotation(Vector3 targetPos)
    {
        // top rotation.
        float angle = Mathf.Atan2(targetPos.z, targetPos.x);

        if (lerpRotation)
        {
            craneTop.rotation = Quaternion.RotateTowards(craneTop.rotation, Quaternion.Euler(0, -(Mathf.Rad2Deg * angle) + rotationOffset, 0), rotationRateDegreesPerSecond * Time.deltaTime);
        }
        else
        {
            craneTop.rotation = Quaternion.Euler(0, -(Mathf.Rad2Deg * angle) + rotationOffset, 0);
        }
    }

    private void GetCarriageBounds(out Vector3 carriageMaxWhenRetracted, out Vector3 armRetractedPoint, out Vector3 carriageFullMax)
    {
        // Get bounds for the crane arms.
        extendableArm.localPosition = extendableArmMin.localPosition;
        carriageMaxWhenRetracted = extendableArm.localPosition + carriageMax.localPosition;
        armRetractedPoint = extendableArm.localPosition;
        if (allowExtension)
        {
            extendableArm.localPosition = extendableArmMax.localPosition;
        }
        else
        {
            extendableArm.localPosition = extendableArmMin.localPosition;
        }
        carriageFullMax = extendableArm.localPosition + carriageMax.localPosition;
    }

    private void UpdateCarriage(Vector3 targetPos, Vector3 carriageFullMax)
    {
        // TODO: make this lerp and not cause the carriage to go in and out when the crane is rotating.
        // We know the dist from the crane, we should not use the current pos of the crane but the final pos of the crane at that point.
        // But we need to do this after we have rotated. We should bring in the boom / carriage then rotate then extend.

        // set the carriage before the arm, we can adjust arm to the carriage.
        float targetDistanceFromCrane = Vector3.Distance(Vector3.zero, GetVectorWithLevelY(targetPos, 0));

        float carriageMinDist = Vector3.Distance(Vector3.zero, GetVectorWithLevelY(carriageMin.localPosition, 0));
        float carriageMaxDist = Vector3.Distance(Vector3.zero, GetVectorWithLevelY(carriageFullMax, 0));

        if (targetDistanceFromCrane <= carriageMinDist)
        {
            carriage.localPosition = carriageMin.localPosition;
        }
        else if (targetDistanceFromCrane > carriageMinDist - extensionStartDistance && targetDistanceFromCrane < carriageMaxDist)
        {
            carriage.localPosition = carriageMin.localPosition + (-Vector3.right * (targetDistanceFromCrane - carriageMinDist));
        }
        else if (targetDistanceFromCrane >= carriageMaxDist)
        {
            carriage.localPosition = carriageMin.localPosition + (-Vector3.right * (carriageMaxDist - carriageMinDist));
        }
    }

    private void UpdateCraneExtendableArm(Vector3 carriageMaxWhenRetracted, Vector3 armRetractedPoint, Vector3 carriageFullMax)
    {
        // Crane arms.
        if (allowExtension)
        {
            // Extendable Arm
            // is the carriage less than the middle point. I did not abs the values hence the > and not the <.
            // We are presuming -X is the forward direction (+Z is default, I am aware, just made crane wrong and I REFUSE TO FIX IT).
            if (carriage.localPosition.x >= carriageMaxWhenRetracted.x + extensionStartDistance)
            {
                extendableArm.localPosition = extendableArmMin.localPosition;
            }
            else if (carriage.localPosition.x < carriageMaxWhenRetracted.x + extensionStartDistance && carriage.localPosition.x > carriageFullMax.x)
            {
                // So much is happening, so we take the distance the carriage is from the max point when retracted, we account for a offset (armRetractedPoint),
                // the we add the extension offset.
                /// D==[]====x---o-
                ///    |     ^ retracted max
                ///    |         ^ carriage point
                /// calculate offset of o from x.
                /// add extension offset.
                float howMuchToExtendBy = carriage.localPosition.x - (carriageMaxWhenRetracted.x - armRetractedPoint.x) - extensionStartDistance;
                // arm will extend when the carriage meets the retracted max - offset (we + because we are working in negatives).


                // check to see if we exceed the bounds after moving.
                if (howMuchToExtendBy > extendableArmMin.localPosition.x) howMuchToExtendBy = extendableArmMin.localPosition.x;
                else if (howMuchToExtendBy < extendableArmMax.localPosition.x) howMuchToExtendBy = extendableArmMax.localPosition.x;

                extendableArm.localPosition = new Vector3(howMuchToExtendBy, extendableArm.localPosition.y, extendableArm.localPosition.z);
            }
            else if (carriage.localPosition.x <= carriageFullMax.x)
            {
                extendableArm.localPosition = extendableArmMax.localPosition;
            }
        }
        else
        {
            extendableArm.localPosition = extendableArmMin.localPosition;
        }
    }

    private void UpdateCraneBoom()
    {
        // Boom (the claw)
        float targetY = followPoint.position.y;

        // get the drop amount with offset if enabled.
        float boomDropAmount = boomMin.position.y - (targetY + (isOverridingBoomOffset ? boomHeightOffsetAmount : 0f));

        if (boomDropAmount > 0)
        {

            if (boomHasMax && boomDropAmount > boomMaxDropDistance)
            {
                boom.position = new Vector3(boom.position.x, boomMin.position.y - boomMaxDropDistance, boom.position.z);
            }
            else
            {
                boom.position = new Vector3(boom.position.x, boomMin.position.y - boomDropAmount, boom.position.z);
            }
        }
        else if (boomDropAmount <= 0)
        {

            boom.position = boomMin.position;
        }

        boomCable.localPosition = (boom.localPosition + Vector3.up) / 2f; // we can ignore the carriage pos since its 0,0,0 and we don't need to calc that.
        boomCable.localScale = new Vector3(boomCable.localScale.x, (boom.localPosition.y + 1f) / 2f, boomCable.localScale.z); // real fucking lazy but it does the job.
    }

    #endregion

    /// <summary>
    /// Simple function to create a new vector with the provided vector but with the new y.
    /// </summary>
    /// <param name="target">The vector to create new vector from.</param>
    /// <param name="y">The new y value for the vector.</param>
    /// <returns>The vector with the given y value</returns>
    private Vector3 GetVectorWithLevelY(Vector3 target, float y) // TODO: move into a util class.
    {
        return new Vector3(target.x, y, target.z);
    }


    #region Public Control Functions

    public void SetTargetPoint(Vector3 position)
    {
        currentTargetPoint = defaultTargetPoint;
        defaultTargetPoint.position = position;
    }

    public void SetTargetPoint(Transform transform)
    {
        if (transform == null)
        {
            currentTargetPoint = defaultTargetPoint;
            return;
        }

        currentTargetPoint = transform;
    }

    public Transform GetHookTransform()
    {
        return boom;
    }

    public void OverrideBoomDropDist(bool isOverriding = true, float boomDropAmount = 0)
    {
        isOverridingDropAmountBoom = isOverriding;
        boomDropOverrideAmount = boomDropAmount;
    }

    public void OverrideBoomOffset(bool isOverriding = true, float boomOffsetAmount = 0)
    {
        if (boomOffsetAmount == 0)
        {
            isOverridingBoomOffset = false;
            return;
        }

        isOverridingBoomOffset = isOverriding;
        boomHeightOffsetAmount = boomOffsetAmount;
    }

    public float GetDistanceFromTargetWithOffsets()
    {
        return Vector3.Distance(actualTarget, boom.position);
    }

    public float GetDistanceFromRealTarget()
    {
        return Vector3.Distance(currentTargetPoint.position, boom.position);
    }

    public float GetYDistance()
    {
        return Mathf.Abs(actualTarget.y - boom.position.y);
    }

    public float GetXZDistance()
    {
        return Vector3.Distance(GetVectorWithLevelY(actualTarget, 0), GetVectorWithLevelY(boom.position, 0));
    }

    #endregion
}
