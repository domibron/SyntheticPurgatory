using UnityEngine;

/// <summary>
/// A rocket projectile.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Rocket : MonoBehaviour
{
    /// <summary>
    /// The attached rigid body.
    /// </summary>
    private Rigidbody rb;

    /// <summary>
    /// The target height for the missiles to fly up to before moving over the target.
    /// </summary>
    [SerializeField]
    private float targetWorldHeight = 10f;

    /// <summary>
    /// The target point to move towards.
    /// </summary>
    private Vector3 targetPosition;

    /// <summary>
    /// Live target to chase after.
    /// </summary>
    private Transform liveTarget;

    /// <summary>
    /// How fast the missile will travel when it's in the barrage mode. M/s.
    /// </summary>
    [SerializeField]
    float barrageSpeed = 5f;

    /// <summary>
    /// The turning rate of the missing in the barrage mode.
    /// </summary>
    [SerializeField]
    float barrageTurningDegreesPerSecond = 150f;

    /// <summary>
    /// The missile speed when chasing a target. M/s.
    /// </summary>
    [SerializeField]
    float chaseSpeed = 5f;

    /// <summary>
    /// The turning speed of the missile when it's in the chase mode.
    /// </summary>
    [SerializeField]
    float chaseTurningDegreesPerSecond = 50f;

    /// <summary>
    /// The damage the missile will do when it explodes.
    /// </summary>
    [SerializeField]
    float explosionDamage = 420;

    /// <summary>
    /// The size of the explosion from the missile.
    /// </summary>
    [SerializeField]
    float explosionRadius = 5;

    /// <summary>
    /// The explosion prefab that will spawn to deal damage and for vfx / sfx.
    /// </summary>
    [SerializeField]
    GameObject explosionPrefab;

    /// <summary>
    /// All the missile states.
    /// </summary>
    enum RocketState
    {
        None,
        Climbing,
        FlyingAboveTarget,
        FlyingToTarget,
        ChaseTarget,
    }

    /// <summary>
    /// The current state the missile is in.
    /// </summary>
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
                transform.rotation = Quaternion.LookRotation(Vector3.up, Utils.GetLevelVectorY(transform.position) - Utils.GetLevelVectorY(targetPosition));
                rb.AddForce((-rb.linearVelocity) + (transform.forward * barrageSpeed), ForceMode.VelocityChange);

                if (transform.position.y > targetWorldHeight)
                {
                    currentState = RocketState.FlyingAboveTarget;
                    // originalRotation = transform.rotation;
                    return;
                }
                break;




            case RocketState.FlyingAboveTarget:

                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Utils.GetLevelVectorY(targetPosition) - Utils.GetLevelVectorY(transform.position), Vector3.up), barrageTurningDegreesPerSecond * Time.fixedDeltaTime);
                // transform.LookAt(GetLevelVector(targetPosition, transform.position.y));
                rb.AddForce((-rb.linearVelocity) + (transform.forward * barrageSpeed), ForceMode.VelocityChange);

                float radius = rb.linearVelocity.magnitude / (barrageTurningDegreesPerSecond * Mathf.Deg2Rad);


                if (Vector3.Distance(Utils.GetLevelVectorY(transform.position), Utils.GetLevelVectorY(targetPosition)) <= radius)
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

    /// <summary>
    /// Set up the rocket with the desired settings.
    /// </summary>
    /// <param name="target">The live target.</param>
    /// <param name="isBarrage">True will make the missile fly using barrage logic./param>
    public void SetUpRocket(Transform target, bool isBarrage = true)
    {
        targetPosition = target.position;
        liveTarget = target;

        if (!isBarrage)
        {
            // Like what the fuck, I have tried so many fucking way to get the missile to face the player and it just got progressively worse till i gave up.
            // TODO: Fix this fucking bullshit later. 20-02-2026 at 2am.
            transform.rotation = Quaternion.LookRotation(Utils.GetLevelVectorY(targetPosition, transform.position.y) - transform.position, Vector3.up);
        }

        // this will indicate to the rest of the system the rocket is now ready.
        currentState = isBarrage ? RocketState.Climbing : RocketState.ChaseTarget;
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        explosion.GetComponent<Explosion>().SetUpExplosion(explosionDamage, explosionRadius);

        Destroy(gameObject);
    }
}

