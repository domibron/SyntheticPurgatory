using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Health healthscript;
        if (healthscript = collision.gameObject.GetComponent<Health>()) // Damage object if it has the health script attached
        {
            healthscript.AddToHealth(-12);
        }

        Destroy(gameObject);
    }
}
