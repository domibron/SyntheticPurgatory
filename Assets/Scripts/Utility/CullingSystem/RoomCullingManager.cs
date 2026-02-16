using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomCullingManager : SequenceBase
{
    // public int roomDepth = 2;

    Transform player;

    private LevelGenerator levelGenerator;
    private DoorGenerator doorGenerator;

    List<int> lastLoadedRooms = new List<int>();

    Vector2Int lastCoordinates = new Vector2Int();

    public override event Action OnThisSequenceEnd;

    private float currentProgress = 0f;

    // 0 is current room.

    private int radius = 4;

    private int mediumStart = 2;

    private int lowStart = 3;

    Vector2Int levelGridSize = Vector2Int.zero;

    void Awake()
    {
        // GetComponent<Sequencer>().OnSequencesEnd += SetUpCullingManager; // TODO: move into the sequencer.
        if (levelGenerator == null) levelGenerator = GetComponent<LevelGenerator>();
        if (doorGenerator == null) doorGenerator = GetComponent<DoorGenerator>();

        // doorGenerator.OnDoorToggled += DoorToggled;

    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // yield return null;

        // levelGenerator.GetComponent<LevelGenerator>(); // TODO: figure out why this if fucked up.


        // while (PlayerRefFetcher.Instance == null)
        // {
        //     yield return null;
        // }

        // player = PlayerRefFetcher.Instance.GetPlayerRef().transform;

        // UnloadAllRooms();

        // Vector2Int currentRoomCoordinates = levelGenerator.GetGridCoordinates(player.position);
        // lastCoordinates = currentRoomCoordinates;
        // UpdateRoomCulling(currentRoomCoordinates);


    }

    private void SetUpCullingManager()
    {
        currentProgress = 0f;
        player = PlayerRefFetcher.Instance.GetPlayerRef().transform;
        SetupAllRooms();
        currentProgress = 0.333f;
        UnloadAllRooms();
        currentProgress = 0.666f;


        Vector2Int currentRoomCoordinates = levelGenerator.GetGridCoordinates(player.position);
        lastCoordinates = currentRoomCoordinates;
        UpdateRoomCulling(currentRoomCoordinates);
        currentProgress = 1f;
        OnThisSequenceEnd?.Invoke();
    }



    // Update is called once per frame. // TODO: have a check to see if the player enters another room.
    void Update()
    {
        if (player == null) return;

        Vector2Int currentRoomCoordinates = levelGenerator.GetGridCoordinates(player.position);

        // some optimisation.
        if (lastCoordinates == currentRoomCoordinates) return;

        lastCoordinates = currentRoomCoordinates;

        UpdateRoomCulling(currentRoomCoordinates);

    }

    // private void DoorToggled()
    // {
    //     UpdateRoomCulling(lastCoordinates);
    // }

    void SetupAllRooms()
    {
        List<SpawnedLevelRoomData> spawnedLevelRoomDatas = levelGenerator.GetAllSpawnedRoomData();
        // print("list " + spawnedLevelRoomDatas.Count + "");
        foreach (SpawnedLevelRoomData spawnedLevelRoom in spawnedLevelRoomDatas)
        {
            spawnedLevelRoom.GetRoomObject().GetComponent<RoomCulling>().SetupRoomCulling();
        }
    }

    void UnloadAllRooms()
    {
        List<SpawnedLevelRoomData> spawnedLevelRoomDatas = levelGenerator.GetAllSpawnedRoomData();
        // print("list " + spawnedLevelRoomDatas.Count + "");
        foreach (SpawnedLevelRoomData spawnedLevelRoom in spawnedLevelRoomDatas)
        {
            spawnedLevelRoom.GetRoomObject().GetComponent<RoomCulling>().SetRendererState(VisibleState.Unload);
            spawnedLevelRoom.GetRoomObject().SetActive(false);
        }
    }


    private void UpdateRoomCulling(Vector2Int currentRoomCoordinates)
    {
        int xRoomPos = currentRoomCoordinates.x;
        int yRoomPos = currentRoomCoordinates.y;

        Vector2Int checkingPos = Vector2Int.zero;


        // This does the radius of rooms.
        List<int> ignoredIDs = new List<int>();
        List<int> highDetailRooms = new List<int>();
        List<int> mediumDetailRooms = new List<int>();
        List<int> lowDetailRooms = new List<int>();

        print("Trying to set render states");

        // * a attempt to fix the culling until i realized I could do x = 1 (radius) and 1 (radius) - x = y. Should then generate a circle.
        // for (int xRow = checkingPos.x - radius; xRow <= checkingPos.x + radius; xRow++)
        // {
        //     for (int yRow = checkingPos.y - radius; yRow <= checkingPos.y + radius; yRow++)
        //     {
        //         checkingPos = new Vector2Int(xRow, yRow);

        //         if (checkingPos.x < 0 || checkingPos.x >= levelGridSize.x) continue;
        //         if (checkingPos.y < 0 || checkingPos.y >= levelGridSize.y) continue;

        //         int roomID = levelGenerator.GetRoomIDFromCoordinates(checkingPos);
        //         if (roomID == LevelGenerator.BLANK_ID) continue;


        //         if (ignoredIDs.Contains(roomID)) continue;
        //         use xRoomPos please instead of checking pos.
        //         int distFromCenter = Mathf.Abs(xRow - checkingPos.x) + Mathf.Abs(yRow - checkingPos.y);

        //         if (distFromCenter < mediumStart)
        //         {
        //             // if (ignoredIDs.Contains(roomID)) continue;
        //             highDetailRooms.Add(roomID);
        //             ignoredIDs.Add(roomID);
        //         }
        //         else if (distFromCenter < lowStart)
        //         {
        //             // if (ignoredIDs.Contains(roomID)) continue;
        //             mediumDetailRooms.Add(roomID);
        //             ignoredIDs.Add(roomID);
        //         }
        //         else if (distFromCenter <= radius)
        //         {
        //             // if (ignoredIDs.Contains(roomID)) continue;
        //             lowDetailRooms.Add(roomID);
        //             ignoredIDs.Add(roomID);
        //         }
        //     }
        // }


        // row is X and col is Y, but because how arrays are, top left is 0,0 and this is a brain fuck.
        for (int row = -radius; row <= radius; row++)
        {
            checkingPos.x = xRoomPos + row;
            if (checkingPos.x < 0 || checkingPos.x >= levelGridSize.x) continue;

            for (int col = -radius; col <= radius; col++) // does check extra rooms we dont need, we could use for (int col = -(radius - row); col <= (radius - row); col++) instead
            {
                checkingPos.y = yRoomPos + col;
                if (checkingPos.y < 0 || checkingPos.y >= levelGridSize.y) continue;

                int roomID = levelGenerator.GetRoomIDFromCoordinates(checkingPos);
                if (roomID == LevelGenerator.BLANK_ID) continue;

                // if (ignoredIDs.Contains(roomID)) continue;

                int distFromCenter = Mathf.Abs(xRoomPos - checkingPos.x) + Mathf.Abs(yRoomPos - checkingPos.y);

                if (distFromCenter < mediumStart) // ? could flip for more performance? so low med then high?
                {
                    if (ignoredIDs.Contains(roomID) && mediumDetailRooms.Contains(roomID))
                    {
                        mediumDetailRooms.Remove(roomID);
                    }
                    else if (ignoredIDs.Contains(roomID) && lowDetailRooms.Contains(roomID))
                    {
                        lowDetailRooms.Remove(roomID);
                    }
                    else if (ignoredIDs.Contains(roomID)) continue;
                    else ignoredIDs.Add(roomID);

                    highDetailRooms.Add(roomID);
                }
                else if (distFromCenter < lowStart)
                {
                    if (ignoredIDs.Contains(roomID) && lowDetailRooms.Contains(roomID))
                    {
                        lowDetailRooms.Remove(roomID);
                    }
                    else if (ignoredIDs.Contains(roomID)) continue;
                    else ignoredIDs.Add(roomID);

                    mediumDetailRooms.Add(roomID);
                }
                else if (distFromCenter <= radius)
                {
                    if (ignoredIDs.Contains(roomID)) continue;
                    else ignoredIDs.Add(roomID);

                    lowDetailRooms.Add(roomID);
                }
            }
        }


        foreach (int roomID in highDetailRooms)
        {
            if (lastLoadedRooms.Contains(roomID))
                lastLoadedRooms.Remove(roomID);

            // SpawnedLevelRoomData roomData = levelGenerator.GetSpawnedLevelRoomData(roomID);

            GameObject roomObject = levelGenerator.GetRoomGameObjectFromID(roomID);
            roomObject?.SetActive(true);
            roomObject?.GetComponent<RoomCulling>()?.SetRendererState(VisibleState.Maximum);

        }

        foreach (int roomID in mediumDetailRooms)
        {
            if (lastLoadedRooms.Contains(roomID))
                lastLoadedRooms.Remove(roomID);

            // SpawnedLevelRoomData roomData = levelGenerator.GetSpawnedLevelRoomData(roomID);

            GameObject roomObject = levelGenerator.GetRoomGameObjectFromID(roomID);
            roomObject?.SetActive(true);
            roomObject?.GetComponent<RoomCulling>()?.SetRendererState(VisibleState.Medium);

        }

        foreach (int roomID in lowDetailRooms)
        {
            if (lastLoadedRooms.Contains(roomID))
                lastLoadedRooms.Remove(roomID);

            GameObject roomObject = levelGenerator.GetRoomGameObjectFromID(roomID);
            roomObject?.SetActive(true);
            roomObject?.GetComponent<RoomCulling>()?.SetRendererState(VisibleState.Minimal);
        }


        foreach (int roomID in lastLoadedRooms)
        {
            GameObject roomObject = levelGenerator.GetRoomGameObjectFromID(roomID);

            if (!roomObject.activeSelf) continue; // room already disabled.

            roomObject?.GetComponent<RoomCulling>()?.SetRendererState(VisibleState.Unload);
            roomObject?.SetActive(false);
        }

        lastLoadedRooms.Clear();

        // TODO: this is shitty, replace with remove range something. What? legacy comment?

        // lastLoadedRooms.Add(currentRoomID);
        lastLoadedRooms.AddRange(highDetailRooms);
        lastLoadedRooms.AddRange(mediumDetailRooms);
        lastLoadedRooms.AddRange(lowDetailRooms);

    }

    // TODO: should be in level generator.
    private static List<int> GetConnectingRoomIDs(SpawnedLevelRoomData currentRoom, LevelGenerator levelGenerator, out List<int> roomsWithClosedDoor, DoorGenerator doorGenerator = null, params int[] ignoredIDs)
    {

        roomsWithClosedDoor = new List<int>();

        int roomID = currentRoom.ID;

        Vector2Int doorCoordinates = Vector2Int.zero;

        List<int> roomIDs = new List<int>();

        foreach (var door in currentRoom.DoorwayData)
        {
            doorCoordinates = currentRoom.GridCoordinates + door.Location + door.GetFacingAsVector();

            int checkingRoomID = levelGenerator.GetRoomIDFromCoordinates(doorCoordinates);

            if (checkingRoomID == roomID) continue;

            if (ignoredIDs.Contains(checkingRoomID)) continue;

            if (doorGenerator != null)
            {
                // print("door state is: " + doorGenerator.IsDoorOpenInDoorway(door.Location, door.FacingDirection));
                if (!doorGenerator.IsDoorOpenInDoorway(door.Location, door.FacingDirection))
                {
                    roomsWithClosedDoor.Add(checkingRoomID);
                }
            }

            roomIDs.Add(checkingRoomID);
        }


        return roomIDs;
    }

    public override void StartSequence()
    {
        if (levelGenerator == null) levelGenerator = GetComponent<LevelGenerator>();
        // localLevelGrid = levelGenerator.GetLevelGrid();
        levelGridSize = levelGenerator.GetGridSize();
        SetUpCullingManager();
    }

    public override float GetProgress()
    {
        return currentProgress;
    }
}
