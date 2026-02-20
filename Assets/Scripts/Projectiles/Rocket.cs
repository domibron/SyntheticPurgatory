using System;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField]
    private float targetWorldHeight = 10f;

    private Vector3 targetPosition;

    private Transform liveTarget;

    [SerializeField]
    float barrageSpeed = 5f;

    [SerializeField]
    float barrageTurningDegreesPerSecond = 150f;

    [SerializeField]
    float chaseSpeed = 5f;

    [SerializeField]
    float chaseTurningDegreesPerSecond = 50f;

    [SerializeField]
    float explosionDamage = 420;

    [SerializeField]
    float explosionRadius = 5;

    [SerializeField]
    GameObject explosionPrefab;

    enum RocketState
    {
        None,
        Climbing,
        FlyingAboveTarget,
        FlyingToTarget,
        ChaseTarget,
    }

    RocketState currentState = RocketState.None;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case RocketState.None:
                return;
            case RocketState.Climbing:
                transform.rotation = Quaternion.LookRotation(Vector3.up, GetLevelVector(transform.position) - GetLevelVector(targetPosition));
                rb.AddForce((-rb.linearVelocity) + (transform.forward * barrageSpeed), ForceMode.VelocityChange);

                if (transform.position.y > targetWorldHeight)
                {
                    currentState = RocketState.FlyingAboveTarget;
                    // originalRotation = transform.rotation;
                    return;
                }
                break;
            case RocketState.FlyingAboveTarget:

                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(GetLevelVector(targetPosition) - GetLevelVector(transform.position), Vector3.up), barrageTurningDegreesPerSecond * Time.fixedDeltaTime);
                // transform.LookAt(GetLevelVector(targetPosition, transform.position.y));
                rb.AddForce((-rb.linearVelocity) + (transform.forward * barrageSpeed), ForceMode.VelocityChange);

                float radius = rb.linearVelocity.magnitude / (barrageTurningDegreesPerSecond * Mathf.Deg2Rad);


                if (Vector3.Distance(GetLevelVector(transform.position), GetLevelVector(targetPosition)) <= radius)
                {
                    currentState = RocketState.FlyingToTarget;
                    // originalRotation = transform.rotation;
                    return;
                }
                break;
            case RocketState.FlyingToTarget:
                // transform.LookAt(targetPosition);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetPosition - transform.position, transform.up), barrageTurningDegreesPerSecond * Time.fixedDeltaTime);
                rb.AddForce((-rb.linearVelocity) + (transform.forward * barrageSpeed), ForceMode.VelocityChange);
                break;
            case RocketState.ChaseTarget:
                targetPosition = liveTarget.position;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetPosition - transform.position, transform.up), chaseTurningDegreesPerSecond * Time.fixedDeltaTime);
                rb.AddForce((-rb.linearVelocity) + (transform.forward * chaseSpeed), ForceMode.VelocityChange);
                break;
        }
    }

    public void SetUpRocket(Transform target, bool isBarrage = true)
    {
        targetPosition = target.position;
        liveTarget = target;

        if (!isBarrage)
        {
            // Like what the fuck, I have tried so many fucking way to get the missile to face the player and it just got progressively worse till i gave up.
            // TODO: Fix this fucking bullshit later. 20-02-2026 at 2am.
            transform.rotation = Quaternion.LookRotation(GetLevelVector(targetPosition, transform.position.y) - transform.position, Vector3.up);
        }

        // this will indicate to the rest of the system the rocket is now ready.
        currentState = isBarrage ? RocketState.Climbing : RocketState.ChaseTarget;
    }

    Vector3 GetLevelVector(Vector3 target, float ySet = 0)
    {
        return new Vector3(target.x, ySet, target.z);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        explosion.GetComponent<Explosion>().SetUpExplosion(explosionDamage, explosionRadius);

        Destroy(gameObject);
    }
}

