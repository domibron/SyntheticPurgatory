using UnityEngine;

/// <summary>
/// Interface that allows cranes to pick up and place objects.
/// </summary>
public interface ICraneGrabbable
{
    /// <summary>
    /// Unhook the object from the crane.
    /// </summary>
    public void DropObject();

    /// <summary>
    /// Hook the object onto the crane hook.
    /// </summary>
    /// <param name="craneHook">The crane hook to hook up to.</param>
    public void PickUpObject(Transform craneHook);

    /// <summary>
    /// Get the connection point for the crane boom to connect to.
    /// </summary>
    /// <returns>The <see cref="Transform"/> of the grab point.</returns>
    public Transform GetGrabPoint();

    /// <summary>
    /// Get the placement point for use of precision when placing the object.
    /// </summary>
    /// <returns>The <see cref="Transform"/> of the placement point.</returns>
    public Transform GetPlacementPoint();

    /// <summary>
    /// Check to see if the object is connected to a hook.
    /// </summary>
    /// <returns>True if the object is hooked.</returns>
    public bool GetIsOnHook();
}
