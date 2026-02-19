using UnityEngine;

public class Rocket : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField]
    private float targetWorldHeight = 10f;

    private Vector3 targetPosition;

    [SerializeField]
    float speed = 5f;

    [SerializeField]
    float turningDegreesPerSecond = 150f;

    enum RocketState
    {
        Climbing,
        FlyingAboveTarget,
        FlyingToTarget,
        ChaseTarget,
    }

    RocketState currentState = RocketState.Climbing;

    Quaternion originalRotation;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        originalRotation = transform.rotation;
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case RocketState.Climbing:
                transform.rotation = Quaternion.LookRotation(Vector3.up, GetLevelVector(transform.position) - GetLevelVector(targetPosition));
                rb.AddForce((-rb.linearVelocity) + (transform.forward * speed), ForceMode.VelocityChange);

                if (transform.position.y > targetWorldHeight)
                {
                    currentState = RocketState.FlyingAboveTarget;
                    // originalRotation = transform.rotation;
                    return;
                }
                break;
            case RocketState.FlyingAboveTarget:

                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(GetLevelVector(targetPosition) - GetLevelVector(transform.position), Vector3.up), turningDegreesPerSecond * Time.fixedDeltaTime);
                // transform.LookAt(GetLevelVector(targetPosition, transform.position.y));
                rb.AddForce((-rb.linearVelocity) + (transform.forward * speed), ForceMode.VelocityChange);

                float radius = rb.linearVelocity.magnitude / (turningDegreesPerSecond * Mathf.Deg2Rad);


                if (Vector3.Distance(GetLevelVector(transform.position), GetLevelVector(targetPosition)) <= radius)
                {
                    currentState = RocketState.FlyingToTarget;
                    // originalRotation = transform.rotation;
                    return;
                }
                break;
            case RocketState.FlyingToTarget:
                // transform.LookAt(targetPosition);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetPosition - transform.position, transform.up), turningDegreesPerSecond * Time.fixedDeltaTime);
                rb.AddForce((-rb.linearVelocity) + (transform.forward * speed), ForceMode.VelocityChange);
                break;
        }
    }

    public void SetUpRocket(Vector3 target, bool isArial = true)
    {
        originalRotation = transform.rotation;
        targetPosition = target;
        currentState = isArial ? RocketState.Climbing : RocketState.ChaseTarget;
    }

    Vector3 GetLevelVector(Vector3 target, float ySet = 0)
    {
        return new Vector3(target.x, ySet, target.z);
    }
}

