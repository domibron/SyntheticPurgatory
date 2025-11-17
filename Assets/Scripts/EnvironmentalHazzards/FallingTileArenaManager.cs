using UnityEngine;

public class FallingTileArenaManager : MonoBehaviour
{
    [SerializeField]
    DroppablePlatform[] fallingTiles;

    Bounds boundsOfOneTile;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boundsOfOneTile = fallingTiles[0].GetBounds();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool CheckTilesFallenInArea(Vector3 checkArea, Vector3 halfExtents)
    {
        // disregard y values.
        checkArea = ResetYValue(checkArea);
        halfExtents = ResetYValue(halfExtents);

        Vector3 backRight = checkArea + halfExtents;
        Vector3 frontLeft = checkArea - halfExtents;

        foreach (var tile in fallingTiles)
        {
            Vector3 tilePos = tile.GetWorldPosition();
            Vector3 tileBackRight = tilePos + boundsOfOneTile.extents;
            Vector3 tileFrontLeft = tilePos - boundsOfOneTile.extents;
            // if (tileBackRight.x < frontLeft.x && tileBackRight.z < frontLeft.z && tileFrontLeft)
        }

        return true;

    }

    private Vector3 ResetYValue(Vector3 vector)
    {
        return new Vector3(vector.x, 0, vector.z);
    }
}
