using UnityEngine;

/// <summary>
/// A moving physics object that will deal damage if it hits an object that is damageable, this will also destroy the object on collision. 
/// </summary>
public class ProjectileScript : MonoBehaviour
{
    /// <summary>
    /// Damage dealt to object when projectile makes contact.
    /// </summary>
    [HideInInspector]
    public float ProjectileDamage = 12;

    /// <summary>
    /// Did the projectile already hit something. Prevents entities getting hit multiple times.
    /// </summary>
    private bool hasHit;

    /// <summary>
    /// The source of the projectile, ideally the entity that fired it.
    /// </summary>
    public Transform SourceForProjectile;

    [SerializeField]
    private GameObject decalObject;

    Rigidbody rb;

    SphereCollider sphereCollider;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
        decalObject?.SetActive(false);
    }

    void FixedUpdate()
    {
        if (Physics.SphereCast(transform.position, sphereCollider.radius, rb.linearVelocity.normalized, out RaycastHit hit,
            rb.linearVelocity.magnitude * Time.fixedDeltaTime, sphereCollider.includeLayers, QueryTriggerInteraction.Collide))
        {
            print("fuck");
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (hasHit) return;

        if (SourceForProjectile)
            collider.gameObject.GetComponent<IDamageDirection>()?.DamagedFrom(SourceForProjectile.position);

        IDamageable damageable = collider.gameObject.GetComponent<IDamageable>();

        if (collider.isTrigger)
        {
            if (damageable != null) // Damage object if it has the enemy damage area script attached
            {
                hasHit = true;

                damageable.TakeDamage(-ProjectileDamage, transform.position);

                SetUpDecal(collider);

                Destroy(gameObject);

                return;
            }
            else
            {
                collider.gameObject.GetComponent<IShootable>()?.HitObject(); // just in case the trigger has this.
                return; // cannot hit triggers
            }
        }
        else
        {
            if (damageable != null)
            {
                damageable.TakeDamage(-ProjectileDamage, transform.position);
                hasHit = true;
            }
        }

        collider.gameObject.GetComponent<IShootable>()?.HitObject();

        SetUpDecal(collider);

        Destroy(gameObject);
    }

    private void SetUpDecal(Collider collider)
    {
        if (decalObject)
        {
            decalObject.transform.parent = collider.transform;


            Vector3 targetLocation = Vector3.zero;

            if (Physics.Raycast(transform.position - rb.linearVelocity * 0.1f, rb.linearVelocity.normalized, out RaycastHit hit, rb.linearVelocity.magnitude * 0.2f, rb.includeLayers, QueryTriggerInteraction.Collide))
            {
                targetLocation = hit.point;

                decalObject.transform.position = targetLocation;
                decalObject.transform.LookAt(hit.normal);
                decalObject.transform.rotation = Quaternion.Euler(decalObject.transform.rotation.eulerAngles + new Vector3(-90, 0, 0));
            }
            else
            {
                targetLocation = transform.position;

                decalObject.transform.position = targetLocation;
                decalObject.transform.LookAt(targetLocation - transform.position);
                decalObject.transform.rotation = Quaternion.Euler(decalObject.transform.rotation.eulerAngles + new Vector3(-90, 0, 0));
            }

            decalObject.SetActive(true);

            // TODO: replace with better self destruct script.
            Destroy(decalObject, 10f);
        }
    }

}
