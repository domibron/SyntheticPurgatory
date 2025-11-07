using System;
using UnityEngine;

[ExecuteInEditMode]
public class Crane : MonoBehaviour
{
    [SerializeField]
    private Transform targetPoint;


    [Space, SerializeField]
    bool enableLerping = true;

    [SerializeField]
    float rate = 1f;

    [SerializeField]
    private Transform followPoint;

    [SerializeField]
    private Transform craneTop;



    [Space, SerializeField]
    bool allowExtension = true;

    [SerializeField]
    float extensionStartDistance = 10f; // 5m from end of inner arm

    [SerializeField]
    private Transform extendableArm;

    [SerializeField]
    private Transform extendableArmMin;

    [SerializeField]
    private Transform extendableArmMax;



    [Space, SerializeField]
    private Transform carriage;

    [SerializeField]
    private Transform carriageMin;

    [SerializeField]
    private Transform carriageMax;



    private Vector3 targetPos;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (enableLerping)
        {
            followPoint.position = Vector3.Lerp(followPoint.position, targetPoint.position, rate * Time.deltaTime);
        }
        else
        {
            followPoint.position = targetPoint.position;
        }


        targetPos = followPoint.position - transform.position;

        // top rotation.
        float angle = Mathf.Atan2(targetPos.z, targetPos.x);

        craneTop.rotation = Quaternion.Euler(0, -(Mathf.Rad2Deg * angle) + 180f, 0);


        // presume x is move direction. Hard coded but gets the job done.
        extendableArm.localPosition = extendableArmMin.localPosition;
        Vector3 carriageMaxWhenRetracted = extendableArm.localPosition + carriageMax.localPosition;

        extendableArm.localPosition = extendableArmMax.localPosition;
        Vector3 carriageFullMax = extendableArm.localPosition + carriageMax.localPosition;


        // carriage
        float targetDistanceFromCrane = Vector3.Distance(Vector3.zero, GetVectorWithLevelY(targetPos, 0));


        float carrageMinDist = Vector3.Distance(Vector3.zero, GetVectorWithLevelY(carriageMin.localPosition, 0));
        float carrageMaxDist = Vector3.Distance(Vector3.zero, GetVectorWithLevelY(carriageFullMax, 0));




        // use max extension to create a lerp, so some dist calc to convert, turn into percentage then lerp.

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



        // Extendable Arm
        // is the carriage less than the middle point. I did not abs the values hense the > and not the <.
        if (carriage.localPosition.x >= carriageMaxWhenRetracted.x + extensionStartDistance)
        {
            extendableArm.localPosition = extendableArmMin.localPosition;
        }
        else if (carriage.localPosition.x < carriageMaxWhenRetracted.x + extensionStartDistance && carriage.localPosition.x > carriageFullMax.x)
        {
            float howMuchToExtendBy = carriage.localPosition.x - (carriageMaxWhenRetracted.x + extensionStartDistance);

            if (howMuchToExtendBy > extendableArmMin.localPosition.x) howMuchToExtendBy = extendableArmMin.localPosition.x;
            else if (howMuchToExtendBy < extendableArmMax.localPosition.x) howMuchToExtendBy = extendableArmMax.localPosition.x;

            extendableArm.localPosition = new Vector3(howMuchToExtendBy, extendableArm.localPosition.y, extendableArm.localPosition.z);
        }
        else if (carriage.localPosition.x <= carriageFullMax.x)
        {
            extendableArm.localPosition = extendableArmMax.localPosition;
        }
    }

    private Vector3 GetVectorWithLevelY(Vector3 target, float y)
    {
        return new Vector3(target.x, y, target.z);
    }
}
