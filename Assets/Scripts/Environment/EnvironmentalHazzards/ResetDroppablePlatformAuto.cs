using UnityEngine;

/// <summary>
/// Automatically reset the <see cref="DroppablePlatform"/> automatically after a given time.
/// </summary>
[RequireComponent(typeof(DroppablePlatform))]
public class ResetDroppablePlatformAuto : MonoBehaviour
{
    /// <summary>
    /// The attached <see cref="DroppablePlatform"/> that will be reset.
    /// </summary>
    DroppablePlatform droppablePlatform;

    /// <summary>
    /// Has the platform fallen.
    /// </summary>
    private bool hasFallen = false;

    /// <summary>
    /// How long to wait before resetting the platform.
    /// </summary>
    [SerializeField]
    private float waitToReset = 15f;

    /// <summary>
    /// The current time used to track how long left before resetting.
    /// </summary>
    private float currentTimer = 0f;

    void Awake()
    {
        droppablePlatform = GetComponent<DroppablePlatform>();
    }


    // Update is called once per frame
    void Update()
    {
        if (hasFallen && currentTimer > 0)
        {
            currentTimer -= Time.deltaTime;
        }
        else if (hasFallen && currentTimer <= 0)
        {
            droppablePlatform.Rise();
            hasFallen = false;
        }
        else if (!hasFallen && droppablePlatform.HasDropped())
        {
            hasFallen = true;
            currentTimer = waitToReset;
        }
    }

}
