using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomPiece
{
    private SO_LevelPiece LevelPiece;
    public float OrientationInDegrees = 0; // rotation is such a pain.

    public Vector3 SpawnPointOffset = Vector3.zero;
    public Quaternion Rotation;
    public DoorwayData[] DoorwayData;
    public Vector2Int BoundingSize;

    public float UnitSizeInMeters;

    public RoomPiece(SO_LevelPiece levelPiece, float orientationInDegrees, float unitSizeInMeters)
    {
        UnitSizeInMeters = unitSizeInMeters;

        LevelPiece = levelPiece;

        if (orientationInDegrees >= 0 && orientationInDegrees <= 360)
            OrientationInDegrees = Mathf.Abs(orientationInDegrees);
        else
            OrientationInDegrees = 0f;

        BoundingSize = LevelPiece.BoundingSize;

        if (orientationInDegrees != 0)
        {
            if ((orientationInDegrees / 90) % 2 == 1)
            {
                BoundingSize.x = LevelPiece.BoundingSize.y;
                BoundingSize.y = LevelPiece.BoundingSize.x;
            }
        }

        Rotation = Quaternion.Euler(0, orientationInDegrees, 0);

        DoorwayData = new DoorwayData[LevelPiece.DoorwayData.Length];
        for (int i = 0; i < DoorwayData.Length; i++)
        {
            DoorwayData[i] = new DoorwayData(LevelPiece.DoorwayData[i].Location, LevelPiece.DoorwayData[i].FacingDirection);
        }

        if (orientationInDegrees != 0)
        {

            switch (orientationInDegrees / 90)
            {
                case 1:
                    SpawnPointOffset.z = levelPiece.BoundingSize.x - 1;
                    break;
                case 2:
                    SpawnPointOffset.z = levelPiece.BoundingSize.x - 1;
                    SpawnPointOffset.x = levelPiece.BoundingSize.y - 1;
                    break;
                case 3:
                    SpawnPointOffset.x = levelPiece.BoundingSize.y - 1;
                    break;
                default: // in case for immense fuck up. I know I would manage something like that.
                    SpawnPointOffset = Vector3.zero;
                    break;
            }


            RotateAllDoorways();
        }
    }

    public SO_LevelPiece GetLevelPiece()
    {
        return LevelPiece;
    }

    public GameObject GetPrefab()
    {
        return LevelPiece.LevelPiecePrefab;
    }

    private void RotateAllDoorways()
    {
        for (int i = 0; i < DoorwayData.Length; i++)
        {
            DoorwayData[i] = GetDoorwayAfterRotation(DoorwayData[i]);
        }
    }

    public DoorwayData GetDoorwayAfterRotation(DoorwayData doorwayToRotate)
    {

        DoorwayData returnedDoorwayData = doorwayToRotate;

        // Debug.Log($"before: {returnedDoorwayData.Location}");

        Vector2 rotatedPoint = LevelGenerationUtil.RotatePoint(doorwayToRotate.Location, (new Vector2(LevelPiece.BoundingSize.x, LevelPiece.BoundingSize.y) - Vector2.one) / 2f,
            (-1f * OrientationInDegrees) * Mathf.Deg2Rad); // times -1 to turn into negative to inverse rotation direction to be clockwise rather than counter.

        returnedDoorwayData.Location = new Vector2Int(Mathf.RoundToInt(rotatedPoint.x), Mathf.RoundToInt(rotatedPoint.y));
        // Debug.Log($"after: {returnedDoorwayData.Location} rr {rotatedPoint}");


        returnedDoorwayData.FacingDirection = LevelGenerationUtil.RotateCompassDirectionByDegrees(doorwayToRotate.FacingDirection, OrientationInDegrees);

        return returnedDoorwayData;
    }

    public List<DoorwayData> GetDoorwaysFacingThatDirection(CompassDirection facingDirection)
    {
        List<DoorwayData> doorways = new List<DoorwayData>();

        foreach (DoorwayData doorway in DoorwayData)
        {
            if (doorway.FacingDirection == facingDirection)
            {
                doorways.Add(doorway);
            }
        }

        return doorways;
    }

    public bool HasAnyDoorwayWithFacingDirection(CompassDirection direction)
    {
        foreach (DoorwayData doorway in DoorwayData)
        {
            if (doorway.FacingDirection == direction)
            {
                return true;
            }
        }

        return false;
    }
}
