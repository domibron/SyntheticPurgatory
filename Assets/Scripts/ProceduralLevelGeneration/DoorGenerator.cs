using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DoorData
{
    public Vector2Int cordsOne = Vector2Int.zero;
    public Vector2Int cordsTwo = Vector2Int.zero;
    public int id = -1;
    public int roomOneID = -1;
    public int roomTwoID = -1;
    public GameObject doorObject;
    public Door doorScript;

    public DoorData(int roomID, Vector2Int roomOneCords, Vector2Int roomTwoCords, int firstRoomID, int secondRoomID, GameObject doorGameObject)
    {
        cordsOne = roomOneCords;
        cordsTwo = roomTwoCords;
        id = roomID;
        roomOneID = firstRoomID;
        roomTwoID = secondRoomID;
        doorObject = doorGameObject;
        doorScript = doorObject.GetComponent<Door>();
    }

    public bool OccupiesCords(Vector2Int firstPos, Vector2Int secondPos)
    {
        if ((cordsOne == firstPos && cordsTwo == secondPos) || (cordsOne == secondPos && cordsTwo == firstPos))
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

    public event Action OnDoorsGenerated;
    public override event Action OnThisSequenceEnd;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        levelGenerator = GetComponent<LevelGenerator>();

        if (levelGenerator == null)
        {
            throw new NullReferenceException("LevelGenerator is null!");
        }

        // levelGenerator.onLevelGenerationComplete += OnLevelGenerated;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            int roomID = levelGenerator.GetRoomIDFromCoordinates(levelGenerator.GetGridCoordinates(PlayerRefFetcher.Instance.transform.position));

            ToggleDoors(roomID);
        }
    }

    private void OnLevelGenerated()
    {
        List<SpawnedLevelRoomData> spawnedLevelRoomData = levelGenerator.GetAllSpawnedRoomData();

        Vector2Int cordA = Vector2Int.zero;
        Vector2Int cordB = Vector2Int.zero;
        float unitSize = levelGenerator.GetUnitSizeInMeters();
        float halfUnitSize = unitSize / 2f;
        bool flipFlop = false;

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

                doorCollection.Add(new DoorData(doorUUID, cordA, cordB, roomData.ID, levelGenerator.GetRoomIDFromCoordinates(cordB), doorObject));

                doorObject.GetComponent<Door>().SetDoorState(flipFlop);
                flipFlop = !flipFlop;

                doorUUID++;
                // doorCollection.Add(new DoorData())
            }
        }

        OnDoorsGenerated?.Invoke();
        OnThisSequenceEnd?.Invoke();
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
            if (door.roomOneID == roomID || door.roomTwoID == roomID)
            {
                // toggle door.
                door.doorScript.ToggleDoorState();
            }
            else
            {
                continue;
            }
        }
    }

    public void SetAllDoorsState(int roomID, bool state)
    {
        // this will get expsensive.
        foreach (DoorData door in doorCollection)
        {
            if (door.roomOneID == roomID || door.roomTwoID == roomID)
            {
                // toggle door.
                door.doorScript.SetDoorState(state);
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
            if (door.roomOneID == roomID || door.roomTwoID == roomID)
            {
                // toggle door.
                door.doorScript.SetOverrideState(state);
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
            if (door.roomOneID == roomID || door.roomTwoID == roomID)
            {
                // toggle door.
                door.doorScript.ResetOverrideState();
            }
            else
            {
                continue;
            }
        }
    }

    public override void StartSequence()
    {
        OnLevelGenerated();
    }
}
