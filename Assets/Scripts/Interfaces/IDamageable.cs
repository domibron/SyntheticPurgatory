using UnityEngine;

/// <summary>
/// Allows handling and interaction with the damage systems.
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// Damage the object.
    /// </summary>
    /// <param name="damage">The amount to deal.</param>
    /// <param name="hitPosition">The position of the attack / attacker.</param>
    public void TakeDamage(float damage, Vector3 hitPosition);
}
