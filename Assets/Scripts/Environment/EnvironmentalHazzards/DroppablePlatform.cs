using UnityEngine;
using UnityEngine.AI;

public class DroppablePlatform : MonoBehaviour
{
    [SerializeField]
    private bool startOpen = false;

    [SerializeField]
    private bool hideAfterDistance = false;

    [SerializeField]
    private float maxDistanceBeforeHiding;

    [SerializeField, Min(0.0001f)]
    private float fallRate = 50;

    [SerializeField, Min(0.0001f)]
    private float riseRate = 50;

    // [SerializeField]
    // private Vector3 fallDirection = Vector3.down;

    [SerializeField]
    private Transform platform;

    private Vector3 defaultPos;

    // TODO: make it vel so the platforms speed up when falling to add polish.
    private Vector3 vel;

    [SerializeField]
    NavMeshObstacle navMeshObstacle;

    [SerializeField]
    Bounds bounds;

    private enum PlatformState
    {
        None,
        Dropping,
        Rising,
        Hidden,
    }

    private PlatformState platformState = PlatformState.None;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        defaultPos = platform.localPosition;

        // fallDirection.Normalize();


        if (startOpen)
        {
            Drop();
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (platformState)
        {
            case PlatformState.Hidden:
                platform.gameObject.SetActive(false);
                if (navMeshObstacle != null) navMeshObstacle.enabled = true;
                break;
            case PlatformState.Dropping:
                platform.gameObject.SetActive(true);
                if (navMeshObstacle != null) navMeshObstacle.enabled = true;
                if (Vector3.Distance(platform.localPosition, defaultPos) >= maxDistanceBeforeHiding && hideAfterDistance)
                {
                    platformState = PlatformState.Hidden;
                    break;
                }

                platform.Translate(Vector3.down * fallRate * Time.deltaTime, Space.World);

                break;
            case PlatformState.Rising:

                platform.gameObject.SetActive(true);
                if (platform.localPosition.y - defaultPos.y >= defaultPos.y)
                {
                    platformState = PlatformState.None;
                    if (navMeshObstacle != null) navMeshObstacle.enabled = false;
                    break;
                }

                platform.Translate(Vector3.up * riseRate * Time.deltaTime, Space.World);

                break;
            case PlatformState.None:
                platform.gameObject.SetActive(true);
                if (navMeshObstacle != null) navMeshObstacle.enabled = false;
                platform.localPosition = defaultPos;
                break;
        }
    }

    // Drops the platform
    public void Drop()
    {
        platformState = PlatformState.Dropping;
    }

    // inverse to drop, raises the platform.
    public void Rise()
    {
        platformState = PlatformState.Rising;
    }

    // instantly teleport the platform
    public void Reset()
    {
        platformState = PlatformState.None;
    }

    public bool HasDropped()
    {
        return platformState == PlatformState.Dropping || platformState == PlatformState.Hidden;
    }

    public Bounds GetBounds()
    {
        return bounds;
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }
}
