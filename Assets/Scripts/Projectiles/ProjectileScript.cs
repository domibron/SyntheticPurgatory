using TMPro;
using UnityEngine;

// By Vincent Pressey

public class ProjectileScript : MonoBehaviour
{
    /// <summary>
    /// Damage dealt to object when projectile makes contact
    /// </summary>
    [HideInInspector]
    public float ProjectileDamage = 12;
     
    private bool hasHit;

    private void OnTriggerEnter(Collider collider)
    {
        if (hasHit) return;

        if (collider.isTrigger)
        {

            IDamageable damageArea = collider.gameObject.GetComponent<IDamageable>();
            if (damageArea != null) // Damage object if it has the enemy damage area script attached
            {
                hasHit = true;

                damageArea.TakeDamage(-ProjectileDamage, transform.position);

                Destroy(gameObject);

                return;
            }
            else return;

        }

        collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(-ProjectileDamage, transform.position);

        Destroy(gameObject);

    }

}
