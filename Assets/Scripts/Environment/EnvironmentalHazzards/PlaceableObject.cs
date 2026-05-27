using UnityEngine;

/// <summary>
/// Crane placeable object.
/// </summary>
public class PlaceableObject : MonoBehaviour, ICraneGrabbable
{
    /// <summary>
    /// The target point for the crane to grab.
    /// </summary>
    [SerializeField]
    private Transform grabPoint;

    /// <summary>
    /// The target point for the crane to use for alignment when placing.
    /// </summary>
    [SerializeField]
    private Transform placementPoint;

    /// <summary>
    /// Ignore the crane's rotation when this object is moved.
    /// </summary>
    [SerializeField]
    private bool keepOriginalRotation = true;

    /// <summary>
    /// The original rotation when <see cref="keepOriginalRotation"/> is set to TRUE.
    /// </summary>
    private Quaternion originalRotation;

    /// <summary>
    /// Used to track if we are still on the hook.
    /// </summary>
    private bool onHook = false;


    // Update is called once per frame
    void Update()
    {
        if (keepOriginalRotation && onHook)
        {
            transform.rotation = originalRotation;
        }
    }

    /// <summary>
    /// Place down this object.
    /// </summary>
    void ICraneGrabbable.DropObject()
    {
        transform.parent = null;
        onHook = false;
    }

    /// <summary>
    /// Get the <see cref="grabPoint"/>'s <see cref="Transform"/> of this object.
    /// </summary>
    /// <returns>The <see cref="Transform"/> of the grab point.</returns>
    Transform ICraneGrabbable.GetGrabPoint()
    {
        return grabPoint;
    }

    /// <summary>
    /// Attack the object to the provided <paramref name="craneHook"/>.
    /// </summary>
    /// <param name="craneHook">The crane hook to attack to.</param>
    void ICraneGrabbable.PickUpObject(Transform craneHook)
    {
        originalRotation = transform.rotation;
        onHook = true;
        transform.parent = craneHook;
    }

    /// <summary>
    /// Get the <see cref="Transform"/> of the placement point.
    /// </summary>
    /// <returns>The <see cref="Transform"/> of the placement point.</returns>
    Transform ICraneGrabbable.GetPlacementPoint()
    {
        return placementPoint;
    }

    /// <summary>
    /// Check to see if this object is on the hook.
    /// </summary>
    /// <returns>True if the object is on the hook.</returns>
    bool ICraneGrabbable.GetIsOnHook()
    {
        return onHook;
    }
}
