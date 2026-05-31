using UnityEngine;


/// <summary>
/// A simple script that deals damage to the entity.
/// </summary>
public class StandardDamageableObject : MonoBehaviour, IDamageable
{
    /// <summary>
    /// The health class attached to this entity.
    /// </summary>
    private Health health;


    void Awake()
    {
        health = GetComponent<Health>();
    }

    void IDamageable.TakeDamage(float damage, Vector3 hitPosition)
    {
        // floating numbers pop up.
        health.AddToHealth(-Mathf.Abs(damage));

        // print(hitPointsData.GetGradient(type));
    }
}
