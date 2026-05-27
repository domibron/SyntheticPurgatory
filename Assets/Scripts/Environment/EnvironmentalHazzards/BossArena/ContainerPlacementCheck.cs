using UnityEngine;

/// <summary>
/// Container placement check to make sure the container can be placed down without overlap or missing floor.
/// </summary>
public class ContainerPlacementCheck : MonoBehaviour
{
    /// <summary>
    /// The bounds of the check.
    /// </summary>
    BoxCollider boxCollider;


    /// <summary>
    /// The layers to check for.
    /// </summary>
    [SerializeField]
    LayerMask layerMask;

    /// <summary>
    /// All the falling floor tiles.
    /// </summary>
    [SerializeField]
    FallingTileArenaManager fallingTileArenaManager;

    /// <summary>
    /// The out of bounds reset position once check has been completed.
    /// </summary>
    private Vector3 resetPoint;

    void Awake()
    {
        resetPoint = transform.position;
        boxCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        // Physics.CheckBox

        // print(SampleContainerPosition(transform.position));

    }

    /// <summary>
    /// Moves the check box out of bounds.
    /// </summary>
    public void ResetPosition()
    {
        transform.position = resetPoint;
    }

    /// <summary>
    /// Check the target position if it is a valid position.
    /// </summary>
    /// <param name="targetPos">The target position to check at.</param>
    /// <returns>True if it's a valid position.</returns>
    public bool SampleContainerPosition(Vector3 targetPos)
    {
        transform.position = targetPos;

        Collider[] colliders = Physics.OverlapBox(transform.position + boxCollider.center, boxCollider.size / 2f, transform.rotation, layerMask);

        if (colliders.Length > 0)
        {
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.CompareTag(Constants.MediumDetailTag) || collider.gameObject.CompareTag(Constants.LowDetailTag))
                {
                    return false;
                }
            }
        }

        if (fallingTileArenaManager.CheckTilesFallenInArea(transform.position + boxCollider.center, boxCollider.size / 2f))
        {
            // print("Tiles failed");
            return false;
        }



        return true;
    }
}
