using UnityEngine;

/// <summary>
/// Used for cranes to allow dropping / placing objects.
/// </summary>
public class DroppableObject : MonoBehaviour, ICraneGrabbable
{
    /// <summary>
    /// The physics body attached to this object.
    /// </summary>
    private Rigidbody rb;

    /// <summary>
    /// The target grab point for the crane to use to move this object.
    /// </summary>
    [SerializeField]
    private Transform grabPoint;

    /// <summary>
    /// The placement point to use to place this object.
    /// </summary>
    [SerializeField]
    private Transform placementPoint;

    /// <summary>
    /// Ignore the rotation of the crane when moving this object.
    /// Used for precision over realism.
    /// </summary>
    [SerializeField]
    private bool keepOriginalRotation = true;

    /// <summary>
    /// The original rotation if enabled.
    /// </summary>
    private Quaternion originalRotation;

    /// <summary>
    /// Used for checks to see if we are hooked by the crane.
    /// </summary>
    private bool onHook = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Keeps the original rotation. I think it also fixes the position buggy-ness, although i'm not sure. I wrote this 6 months ago in a strange haze powered by monster.
        if (keepOriginalRotation && onHook)
        {
            transform.rotation = originalRotation;
            transform.localPosition = -grabPoint.localPosition; // what?
        }
    }

    /// <summary>
    /// Drops the object.
    /// </summary>
    void ICraneGrabbable.DropObject()
    {
        transform.parent = null;
        onHook = false;
        rb.isKinematic = false;
    }

    /// <summary>
    /// Get the grab point.
    /// </summary>
    /// <returns>The transform of the grab point.</returns>
    Transform ICraneGrabbable.GetGrabPoint()
    {
        return grabPoint;
    }

    /// <summary>
    /// Get whether we are on the hook or not.
    /// </summary>
    /// <returns></returns>
    bool ICraneGrabbable.GetIsOnHook()
    {
        return onHook;
    }

    /// <summary>
    /// Get the placement point of this object.
    /// </summary>
    /// <returns>The transform of the placement point.</returns>
    Transform ICraneGrabbable.GetPlacementPoint()
    {
        return placementPoint;
    }

    /// <summary>
    /// Connect to the crane hook.
    /// </summary>
    /// <param name="craneHook">The crane hook to hook up to.</param>
    void ICraneGrabbable.PickUpObject(Transform craneHook)
    {
        originalRotation = transform.rotation;
        onHook = true;
        transform.parent = craneHook;
        rb.isKinematic = true;
    }
}
