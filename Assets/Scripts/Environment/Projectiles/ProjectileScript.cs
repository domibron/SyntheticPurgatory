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
        if (hasHit) return;


        if (Physics.SphereCast(transform.position - (rb.linearVelocity.normalized * Time.fixedDeltaTime), sphereCollider.radius, rb.linearVelocity.normalized, out RaycastHit hit,
            rb.linearVelocity.magnitude * Time.fixedDeltaTime, sphereCollider.includeLayers, QueryTriggerInteraction.Collide))
        {


            if (SourceForProjectile)
                hit.transform.GetComponent<IDamageDirection>()?.DamagedFrom(SourceForProjectile.position);

            IDamageable damageable = hit.transform.GetComponent<IDamageable>();

            if (hit.collider.isTrigger)
            {
                if (damageable != null) // Damage object if it has the enemy damage area script attached
                {
                    hasHit = true;

                    hit.transform.GetComponent<IShootable>()?.HitObject();
                    damageable.TakeDamage(-ProjectileDamage, transform.position);

                    SetUpDecal(hit);

                    // print("trigger");


                    Destroy(gameObject);

                    return;
                }
                else
                {
                    hit.transform.GetComponent<IShootable>()?.HitObject(); // just in case the trigger has this.
                    return; // cannot hit triggers
                }
            }
            else
            {
                if (damageable != null)
                {
                    hasHit = true;

                    hit.transform.GetComponent<IShootable>()?.HitObject();
                    damageable.TakeDamage(-ProjectileDamage, transform.position);
                    SetUpDecal(hit);

                    // print("Special");

                    Destroy(gameObject);
                    return;

                }
            }

            hit.transform.GetComponent<IShootable>()?.HitObject();

            // print("normal");

            SetUpDecal(hit);

            Destroy(gameObject);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        // ProjectileLogic(other);
    }

    private void ProjectileLogic(Collider other)
    {
        if (hasHit) return;

        if (SourceForProjectile)
            other.gameObject.GetComponent<IDamageDirection>()?.DamagedFrom(SourceForProjectile.position);

        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();

        if (other.isTrigger)
        {
            if (damageable != null) // Damage object if it has the enemy damage area script attached
            {
                hasHit = true;

                damageable.TakeDamage(-ProjectileDamage, transform.position);

                SetUpDecal(other);

                Destroy(gameObject);

                return;
            }
            else
            {
                other.gameObject.GetComponent<IShootable>()?.HitObject(); // just in case the trigger has this.
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

        other.gameObject.GetComponent<IShootable>()?.HitObject();

        SetUpDecal(other);

        Destroy(gameObject);
    }

    private void SetUpDecal(RaycastHit hit)
    {
        if (!decalObject) return;


        // decalObject.transform.position = hit.point + (hit.normal.normalized * 0.5f);
        // decalObject.transform.LookAt(transform.position + -hit.normal.normalized);
        decalObject.transform.position = hit.point + (-rb.linearVelocity.normalized * 0.5f);
        decalObject.transform.LookAt(transform.position + rb.linearVelocity.normalized);
        // decalObject.transform.rotation = Quaternion.Euler(decalObject.transform.rotation.eulerAngles);

        decalObject.transform.parent = null;

        decalObject.SetActive(true);

        Destroy(decalObject, 10f);


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
