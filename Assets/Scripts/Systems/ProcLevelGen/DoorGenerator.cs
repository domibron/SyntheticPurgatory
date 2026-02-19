using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DoorData
{
    public Vector2Int CordsOne = Vector2Int.zero;
    public Vector2Int CordsTwo = Vector2Int.zero;
    public Vector2Int GridCoordinates = Vector2Int.zero;
    public int ID = -1;
    public int RoomOneID = -1;
    public int RoomTwoID = -1;
    public GameObject DoorObject;
    public Door DoorScript;

    public DoorData(int roomID, Vector2Int roomOneCords, Vector2Int roomTwoCords, int firstRoomID, int secondRoomID, Vector2Int roomPosition, GameObject doorGameObject)
    {
        CordsOne = roomOneCords;
        CordsTwo = roomTwoCords;
        GridCoordinates = roomPosition;
        ID = roomID;
        RoomOneID = firstRoomID;
        RoomTwoID = secondRoomID;
        DoorObject = doorGameObject;
        DoorScript = DoorObject.GetComponent<Door>();
    }

    public bool OccupiesCords(Vector2Int firstPos, Vector2Int secondPos)
    {
        if ((CordsOne == firstPos && CordsTwo == secondPos) || (CordsOne == secondPos && CordsTwo == firstPos))
        {
            return true;
        }

        return false;
    }

    public bool IsOccupingDoorway(Vector2Int doorPosition, CompassDirection facingDirection)
    {
        doorPosition += GridCoordinates;

        if (CordsOne == doorPosition && CordsTwo == doorPosition + LevelGenerationUtil.GetCompassDirectionAsVector2Int(facingDirection))
        {
            return true;
        }
        else if (CordsTwo == doorPosition && CordsOne == doorPosition + LevelGenerationUtil.GetCompassDirectionAsVector2Int(facingDirection))
        {
            return true;
        }

        return false;
    }

    // public bool OccupiesCords(Vector2Int gridCords)
    // {
    //     if (cordsOne == gridCords || cordsTwo == gridCords)
    //     {
    //         return true;
    //     }

    //     return false;
    // }
}


public class DoorGenerator : SequenceBase
{
    public GameObject DoorPrefab; // will need to take variations

    private List<DoorData> doorCollection = new List<DoorData>();

    private LevelGenerator levelGenerator;

    private int doorUUID = 1;

    // public event Action OnDoorsGenerated;
    public override event Action OnThisSequenceEnd;

    private float currentProgress = 0;

    public event Action OnDoorToggled;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();

        // levelGenerator.onLevelGenerationComplete += OnLevelGenerated;
    }

    public void Initialize()
    {
        levelGenerator = GetComponent<LevelGenerator>();

        if (levelGenerator == null)
        {
            throw new NullReferenceException("LevelGenerator is null!");
        }
    }

    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.E))
    //     {
    //         int roomID = levelGenerator.GetRoomIDFromCoordinates(levelGenerator.GetGridCoordinates(PlayerRefFetcher.Instance.transform.position));

    //         ToggleDoors(roomID);
    //     }
    // }

    private void OnLevelGenerated()
    {
        List<SpawnedLevelRoomData> spawnedLevelRoomData = levelGenerator.GetAllSpawnedRoomData();

        Vector2Int cordA = Vector2Int.zero;
        Vector2Int cordB = Vector2Int.zero;
        float unitSize = levelGenerator.GetUnitSizeInMeters();
        float halfUnitSize = unitSize / 2f;
        bool flipFlop = false;

        int counter = 0;

        foreach (var roomData in spawnedLevelRoomData)
        {
            foreach (var doorway in roomData.DoorwayData)
            {
                cordA = roomData.GridCoordinates + doorway.Location;
                cordB = cordA + doorway.GetFacingAsVector(); // space after doorway.
                Vector2 doorOffset = doorway.GetFacingAsVector();
                doorOffset /= 2f; // half


                if (ContainsDoorWithCords(cordA, cordB)) continue; // we have a door here already.

                // get doorway position
                Vector3 doorSpawnPos = new Vector3(halfUnitSize, 0, halfUnitSize); // get the offset to the center of the room.
                doorSpawnPos += new Vector3(cordA.x * unitSize, 0, cordA.y * unitSize);
                doorSpawnPos += new Vector3(doorOffset.x, 0, doorOffset.y) * unitSize; // turn that into world space.



                GameObject doorObject = Instantiate(DoorPrefab, doorSpawnPos, LevelGenerationUtil.GetCompassDirectionAsQuaternion(doorway.FacingDirection));
                doorObject.transform.SetParent(roomData.GetRoomObject().transform);
                doorObject.name = "[" + doorUUID.ToString() + "] " + DoorPrefab.name;

                doorCollection.Add(new DoorData(doorUUID, cordA, cordB, roomData.ID, levelGenerator.GetRoomIDFromCoordinates(cordB), roomData.GridCoordinates, doorObject));

                doorObject.GetComponent<Door>().SetDoorState(flipFlop);
                flipFlop = !flipFlop;

                doorUUID++;
                // doorCollection.Add(new DoorData())
            }
            counter++;
            currentProgress = (float)counter / spawnedLevelRoomData.Count;
        }

        // OnDoorsGenerated?.Invoke();
        OnThisSequenceEnd?.Invoke();
        currentProgress = 1f;
    }


    private bool ContainsDoorWithCords(Vector2Int a, Vector2Int b)
    {
        if (a == b) throw new Exception("Can't have a door in one tile!");

        // will get slower the more doors there are.
        // TODO: fix ya brain, and fix this mess. More rooms = more slower.

        foreach (DoorData door in doorCollection)
        {
            if (door.OccupiesCords(a, b))
            {
                return true;
            }
        }

        return false;
    }

    // ? this was meant to replace or house the code in Onlevelgenerated but I forgor.
    // public void TryAndCreateDoor(SpawnedLevelRoomData data)
    // {
    //     Vector2Int doorCords = Vector2Int.zero;
    //     Vector2Int door2Cords = Vector2Int.zero;

    //     foreach (DoorwayData doorwayData in data.DoorwayData)
    //     {
    //         doorCords = data.GridCoordinates + doorwayData.Location;
    //         door2Cords = doorCords + doorwayData.GetFacingAsVector();
    //     }
    // }

    public void ToggleDoors(int roomID)
    {
        // this will get expsensive.
        foreach (DoorData door in doorCollection)
        {
            if (door.RoomOneID == roomID || door.RoomTwoID == roomID)
            {
                // toggle door.
                door.DoorScript.ToggleDoorState();
                OnDoorToggled?.Invoke();
            }
            else
            {
                continue;
            }
        }
    }

    public bool IsDoorOpenInDoorway(Vector2Int doorCoordinates, CompassDirection facingDirection)
    {
        foreach (DoorData door in doorCollection)
        {
            if (door.IsOccupingDoorway(doorCoordinates, facingDirection))
            {
                return door.DoorScript.IsDoorOpen();
            }
        }

        Debug.LogWarning("Failed to find door, returning false.");
        return false;
    }

    public void SetAllDoorsState(int roomID, bool state)
    {
        // this will get expsensive.
        foreach (DoorData door in doorCollection)
        {
            if (door.RoomOneID == roomID || door.RoomTwoID == roomID)
            {
                // toggle door.
                door.DoorScript.SetDoorState(state);
                OnDoorToggled?.Invoke();
            }
            else
            {
                continue;
            }
        }
    }

    public void SetDoorsOverride(int roomID, DoorOverrideState state)
    {
        // this will get expsensive.
        foreach (DoorData door in doorCollection)
        {
            if (door.RoomOneID == roomID || door.RoomTwoID == roomID)
            {
                // toggle door.
                door.DoorScript.SetOverrideState(state);
                OnDoorToggled?.Invoke();
            }
            else
            {
                continue;
            }
        }
    }

    public void ResetOverrideState(int roomID)
    {
        // this will get expsensive.
        foreach (DoorData door in doorCollection)
        {
            if (door.RoomOneID == roomID || door.RoomTwoID == roomID)
            {
                // toggle door.
                door.DoorScript.ResetOverrideState();
                OnDoorToggled?.Invoke(); // ? maybe not :/
            }
            else
            {
                continue;
            }
        }
    }

    public override void StartSequence()
    {
        currentProgress = 0f;
        Initialize();
        OnLevelGenerated();
    }

    public override float GetProgress()
    {
        return currentProgress;
    }
}
