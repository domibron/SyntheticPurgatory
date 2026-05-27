using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// A platform that can be dropped. A collapsing platform.
/// </summary>
public class DroppablePlatform : MonoBehaviour
{
    /// <summary>
    /// Start already fallen.
    /// </summary>
    [SerializeField]
    private bool startOpen = false;

    /// <summary>
    /// Hide the platform after it falls a certain amount.
    /// </summary>
    [SerializeField]
    private bool hideAfterDistance = false;

    /// <summary>
    /// The max amount of distance the platform must fall before hiding.
    /// </summary>
    [SerializeField]
    private float maxDistanceBeforeHiding;

    /// <summary>
    /// How fast the platform falls. m/s
    /// </summary>
    [SerializeField, Min(0.0001f)]
    private float fallRate = 50;

    /// <summary>
    /// How fast the platform rises. m/s
    /// </summary>
    [SerializeField, Min(0.0001f)]
    private float riseRate = 50;

    // [SerializeField]
    // private Vector3 fallDirection = Vector3.down;

    /// <summary>
    /// The platform to move.
    /// </summary>
    [SerializeField]
    private Transform platform;

    /// <summary>
    /// The default location for the platform.
    /// </summary>
    private Vector3 defaultPos;

    // TODO: make it vel so the platforms speed up when falling to add polish.
    /// <summary>
    /// The current velocity of the platform.
    /// </summary>
    private Vector3 vel;

    /// <summary>
    /// The attached navMeshObstacle to allow AI to avoid the gaping hole.
    /// </summary>
    [SerializeField]
    NavMeshObstacle navMeshObstacle;

    /// <summary>
    /// The bounds of the platform. Used specifically for the arena falling tile manager.
    /// </summary>
    [SerializeField]
    Bounds bounds;

    /// <summary>
    /// All the states the platform can be in.
    /// </summary>
    private enum PlatformState
    {
        None,
        Dropping,
        Rising,
        Hidden,
    }

    /// <summary>
    /// The current state the platform is in.
    /// </summary>
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

    /// <summary>
    /// Make the platform fall.
    /// </summary>
    public void Drop()
    {
        platformState = PlatformState.Dropping;
    }

    /// <summary>
    /// Make the platform rise into position.
    /// </summary>
    public void Rise()
    {
        platformState = PlatformState.Rising;
    }

    /// <summary>
    /// Instantly set the platform back to the default position.
    /// </summary>
    public void Reset()
    {
        platformState = PlatformState.None;
    }

    /// <summary>
    /// Check to see if the platform is missing.
    /// </summary>
    /// <returns>True if the platform has dropped or is dropping.</returns>
    public bool HasDropped()
    {
        return platformState == PlatformState.Dropping || platformState == PlatformState.Hidden;
    }

    /// <summary>
    /// Get the bounds of the platform.
    /// </summary>
    /// <returns>The bounds of this platform.</returns>
    public Bounds GetBounds()
    {
        return bounds;
    }

    /// <summary>
    /// Get the world position of the <see cref="platform"/>
    /// </summary>
    /// <returns>The 3d vector of the platform's position in the world.</returns>
    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }
}
