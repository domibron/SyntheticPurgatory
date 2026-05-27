using UnityEngine;

/// <summary>
/// Allows passing damage direction into indicators.
/// </summary>
public interface IDamageDirection
{
    /// <summary>
    /// Inform the object where the source of the damage if from.
    /// </summary>
    /// <param name="positionOfDamageSource">The location of the damage / attacker.</param>
    public void DamagedFrom(Vector3 positionOfDamageSource);
}
