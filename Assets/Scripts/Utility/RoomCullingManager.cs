using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomCullingManager : MonoBehaviour
{
    // public int roomDepth = 2;

    Transform player;

    public LevelGenerator levelGenerator;

    List<int> lastLoadedRooms = new List<int>();

    Vector2Int lastCoordinates = new Vector2Int();

    void Awake()
    {
        GetComponent<Sequencer>().OnSequencesEnd += SetUpCullingManager;
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
        player = PlayerRefFetcher.Instance.GetPlayerRef().transform;
        SetupAllRooms();
        UnloadAllRooms();

        Vector2Int currentRoomCoordinates = levelGenerator.GetGridCoordinates(player.position);
        lastCoordinates = currentRoomCoordinates;
        UpdateRoomCulling(currentRoomCoordinates);
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
        }
    }

    private void UpdateRoomCulling(Vector2Int currentRoomCoordinates)
    {
        int currentRoomID = levelGenerator.GetRoomIDFromCoordinates(currentRoomCoordinates);

        SpawnedLevelRoomData currentRoomData = levelGenerator.GetSpawnedLevelRoomData(currentRoomID);

        if (currentRoomData == null)
        {
            throw new NullReferenceException("Room data was null");
        }

        List<int> ignoredIDs = new List<int> { currentRoomID };

        // 1st ring around the rooms.
        List<int> firstLayerRoomIDs = GetConnectingRoomIDs(currentRoomData, levelGenerator);

        ignoredIDs.AddRange(firstLayerRoomIDs);

        List<int> secondLayerRoomIDs = new List<int>();

        foreach (int firstLayerRoomID in firstLayerRoomIDs)
        {
            List<int> secondLayerRoomPartial = GetConnectingRoomIDs(levelGenerator.GetSpawnedLevelRoomData(firstLayerRoomID), levelGenerator, ignoredIDs.ToArray());

            secondLayerRoomIDs.AddRange(secondLayerRoomPartial);

            ignoredIDs.AddRange(secondLayerRoomPartial);
        }

        if (lastLoadedRooms.Contains(currentRoomID))
        {
            lastLoadedRooms.Remove(currentRoomID);
        }

        currentRoomData.GetRoomObject().GetComponent<RoomCulling>().SetRendererState(VisibleState.Maximum);

        foreach (int roomID in firstLayerRoomIDs)
        {
            if (lastLoadedRooms.Contains(roomID))
                lastLoadedRooms.Remove(roomID);

            SpawnedLevelRoomData roomData = levelGenerator.GetSpawnedLevelRoomData(roomID);

            roomData.GetRoomObject()?.GetComponent<RoomCulling>()?.SetRendererState(VisibleState.Medium);
        }

        foreach (int roomID in secondLayerRoomIDs)
        {
            if (lastLoadedRooms.Contains(roomID))
                lastLoadedRooms.Remove(roomID);

            SpawnedLevelRoomData roomData = levelGenerator.GetSpawnedLevelRoomData(roomID);

            roomData.GetRoomObject()?.GetComponent<RoomCulling>()?.SetRendererState(VisibleState.Minimal);
        }

        foreach (int roomID in lastLoadedRooms)
        {
            SpawnedLevelRoomData roomData = levelGenerator.GetSpawnedLevelRoomData(roomID);

            roomData.GetRoomObject()?.GetComponent<RoomCulling>()?.SetRendererState(VisibleState.Unload);
        }

        lastLoadedRooms.Clear();

        // TODO: this is shitty, replace with remove range something.

        lastLoadedRooms.Add(currentRoomID);
        lastLoadedRooms.AddRange(firstLayerRoomIDs);
        lastLoadedRooms.AddRange(secondLayerRoomIDs);
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
}
