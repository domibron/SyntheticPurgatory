using TMPro;
using UnityEngine;

// By Vincent Pressey

public class ProjectileScript : MonoBehaviour
{
    // public FloatingTextSystem floatingTextSystem;

    /// <summary>
    /// Damage dealt to object when projectile makes contact
    /// </summary>
    public float ProjectileDamage = 12;

    // public TMP_ColorGradient weakSpotGradient;
    // public TMP_ColorGradient normalGradient;
    // public TMP_ColorGradient strongSpotGradient;

    private bool hasHit;

    private float lifeTime = 5;

    private void Start()
    {
        Invoke("DestroyObject", lifeTime);
    }

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

                // float dmgMult = damageArea.GetMultiplier();
                // SpawnFloatingText(dmgMult);

                Destroy(gameObject);
            }
            else return;

        }

        // IDamageable healthscript;
        // if (healthscript = collider.gameObject.GetComponent<IDamageable>()) // Damage object if it has the health script attached
        // {

        collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(-ProjectileDamage, transform.position);
        // SpawnFloatingText(1);
        // }

        Destroy(gameObject);

    }


    // public void SpawnFloatingText(float damageMult)
    // {
    //     TMP_ColorGradient gradient = normalGradient;

    //     if (damageMult > 1.3f)
    //     {
    //         gradient = weakSpotGradient;
    //     }
    //     else if (damageMult < 0.7f)
    //     {
    //         gradient = strongSpotGradient;
    //     }

    //     floatingTextSystem.SpawnText((damageMult * ProjectileDamage).ToString("F0"), gradient, 4, -10);
    // }


    public void DestroyObject()
    {
        Destroy(gameObject);
    }

}
