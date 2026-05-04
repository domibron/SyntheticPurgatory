using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnedLevelRoomData
{
    public int ID;
    public Vector2Int GridCoordinates;
    public float OrientationInDegrees = 0; // rotation is such a pain.

    public Vector3 SpawnPointOffset = Vector3.zero;
    public Quaternion Rotation;
    public DoorwayData[] DoorwayData;
    public Vector2Int BoundingSize;

    private GameObject levelPiecePrefab;

    private float UnitSizeInMeters;

    private GameObject roomObject;

    public SpawnedLevelRoomData(int id, Vector2Int gridCoordinates, RoomPiece roomPiece)
    {
        UnitSizeInMeters = roomPiece.UnitSizeInMeters;

        ID = id;

        levelPiecePrefab = roomPiece.GetPrefab();

        OrientationInDegrees = roomPiece.OrientationInDegrees;

        GridCoordinates = gridCoordinates;

        BoundingSize = roomPiece.BoundingSize;

        Rotation = roomPiece.Rotation;

        DoorwayData = new DoorwayData[roomPiece.DoorwayData.Length];
        for (int i = 0; i < DoorwayData.Length; i++)
        {
            DoorwayData[i] = new DoorwayData(roomPiece.DoorwayData[i].Location, roomPiece.DoorwayData[i].FacingDirection);
        }

        if (OrientationInDegrees != 0)
        {

            switch (OrientationInDegrees / 90)
            {
                case 1:
                    SpawnPointOffset.z = roomPiece.BoundingSize.x - 1;
                    break;
                case 2:
                    SpawnPointOffset.z = roomPiece.BoundingSize.x - 1;
                    SpawnPointOffset.x = roomPiece.BoundingSize.y - 1;
                    break;
                case 3:
                    SpawnPointOffset.x = roomPiece.BoundingSize.y - 1;
                    break;
                default: // in case for immense fuck up. I know I would do something like that.
                    SpawnPointOffset = Vector3.zero;
                    break;
            }
        }

    }

    public void SetRoomObject(GameObject roomObject)
    {
        this.roomObject = roomObject;
    }

    public GameObject GetRoomObject()
    {
        return roomObject;
    }

    public GameObject GetPrefab()
    {
        return levelPiecePrefab;
    }

    public Vector3 GetSpawnPoint()
    {
        Vector2 rotatedSpawnPoint = LevelGenerationUtil.RotatePoint(Vector2.zero, new Vector2(BoundingSize.x, BoundingSize.y) / 2f, (-1f * OrientationInDegrees) * Mathf.Deg2Rad);

        return (LevelGenerationUtil.ConvertVector2IntToVector3(GridCoordinates) + new Vector3(rotatedSpawnPoint.x, 0, rotatedSpawnPoint.y)) * UnitSizeInMeters;
    }

    public Vector3 GetOffsetBasedOnDirection()
    {
        if (OrientationInDegrees != 0)
        {
            switch (OrientationInDegrees / 90)
            {
                case 1:
                    return Vector3.forward;
                case 2:
                    return Vector3.forward + Vector3.right;
                case 3:
                    return Vector3.right;
                default: // in case for immense fuck up. I know I would do something like that.
                    return Vector3.zero;
            }
        }
        else
        {
            return Vector3.zero;
        }
    }

    public float GetUnitSizeInMeters()
    {
        return UnitSizeInMeters;
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
