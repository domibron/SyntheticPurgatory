using TMPro;
using UnityEngine;

// By Vincent Pressey

public class ProjectileScript : MonoBehaviour
{
    public FloatingTextSystem floatingTextSystem;

    /// <summary>
    /// Damage dealt to object when projectile makes contact
    /// </summary>
    public float ProjectileDamage = 12;

    public TMP_ColorGradient weakSpotGradient;
    public TMP_ColorGradient normalGradient;
    public TMP_ColorGradient strongSpotGradient;

    private bool hasHit;

    private float lifeTime = 5;

    private void Start()
    {
        Invoke("DestroyObject", lifeTime);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (hasHit) { return; }

        if (collider.isTrigger)
        {

            EnemyDamageArea damageArea;
            if (damageArea = collider.gameObject.GetComponent<EnemyDamageArea>()) // Damage object if it has the enemy damage area script attached
            {
                hasHit = true;

                damageArea.TakeDamage(-ProjectileDamage);

                float dmgMult = damageArea.GetMultiplier();
                SpawnFloatingText(dmgMult);

                Destroy(gameObject);
            }
            else { return; }

        }

        Health healthscript;
        if (healthscript = collider.gameObject.GetComponent<Health>()) // Damage object if it has the health script attached
        {
            print("FSUFS");
            healthscript.AddToHealth(-ProjectileDamage);
        }

        Destroy(gameObject);

    }

    public void SpawnFloatingText(float damageMult)
    {
        TMP_ColorGradient gradient = normalGradient;

        if (damageMult > 1.3f)
        {
            gradient = weakSpotGradient;
        }
        else if (damageMult < 0.7f)
        {
            gradient = strongSpotGradient;
        }

        floatingTextSystem.SpawnText((damageMult * ProjectileDamage).ToString("F0"), gradient, 5);
    }


    public void DestroyObject()
    {
        Destroy(gameObject);
    }

}
