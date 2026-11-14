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
    private bool m_hasHit;

    /// <summary>
    /// The source of the projectile, ideally the entity that fired it.
    /// </summary>
    public Transform SourceForProjectile;

    public float CriticalChance;

    [SerializeField]
    private GameObject m_decalObject;

    Rigidbody m_rb;

    SphereCollider m_sphereCollider;

    void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
        m_sphereCollider = GetComponent<SphereCollider>();

        if (m_decalObject != null)
            m_decalObject?.SetActive(false);
    }

    void FixedUpdate()
    {
        if (m_hasHit) return;


        if (Physics.SphereCast(transform.position - (m_rb.linearVelocity.normalized * Time.fixedDeltaTime), m_sphereCollider.radius, m_rb.linearVelocity.normalized, out RaycastHit hit,
            m_rb.linearVelocity.magnitude * Time.fixedDeltaTime, m_sphereCollider.includeLayers, QueryTriggerInteraction.Collide))
        {


            if (SourceForProjectile)
                hit.transform.GetComponent<IDamageDirection>()?.DamagedFrom(SourceForProjectile.position);

            IDamageable damageable = hit.transform.GetComponent<IDamageable>();

            if (hit.collider.isTrigger)
            {
                if (damageable != null) // Damage object if it has the enemy damage area script attached
                {
                    m_hasHit = true;

                    hit.transform.GetComponent<IShootable>()?.HitObject();
                    // damageable.TakeDamage(-ProjectileDamage, transform.position);
                    damageable.TakeDamage(new DamageContext(ProjectileDamage, transform.position, DamageTypes.Normal,
                        UnityEngine.Random.Range(0f, 1f) <= CriticalChance ? true : false));

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
                    m_hasHit = true;

                    hit.transform.GetComponent<IShootable>()?.HitObject();
                    // damageable.TakeDamage(-ProjectileDamage, transform.position);
                    damageable.TakeDamage(new DamageContext(ProjectileDamage, transform.position, DamageTypes.Normal,
                        UnityEngine.Random.Range(0f, 1f) <= CriticalChance ? true : false));
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


    // Collision events do not trigger like I suspected.
    // Trigger enter events failing to activate for fast moving projectiles.
    // void OnCollisionEnter(Collision collision)
    // {
    //     print("collision for a trigger?");
    // }


    private void OnTriggerEnter(Collider other)
    {
        // ProjectileLogic(other);
    }

    private void ProjectileLogic(Collider other)
    {
        if (m_hasHit) return;

        if (SourceForProjectile)
            other.gameObject.GetComponent<IDamageDirection>()?.DamagedFrom(SourceForProjectile.position);

        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();

        if (other.isTrigger)
        {
            if (damageable != null) // Damage object if it has the enemy damage area script attached
            {
                m_hasHit = true;

                // damageable.TakeDamage(-ProjectileDamage, transform.position);
                damageable.TakeDamage(new DamageContext(ProjectileDamage, transform.position, DamageTypes.Normal,
                    UnityEngine.Random.Range(0f, 1f) <= CriticalChance ? true : false));


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
                // damageable.TakeDamage(-ProjectileDamage, transform.position);
                damageable.TakeDamage(new DamageContext(ProjectileDamage, transform.position, DamageTypes.Normal,
                    UnityEngine.Random.Range(0f, 1f) <= CriticalChance ? true : false));

                m_hasHit = true;
            }
        }

        other.gameObject.GetComponent<IShootable>()?.HitObject();

        SetUpDecal(other);

        Destroy(gameObject);
    }

    private void SetUpDecal(RaycastHit hit)
    {
        if (!m_decalObject) return;


        // decalObject.transform.position = hit.point + (hit.normal.normalized * 0.5f);
        // decalObject.transform.LookAt(transform.position + -hit.normal.normalized);
        m_decalObject.transform.position = hit.point + (-m_rb.linearVelocity.normalized * 0.5f);
        m_decalObject.transform.LookAt(transform.position + m_rb.linearVelocity.normalized);
        // decalObject.transform.rotation = Quaternion.Euler(decalObject.transform.rotation.eulerAngles);

        m_decalObject.transform.parent = hit.transform;

        m_decalObject.SetActive(true);

        Destroy(m_decalObject, 10f);


    }

    private void SetUpDecal(Collider collider)
    {
        if (m_decalObject)
        {
            m_decalObject.transform.parent = collider.transform;


            Vector3 targetLocation = Vector3.zero;

            if (Physics.Raycast(transform.position - m_rb.linearVelocity * 0.1f, m_rb.linearVelocity.normalized, out RaycastHit hit, m_rb.linearVelocity.magnitude * 0.2f, m_rb.includeLayers, QueryTriggerInteraction.Collide))
            {
                targetLocation = hit.point;

                m_decalObject.transform.position = targetLocation;
                m_decalObject.transform.LookAt(hit.normal);
                m_decalObject.transform.rotation = Quaternion.Euler(m_decalObject.transform.rotation.eulerAngles + new Vector3(-90, 0, 0));
            }
            else
            {
                targetLocation = transform.position;

                m_decalObject.transform.position = targetLocation;
                m_decalObject.transform.LookAt(targetLocation - transform.position);
                m_decalObject.transform.rotation = Quaternion.Euler(m_decalObject.transform.rotation.eulerAngles + new Vector3(-90, 0, 0));
            }

            m_decalObject.SetActive(true);

            // TODO: replace with better self destruct script.
            Destroy(m_decalObject, 10f);
        }
    }

}
