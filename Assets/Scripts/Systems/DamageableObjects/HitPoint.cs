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


    void IDamageable.TakeDamage(DamageContext context)
    {
        // fixed the part multiplier not being applied to the health part by overriding the context damage.
        context.Damage *= hitPointsData.GetMultiplier(hitPointType) * (context.IsCritical ? 2f : 1f);

        health.UpdateHealth(context);

        // should be through the health system.
        if (floatingTextSystem != null)
        {
            floatingTextSystem.SpawnText(Mathf.Abs(context.Damage).ToString("F0"), hitPointsData.GetGradient(hitPointType, context.IsCritical), 3, -10, context.Source);

            if (context.IsCritical)
            {
                floatingTextSystem.SpawnText("Critical", hitPointsData.GetGradient(hitPointType, context.IsCritical), 3, -10, context.Source);
            }
        }
    }
}
