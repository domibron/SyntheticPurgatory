using UnityEngine;

public class PlaceableEnemyContainer : MonoBehaviour, ICraneGrabbable
{
    [SerializeField]
    private Transform grabPoint;

    [SerializeField]
    private Transform placementPoint;

    [SerializeField]
    private bool keepOriginalRotation = true;

    private Quaternion originalRotation;

    private bool onHook = false;

    private bool startSpawningEnemies = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (keepOriginalRotation && onHook)
        {
            transform.rotation = originalRotation;
        }

        if (startSpawningEnemies)
        {
            // spawn enemies.
        }
    }



    void ICraneGrabbable.DropObject()
    {
        transform.parent = null;
        onHook = false;
        startSpawningEnemies = true;
    }

    Transform ICraneGrabbable.GetGrabPoint()
    {
        return grabPoint;
    }

    void ICraneGrabbable.PickUpObject(Transform craneHook)
    {
        originalRotation = transform.rotation;
        onHook = true;
        transform.parent = craneHook;
    }

    Transform ICraneGrabbable.GetPlacementPoint()
    {
        return placementPoint;
    }

    bool ICraneGrabbable.GetIsOnHook()
    {
        return onHook;
    }
}
