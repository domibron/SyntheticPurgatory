using UnityEngine;

/// <summary>
/// A single hit point on the entity, you can use multiple of these to create multiple hit points like a weak point on the head.
/// </summary>
public class HitPoint : MonoBehaviour, IDamageable
{
    /// <summary>
    /// The strength of this hit point.
    /// </summary>
    [SerializeField]
    private HitPointType hitPointType = HitPointType.normal;

    /// <summary>
    /// The gradient for the hit points. Optional.
    /// </summary>
    [SerializeField]
    private HitPointsDataSO hitPointsData;

    /// <summary>
    /// The floating text system to display the damage to. Optional.
    /// </summary>
    [SerializeField]
    private FloatingTextSystem floatingTextSystem;

    /// <summary>
    /// The health of this entity to deal damage from this point.
    /// </summary>
    [SerializeField]
    private Health health;


    void IDamageable.TakeDamage(float damage, Vector3 hitPosition)
    {
        float totalDamage = damage * hitPointsData.GetMultiplier(hitPointType);

        health.AddToHealth(-Mathf.Abs(totalDamage));

        if (floatingTextSystem != null)
            floatingTextSystem.SpawnText(Mathf.Abs(totalDamage).ToString("F0"), hitPointsData.GetGradient(hitPointType, false), 3, -10, hitPosition);
    }
}
