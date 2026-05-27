using UnityEngine;

/// <summary>
/// Used for projectiles to allow for interaction.
/// </summary>
public interface IShootable
{
    /// <summary>
    /// Hit this object with the projectile.
    /// </summary>
    public void HitObject();
}
