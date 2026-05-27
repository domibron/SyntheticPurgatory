using UnityEngine;

/// <summary>
/// Allows objects to be kicked by the player.
/// </summary>
public interface IKickable
{
    /// <summary>
    /// Kick this object.
    /// </summary>
    /// <param name="forceAndDir">The force with the direction together.</param>
    /// <param name="forceMode">The force mode for more control.</param>
    public void KickObject(Vector3 forceAndDir, ForceMode forceMode = ForceMode.Force);
}
