using UnityEngine;

/// <summary>
/// used to check for falling tiles.
/// </summary>
public class FallingTileArenaManager : MonoBehaviour
{
    /// <summary>
    /// All the falling tiles in the arena.
    /// </summary>
    [SerializeField]
    DroppablePlatform[] fallingTiles;

    /// <summary>
    /// The bounds of one tiles.
    /// </summary>
    Bounds boundsOfOneTile;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boundsOfOneTile = fallingTiles[0].GetBounds();
    }

    /// <summary>
    /// Check to see if any tiles have falling in the specified bounds.
    /// </summary>
    /// <param name="checkArea">The centre point of the check.</param>
    /// <param name="halfExtents">The half extents of the check.</param>
    /// <returns></returns>
    public bool CheckTilesFallenInArea(Vector3 checkArea, Vector3 halfExtents)
    {
        // disregard y values.
        checkArea = Utils.GetLevelVectorY(checkArea);
        halfExtents = Utils.GetLevelVectorY(halfExtents); // * Bug? setting y to 0?

        Vector3 backRight = checkArea + halfExtents;
        Vector3 frontLeft = checkArea - halfExtents;

        foreach (var tile in fallingTiles)
        {
            Vector3 tilePos = tile.GetWorldPosition();
            Vector3 tileBackRight = tilePos + boundsOfOneTile.extents;
            Vector3 tileFrontLeft = tilePos - boundsOfOneTile.extents;
            if ((tileBackRight.x < frontLeft.x && tileBackRight.z < frontLeft.z) || (tileFrontLeft.x > backRight.x && tileFrontLeft.z > backRight.z))
            {
                if (tile.HasDropped()) return true;
            }
        }

        return false;

    }
}
