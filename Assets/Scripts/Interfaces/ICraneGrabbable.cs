using UnityEngine;

public interface ICraneGrabbable
{
    public void DropObject();

    public void PickUpObject();

    public Vector3 GetGrabPoint();
}
