using UnityEngine;

public class DroppableObject : MonoBehaviour, ICraneGrabbable
{
    private Rigidbody rb;

    [SerializeField]
    private Transform grabPoint;

    [SerializeField]
    private Transform placementPoint;

    [SerializeField]
    private bool keepOriginalRotation = true;

    private Quaternion originalRotation;

    private bool onHook = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (keepOriginalRotation && onHook)
        {
            transform.rotation = originalRotation;
        }
    }

    void ICraneGrabbable.DropObject()
    {
        transform.parent = null;
        onHook = false;
        rb.isKinematic = false;
    }

    Transform ICraneGrabbable.GetGrabPoint()
    {
        return grabPoint;
    }

    bool ICraneGrabbable.GetIsOnHook()
    {
        return onHook;
    }

    Transform ICraneGrabbable.GetPlacementPoint()
    {
        return placementPoint;
    }

    void ICraneGrabbable.PickUpObject(Transform craneHook)
    {
        originalRotation = transform.rotation;
        onHook = true;
        transform.parent = craneHook;
        rb.isKinematic = true;
    }
}
