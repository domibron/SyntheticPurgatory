using UnityEngine;

public interface ICraneGrabbable
{
    public void DropObject();

    public void PickUpObject(Transform craneHook);

    public Transform GetGrabPoint();

    public Transform GetPlacementPoint();

    public bool GetIsOnHook();
}
