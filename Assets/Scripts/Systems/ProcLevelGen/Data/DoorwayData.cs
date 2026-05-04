using System;
using UnityEngine;

[Serializable]
public class DoorwayData
{
    public Vector2Int Location;
    public CompassDirection FacingDirection;

    public DoorwayData(Vector2Int location = new Vector2Int(), CompassDirection direction = CompassDirection.North)
    {
        Location = location;
        FacingDirection = direction;
    }

    public Vector2Int GetFacingAsVector()
    {
        return LevelGenerationUtil.GetCompassDirectionAsVector2Int(FacingDirection);
    }
}