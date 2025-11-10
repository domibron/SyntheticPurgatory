using System;
using UnityEngine;

[ExecuteInEditMode]
public class Crane : MonoBehaviour
{
    [Header("Crane Target"), SerializeField]
    private Transform targetPoint;

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
    private bool boomHasMax = false;

    [SerializeField]
    private float boomMaxDropDistance = 50f;



    [Space, SerializeField]
    private Transform boomCable;




    // Update is called once per frame
    void Update()
    {
        if (enableLerping)
        {

            Vector3 moveAmount = (targetPoint.position - followPoint.position).normalized * rate * Time.deltaTime;

            if (Vector3.Distance(followPoint.position, targetPoint.position) < moveAmount.magnitude)
            {
                followPoint.position = targetPoint.position;
            }
            else
            {
                followPoint.position += moveAmount;
            }
        }
        else
        {
            followPoint.position = targetPoint.position;
        }


        Vector3 targetPos = followPoint.position - transform.position;

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


        // Get bounds for the crane arms.
        extendableArm.localPosition = extendableArmMin.localPosition;
        Vector3 carriageMaxWhenRetracted = extendableArm.localPosition + carriageMax.localPosition;
        Vector3 armRetractedPoint = extendableArm.localPosition;

        if (allowExtension)
        {
            extendableArm.localPosition = extendableArmMax.localPosition;
        }
        else
        {
            extendableArm.localPosition = extendableArmMin.localPosition;
        }
        Vector3 carriageFullMax = extendableArm.localPosition + carriageMax.localPosition;


        // set the carriage before the arm, we can adjust arm to the carriage.
        float targetDistanceFromCrane = Vector3.Distance(Vector3.zero, GetVectorWithLevelY(targetPos, 0));

        float carrageMinDist = Vector3.Distance(Vector3.zero, GetVectorWithLevelY(carriageMin.localPosition, 0));
        float carrageMaxDist = Vector3.Distance(Vector3.zero, GetVectorWithLevelY(carriageFullMax, 0));

        if (targetDistanceFromCrane <= carrageMinDist)
        {
            carriage.localPosition = carriageMin.localPosition;
        }
        else if (targetDistanceFromCrane > carrageMinDist - extensionStartDistance && targetDistanceFromCrane < carrageMaxDist)
        {
            carriage.localPosition = carriageMin.localPosition + (-Vector3.right * (targetDistanceFromCrane - carrageMinDist));
        }
        else if (targetDistanceFromCrane >= carrageMaxDist)
        {
            carriage.localPosition = carriageMin.localPosition + (-Vector3.right * (carrageMaxDist - carrageMinDist));
        }

        // Crane arms.
        if (allowExtension)
        {
            // Extendable Arm
            // is the carriage less than the middle point. I did not abs the values hense the > and not the <.
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



        // Boom (the claw)
        float targetY = followPoint.position.y;


        if (targetY < boomMin.position.y)
        {
            float boomDropAmount = boomMin.position.y - targetY;

            if (boomHasMax && boomDropAmount > boomMaxDropDistance)
            {
                boom.position = new Vector3(boom.position.x, boomMin.position.y - boomMaxDropDistance, boom.position.z);
            }
            else
            {
                boom.position = new Vector3(boom.position.x, boomMin.position.y - boomDropAmount, boom.position.z);
            }
        }
        else if (targetY >= boomMin.position.y)
        {

            boom.position = boomMin.position;
        }

        boomCable.localPosition = boom.localPosition / 2f; // we can ignore the carriage pos since its 0,0,0 and we dont need to calc that.
        boomCable.localScale = new Vector3(boomCable.localScale.x, boom.localPosition.y / 2f, boomCable.localScale.z); // real fucking lazy but it does the job.
    }

    /// <summary>
    /// Simple function to create a new vector with the provided vector but with the new y.
    /// </summary>
    /// <param name="target">The vector to create new vector from.</param>
    /// <param name="y">The new y value for the vector.</param>
    /// <returns>The vector with the given y value</returns>
    private Vector3 GetVectorWithLevelY(Vector3 target, float y)
    {
        return new Vector3(target.x, y, target.z);
    }
}
