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

    List<int> lastLoadedRooms = new List<int>();

    Vector2Int lastCoordinates = new Vector2Int();

    public override event Action OnThisSequenceEnd;

    private float currentProgress = 0f;

    // 0 is current room.

    private int radius = 4;

    private int mediumStart = 3;

    private int lowStart = 4;

    Vector2Int levelGridSize = Vector2Int.zero;

    void Awake()
    {
        // GetComponent<Sequencer>().OnSequencesEnd += SetUpCullingManager; // TODO: move into the sequencer.
        if (levelGenerator == null) levelGenerator = GetComponent<LevelGenerator>();
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

    // private void UpdateRoomCulling(Vector2Int currentRoomCoordinates)
    // {
    //     int currentRoomID = levelGenerator.GetRoomIDFromCoordinates(currentRoomCoordinates);

    //     SpawnedLevelRoomData currentRoomData = levelGenerator.GetSpawnedLevelRoomData(currentRoomID);

    //     if (currentRoomData == null)
    //     {
    //         throw new NullReferenceException("Room data was null");
    //     }

    //     List<int> ignoredIDs = new List<int> { currentRoomID };

    //     // 1st ring around the rooms.
    //     List<int> firstLayerRoomIDs = GetConnectingRoomIDs(currentRoomData, levelGenerator);

    //     ignoredIDs.AddRange(firstLayerRoomIDs);

    //     List<int> secondLayerRoomIDs = new List<int>();

    //     foreach (int firstLayerRoomID in firstLayerRoomIDs)
    //     {
    //         List<int> secondLayerRoomPartial = GetConnectingRoomIDs(levelGenerator.GetSpawnedLevelRoomData(firstLayerRoomID), levelGenerator, ignoredIDs.ToArray());

    //         secondLayerRoomIDs.AddRange(secondLayerRoomPartial);

    //         ignoredIDs.AddRange(secondLayerRoomPartial);
    //     }

    //     if (lastLoadedRooms.Contains(currentRoomID))
    //     {
    //         lastLoadedRooms.Remove(currentRoomID);
    //     }

    //     currentRoomData.GetRoomObject().GetComponent<RoomCulling>().SetRendererState(VisibleState.Maximum);

    //     foreach (int roomID in firstLayerRoomIDs)
    //     {
    //         if (lastLoadedRooms.Contains(roomID))
    //             lastLoadedRooms.Remove(roomID);

    //         SpawnedLevelRoomData roomData = levelGenerator.GetSpawnedLevelRoomData(roomID);

    //         roomData.GetRoomObject()?.GetComponent<RoomCulling>()?.SetRendererState(VisibleState.Medium);
    //     }

    //     foreach (int roomID in secondLayerRoomIDs)
    //     {
    //         if (lastLoadedRooms.Contains(roomID))
    //             lastLoadedRooms.Remove(roomID);

    //         SpawnedLevelRoomData roomData = levelGenerator.GetSpawnedLevelRoomData(roomID);

    //         roomData.GetRoomObject()?.GetComponent<RoomCulling>()?.SetRendererState(VisibleState.Minimal);
    //     }

    //     foreach (int roomID in lastLoadedRooms)
    //     {
    //         SpawnedLevelRoomData roomData = levelGenerator.GetSpawnedLevelRoomData(roomID);

    //         roomData.GetRoomObject()?.GetComponent<RoomCulling>()?.SetRendererState(VisibleState.Unload);
    //     }

    //     lastLoadedRooms.Clear();

    //     // TODO: this is shitty, replace with remove range something.

    //     lastLoadedRooms.Add(currentRoomID);
    //     lastLoadedRooms.AddRange(firstLayerRoomIDs);
    //     lastLoadedRooms.AddRange(secondLayerRoomIDs);
    // }

    private void UpdateRoomCulling(Vector2Int currentRoomCoordinates)
    {
        int xPos = currentRoomCoordinates.x;
        int yPos = currentRoomCoordinates.y;

        Vector2Int checkingPos = Vector2Int.zero;

        List<int> ignoredIDs = new List<int>();
        List<int> highDetailRooms = new List<int>();
        List<int> mediumDetailRooms = new List<int>();
        List<int> lowDetailRooms = new List<int>();

        print("Trying to set render states");

        // row is X and col is Y, but because how arrays are, top left is 0,0 and this is a brain fuck.
        for (int row = -radius; row <= radius; row++)
        {
            checkingPos.x = xPos + row;
            if (checkingPos.x < 0 || checkingPos.x >= levelGridSize.x) continue;

            for (int col = -radius; col <= radius; col++)
            {
                checkingPos.y = yPos + col;
                if (checkingPos.y < 0 || checkingPos.y >= levelGridSize.y) continue;

                int roomID = levelGenerator.GetRoomIDFromCoordinates(checkingPos);
                if (roomID == LevelGenerator.BLANK_ID) continue;

                if (Mathf.Abs(row) + Mathf.Abs(col) < mediumStart)
                {
                    if (ignoredIDs.Contains(roomID)) continue;
                    highDetailRooms.Add(roomID);
                    ignoredIDs.Add(roomID);
                }
                else if (Mathf.Abs(row) + Mathf.Abs(col) < lowStart)
                {
                    if (ignoredIDs.Contains(roomID)) continue;
                    mediumDetailRooms.Add(roomID);
                    ignoredIDs.Add(roomID);
                }
                else if (Mathf.Abs(row) + Mathf.Abs(col) <= radius)
                {
                    if (ignoredIDs.Contains(roomID)) continue;
                    lowDetailRooms.Add(roomID);
                    ignoredIDs.Add(roomID);
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

        // TODO: this is shitty, replace with remove range something.

        // lastLoadedRooms.Add(currentRoomID);
        lastLoadedRooms.AddRange(highDetailRooms);
        lastLoadedRooms.AddRange(mediumDetailRooms);
        lastLoadedRooms.AddRange(lowDetailRooms);

    }

    // TODO: should be in level generator.
    private static List<int> GetConnectingRoomIDs(SpawnedLevelRoomData currentRoom, LevelGenerator levelGenerator, params int[] ignoredIDs)
    {
        int roomID = currentRoom.ID;

        Vector2Int doorCoordinates = Vector2Int.zero;

        List<int> roomIDs = new List<int>();

        foreach (var door in currentRoom.DoorwayData)
        {
            doorCoordinates = currentRoom.GridCoordinates + door.Location + door.GetFacingAsVector();

            int checkingRoomID = levelGenerator.GetRoomIDFromCoordinates(doorCoordinates);

            if (checkingRoomID == roomID) continue;

            if (ignoredIDs.Contains(checkingRoomID)) continue;

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
