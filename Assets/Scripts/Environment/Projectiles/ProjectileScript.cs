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

    private void OnTriggerEnter(Collider collider)
    {
        if (hasHit) return;

        collider.gameObject.GetComponent<IDamageDirection>()?.DamagedFrom(SourceForProjectile.position);

        IDamageable damageable = collider.gameObject.GetComponent<IDamageable>();

        if (collider.isTrigger)
        {
            if (damageable != null) // Damage object if it has the enemy damage area script attached
            {
                hasHit = true;

                damageable.TakeDamage(-ProjectileDamage, transform.position);

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

        Destroy(gameObject);

    }

}
