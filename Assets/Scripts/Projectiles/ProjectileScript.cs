using UnityEngine;

// By Vincent Pressey

public class ProjectileScript : MonoBehaviour
{
    /// <summary>
    /// Damage dealt to object when projectile makes contact
    /// </summary>
    public float ProjectileDamage = 12;


    private void OnTriggerEnter(Collider collider)
    {
        if (collider.isTrigger) { return; }

        Health healthscript;
        if (healthscript = collider.gameObject.GetComponent<Health>()) // Damage object if it has the health script attached
        {
            healthscript.AddToHealth(-ProjectileDamage);
        }

        Destroy(gameObject);
    }
}
