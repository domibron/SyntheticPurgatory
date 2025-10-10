using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;



public class LevelGenerator : MonoBehaviour
{
    public int Seed = -1;

    public const int X_ROW = 0;
    public const int Y_ROW = 1;

    [Min(0)]
    public int GridEdgeBuffer = 5;

    public int MinimumRegularRoomCount = 3;

    private int regularRoomCount = 0;

    [Min(1)]
    public int XSize = 10;
    [Min(1)]
    public int YSize = 10;

    public SO_LevelPieceCollection LevelPieceCollection;

    private int[,] levelGrid = new int[10, 10];

    private Dictionary<int, SpawnedLevelRoomData> levelData = new Dictionary<int, SpawnedLevelRoomData>();

    private int currentID = 1;

    private Coroutine RoomGenerationCoroutine;


    List<RoomPiece> allRepeatableRooms = new List<RoomPiece>();
    List<RoomPiece> allEndCaps = new List<RoomPiece>();
    List<RoomPiece> allExitRooms = new List<RoomPiece>();
    List<RoomPiece> startRooms = new List<RoomPiece>();

    public event Action onLevelGenerationComplete;
    public event Action onLevelGenerationStart;
    public event Action onLevelGenerationDestroy;

    public enum RoomSpawnStatus
    {
        Failed,
        DeadEnd,
        ExtendableRoom,
    }


    void Awake()
    {
        // clear lists before using in case of some strange behaviours.

        SetupRoomGeneration();
    }

    // Start is called before the first frame update
    void Start()
    {

        RoomGenerationCoroutine = StartCoroutine(StartLevelGeneration());

    }

    private IEnumerator StartLevelGeneration()
    {
        onLevelGenerationStart?.Invoke();

        levelData.Clear();
        currentID = 1;

        int seed = InitializeNewSeed();

        SpawnStartRoom(currentID);

        List<int> roomsWithEmptyDoorways = new List<int>();
        roomsWithEmptyDoorways.Add(currentID);

        currentID++;


        while (roomsWithEmptyDoorways.Count > 0)
        {
            int generatingRoomsFromID = roomsWithEmptyDoorways[Random.Range(0, roomsWithEmptyDoorways.Count)];
            List<int> roomIdsToAdd = SpawnConnectingRooms(generatingRoomsFromID);

            roomsWithEmptyDoorways.AddRange(roomIdsToAdd);
            roomsWithEmptyDoorways.Remove(generatingRoomsFromID);

            yield return null;
        }


        // if (CHECKING_CRITERIA)
        // {
        //     DestoryAllRooms();
        // }

        // clean up
        if (RoomGenerationCoroutine != null)
        {
            RoomGenerationCoroutine = null;
            print(GetGridAsString());
            print(seed.ToString());
            print("Done generating");
            onLevelGenerationComplete?.Invoke();
        }
    }

    /// <summary>
    /// Initilizes and populates all the room lists with all the room pieces and their rotatable variants.
    /// </summary>
    private void SetupRoomGeneration()
    {
        allRepeatableRooms.Clear();
        allEndCaps.Clear();
        allExitRooms.Clear();
        startRooms.Clear();

        AddAllRotatableRoomsToList(LevelPieceCollection.RegularRooms, ref allRepeatableRooms);
        AddAllRotatableRoomsToList(LevelPieceCollection.Corridors, ref allRepeatableRooms);


        AddAllRotatableRoomsToList(LevelPieceCollection.EndCapRooms, ref allEndCaps);

        AddAllRotatableRoomsToList(LevelPieceCollection.ExitRooms, ref allExitRooms);

        AddAllRotatableRoomsToList(LevelPieceCollection.StartRooms, ref startRooms); // we add to start rooms list
    }

    /// <summary>
    /// Removes all the room prefabs this level generator created. DO NOT remove other objects.
    /// </summary>
    private void DestoryAllRooms()
    {
        Transform[] children = transform.GetComponentsInChildren<Transform>();

        foreach (Transform child in children)
        {
            if (child == transform) continue;

            Destroy(child.gameObject);
        }

        onLevelGenerationDestroy?.Invoke();
    }

    /// <summary>
    /// Spawns connecting rooms
    /// </summary>
    /// <param name="roomsWithEmptyDoorways"></param>
    private List<int> SpawnConnectingRooms(int generatingRoomsFromID)
    {
        List<int> roomsWithDoorsToReturn = new List<int>();

        DoorwayData[] doorsToConnectTo = levelData[generatingRoomsFromID].DoorwayData;

        int doorsConnected = 0;
        foreach (var doorway in doorsToConnectTo)
        {
            Vector2Int vectorDistanceFromSpawn = (levelData[1].GridCoordinates - levelData[generatingRoomsFromID].GridCoordinates - (doorway.Location + doorway.GetFacingAsVector()));
            float magnitude = LevelGenerationUtil.GetMagnitudeOfVector2Int(vectorDistanceFromSpawn);




            if (!CanGenerateFurther(levelData[generatingRoomsFromID].GridCoordinates + doorway.Location + doorway.GetFacingAsVector()))
            {
                if (SpawnRoomOrEndCap(generatingRoomsFromID, doorway, currentID) != RoomSpawnStatus.Failed)
                {
                    roomsWithDoorsToReturn.Add(currentID);
                    currentID++;

                    doorsConnected++;
                }
            }
            else if (SpawnRoomOrEndCap(generatingRoomsFromID, doorway, currentID, false) != RoomSpawnStatus.Failed)
            {
                roomsWithDoorsToReturn.Add(currentID);
                currentID++;

                doorsConnected++;
            }



        }

        // print(generatingRoomsFromID + " " + doorsConnected + " " + doorsToConnectTo.Length);
        if (doorsConnected >= doorsToConnectTo.Length) roomsWithDoorsToReturn.Remove(generatingRoomsFromID); // remove the generatingRoomsFromID from list since we generated the doors for said room.
        else if (!CanGenerateASingleRoom(generatingRoomsFromID)) roomsWithDoorsToReturn.Remove(generatingRoomsFromID); // remove generatingRoomsFromID if we cant generate a room.

        return roomsWithDoorsToReturn; // added int list return not sure if above needs the orignial ref list. // * It seems to work just fine.

    }

    private int InitializeNewSeed()
    {
        levelGrid = new int[XSize + GridEdgeBuffer + GridEdgeBuffer, YSize + GridEdgeBuffer + GridEdgeBuffer];

        int timeStampSeed = (int)new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
        if (Seed != -1) timeStampSeed = Seed;
        // print("Current seed: " + timeStampSeed);
        Random.InitState(timeStampSeed);
        return timeStampSeed;
    }

    private void SpawnStartRoom(int idForStartRoom)
    {
        List<RoomPiece> availableStartRoomPieces = new List<RoomPiece>();

        Vector2Int startGridCords = new Vector2Int(Random.Range(GridEdgeBuffer - 1, levelGrid.GetLength(X_ROW) - GridEdgeBuffer), Random.Range(GridEdgeBuffer - 1, levelGrid.GetLength(Y_ROW) - GridEdgeBuffer));

        foreach (RoomPiece startRoom in startRooms)
        {
            if (CanPlacePiece(startGridCords, startRoom)) availableStartRoomPieces.Add(startRoom);
        }

        SpawnLevelPiece(new SpawnedLevelRoomData(idForStartRoom, startGridCords, availableStartRoomPieces[Random.Range(0, availableStartRoomPieces.Count)]));
    }

    private bool CanGenerateFurther(Vector2Int gridCoordinates)
    {
        int adjacentSpaces = 0;

        if (WithinBounds(gridCoordinates + Vector2Int.left))
        {

            if (levelGrid[(gridCoordinates + Vector2Int.left).x, (gridCoordinates + Vector2Int.left).y] == 0) adjacentSpaces++;
        }

        if (WithinBounds(gridCoordinates + Vector2Int.up))
        {

            if (levelGrid[(gridCoordinates + Vector2Int.up).x, (gridCoordinates + Vector2Int.up).y] == 0) adjacentSpaces++;
        }

        if (WithinBounds(gridCoordinates + Vector2Int.right))
        {

            if (levelGrid[(gridCoordinates + Vector2Int.right).x, (gridCoordinates + Vector2Int.right).y] == 0) adjacentSpaces++;
        }


        if (WithinBounds(gridCoordinates + Vector2Int.down))
        {

            if (levelGrid[(gridCoordinates + Vector2Int.down).x, (gridCoordinates + Vector2Int.down).y] == 0) adjacentSpaces++;
        }

        return adjacentSpaces > 0;
    }

    private bool WithinBounds(Vector2Int gridCoordinates)
    {
        return !(gridCoordinates.x < GridEdgeBuffer - 1 || gridCoordinates.x >= levelGrid.GetLength(X_ROW) - GridEdgeBuffer || gridCoordinates.y < GridEdgeBuffer - 1 || gridCoordinates.y >= levelGrid.GetLength(Y_ROW) - GridEdgeBuffer);
    }

    // TODO make into general pickAndSpawn func? maybe?
    private RoomSpawnStatus SpawnRoomOrEndCap(int generatingRoomsFromID, DoorwayData doorway, int thisRoomID, bool blockGeneration = true)
    {
        GetAllPossibleRoomsAndPositions(allRepeatableRooms, levelData[generatingRoomsFromID].GridCoordinates, doorway, out List<RoomPiece> allValidLevelPieces, out List<Vector2Int> gridSpawnLocation);

        List<RoomPiece> FinalRooms = new List<RoomPiece>();
        List<Vector2Int> FinalRoomsSpawnLocation = new List<Vector2Int>();

        if (!blockGeneration && allValidLevelPieces.Count > 0)
        {
            for (int i = 0; i < allValidLevelPieces.Count; i++)
            {
                if (CheckHowManyRoomsCanGenerateAfter(gridSpawnLocation[i], allValidLevelPieces[i]))
                {
                    FinalRooms.Add(allValidLevelPieces[i]);
                    FinalRoomsSpawnLocation.Add(new Vector2Int(gridSpawnLocation[i].x, gridSpawnLocation[i].y));
                }
            }
        }

        if (FinalRooms.Count > 0 && FinalRoomsSpawnLocation.Count > 0 && !blockGeneration)
        {
            int randomPieceIndex = Random.Range(0, FinalRooms.Count);

            // if (LevelPieceCollection.RegularRooms.Contains(allValidLevelPieces[randomPieceIndex].GetLevelPiece())) // check if the regular rooms has this piece
            // {
            //     regularRoomCount++;
            // }

            SpawnLevelPiece(new SpawnedLevelRoomData(currentID, FinalRoomsSpawnLocation[randomPieceIndex], FinalRooms[randomPieceIndex]));

            return RoomSpawnStatus.ExtendableRoom; // ! don't continue.

        }
        else if (allValidLevelPieces.Count > 0 && gridSpawnLocation.Count > 0 && blockGeneration)
        {
            int randomPieceIndex = Random.Range(0, allValidLevelPieces.Count);


            SpawnLevelPiece(new SpawnedLevelRoomData(currentID, gridSpawnLocation[randomPieceIndex], allValidLevelPieces[randomPieceIndex]));

            // roomsWithEmptyDoorways.Add(currentID);

            // currentID++;
            if (allEndCaps.Contains(allValidLevelPieces[randomPieceIndex]))
                return RoomSpawnStatus.DeadEnd;
            else
                return RoomSpawnStatus.ExtendableRoom; // ! don't continue.
        }


        GetAllPossibleRoomsAndPositions(allEndCaps, levelData[generatingRoomsFromID].GridCoordinates, doorway, out allValidLevelPieces, out gridSpawnLocation);

        if (allValidLevelPieces.Count > 0 && blockGeneration)
        {
            int randomPieceIndex = UnityEngine.Random.Range(0, allValidLevelPieces.Count);

            SpawnLevelPiece(new SpawnedLevelRoomData(currentID, gridSpawnLocation[randomPieceIndex], allValidLevelPieces[randomPieceIndex]));

            // roomsWithEmptyDoorways.Add(currentID);

            // currentID++;

            return RoomSpawnStatus.DeadEnd;
        }

        return RoomSpawnStatus.Failed;

    }

    /// <summary>
    /// Spawns the level piece into the world and calls appropriate functions. 
    /// </summary>
    /// <param name="levelPiece">The level piece data to spawn the prefab from.</param>
    /// <param name="gridSpawnLocation">The target location to spawn the level piece at.</param>
    /// <param name="id">The id to give this level piece.</param>
    /// <param name="orientation">The rotation of the level piece based of compass direction, north is 0 degrees, east is 90 and so on.</param>
    private void SpawnLevelPiece(SpawnedLevelRoomData levelRoomData)
    {
        GameObject nextRoom = Instantiate(levelRoomData.GetPrefab(), levelRoomData.GetSpawnPoint(), levelRoomData.Rotation);

        nextRoom.GetComponent<LevelRoomDataDisplay>()?.SetUpRoom(levelRoomData);

        nextRoom.transform.SetParent(transform, true);
        nextRoom.name = $"[{levelRoomData.ID}] {nextRoom.name}";

        UpdateLevelData(levelRoomData);
    }

    /// <summary>
    /// Generates two lists, one for the level pieces and the other for the spawn location for that piece. There can be duplicates level pieces with different spawn locations because of doors.
    /// </summary>
    /// <param name="allRoomsAvailable">All the rooms given to check.</param>
    /// <param name="roomCoordinates">The current room coordinates to align with.</param>
    /// <param name="doorwayToConnectTo">The doorway we want to find pieces to connect to.</param>
    /// <param name="allValidLevelPieces">All valid level pieces you can pick from.</param>
    /// <param name="allAssociatedSpawnLocations">All spawn locations associated with the level piece. (use the same index with the level piece for this)</param>
    private void GetAllPossibleRoomsAndPositions(List<RoomPiece> allRoomsAvailable, Vector2Int roomCoordinates, DoorwayData doorwayToConnectTo, out List<RoomPiece> allValidLevelPieces, out List<Vector2Int> allAssociatedSpawnLocations)
    {
        List<RoomPiece> allAvailableRooms = LevelGenerationUtil.GetAllValidLevelPieces(doorwayToConnectTo.FacingDirection, allRoomsAvailable);

        allValidLevelPieces = new List<RoomPiece>();
        allAssociatedSpawnLocations = new List<Vector2Int>();
        foreach (var roomPiece in allAvailableRooms)
        {
            List<DoorwayData> allValidDoorways = roomPiece.GetDoorwaysFacingThatDirection(LevelGenerationUtil.GetOppositeDirection(doorwayToConnectTo.FacingDirection));

            foreach (var targetDoorway in allValidDoorways)
            {
                Vector2Int gridLocation = roomCoordinates + doorwayToConnectTo.Location + doorwayToConnectTo.GetFacingAsVector() - targetDoorway.Location;
                if (CanPlacePiece(gridLocation, roomPiece))
                {
                    allValidLevelPieces.Add(roomPiece);
                    allAssociatedSpawnLocations.Add(gridLocation);
                }
            }
        }
    }

    /// <summary>
    /// Updates the level grid and level data with the new piece.
    /// </summary>
    /// <param name="gridCoordinates">The spawn location for the piece.</param>
    /// <param name="levelPiece">The level piece data.</param>
    /// <param name="idAssociatedWithPiece">The id to assign to the newly spawned level piece.</param>
    /// <param name="orientationOfPiece">The rotation of the level piece based of compass direction, north is 0 degrees, east is 90 and so on.</param>
    private void UpdateLevelData(SpawnedLevelRoomData levelRoomData)
    {
        UpdateGridWithPiece(levelRoomData);

        levelData.Add(currentID, levelRoomData);
    }

    /// <summary>
    /// Updates the level grid with the level piece with the id associated with it.
    /// </summary>
    /// <param name="gridCoordinates">The spawn location for the level piece.</param>
    /// <param name="levelPiece">The level piece data.</param>
    /// <param name="idAssociatedWithPiece">The id to assign to the grid.</param>
    private void UpdateGridWithPiece(SpawnedLevelRoomData levelRoomData)
    {
        for (int x = 0; x < levelRoomData.BoundingSize.x; x++)
        {
            for (int y = 0; y < levelRoomData.BoundingSize.y; y++)
            {
                levelGrid[levelRoomData.GridCoordinates.x + x, levelRoomData.GridCoordinates.y + y] = levelRoomData.ID;
            }
        }
    }

    /// <summary>
    /// Generate the level grid as the readable string, mainly used for debugging.
    /// </summary>
    /// <returns>The level grid as a grid of numbers.</returns>
    private string GetGridAsString()
    {
        string returnString = "";

        // due to how the int 2d array is accessed, reading the data to a viewable and understandable format requires a little manoeuvring.
        // x is the rows while the y is the column.

        // we read the reverse of the 2d array column.
        for (int y = levelGrid.GetLength(Y_ROW) - 1; y >= 0; y--)
        {
            // then read the row of the 2d array column.
            for (int x = 0; x < levelGrid.GetLength(X_ROW); x++)
            {
                returnString += levelGrid[x, y] + " ";
            }
            returnString += "\n";
        }

        // the grid is now oriented in the correct way. 0, 0 is bottom left rather than top left.

        return returnString;
    }

    /// <summary>
    /// Checks if the level piece can be placed on the target grid location.
    /// </summary>
    /// <param name="gridCoordinates">The target spawn location.</param>
    /// <param name="levelPiece">The level piece to spawn data.</param>
    /// <returns><b>True</b> if the level piece can be placed.</returns>
    private bool CanPlacePiece(Vector2Int gridCoordinates, RoomPiece levelPiece)
    {
        // This function calls multiple checking functions for ease.
        return CanFitAtSpot(gridCoordinates, levelPiece) && CanGenerateRoomsAfterPlacement(gridCoordinates, levelPiece) && RoomDoesNotBlockDoorways(gridCoordinates, levelPiece);
    }

    /// <summary>
    /// Checks if the level piece overlaps any existing rooms or out of bounds.
    /// </summary>
    /// <param name="gridCoordinates">The target spawn location.</param>
    /// <param name="levelPiece">The level piece to spawn data.</param>
    /// <returns><b>True</b> if the placement wont overlap any rooms or exceed the bounds of the grid.</returns>
    private bool CanFitAtSpot(Vector2Int gridCoordinates, RoomPiece levelPiece)
    {
        // check if the lower left corner is out of bounds of the grid.
        if (gridCoordinates.x < 0 || gridCoordinates.y < 0 || gridCoordinates.x >= levelGrid.GetLength(X_ROW) || gridCoordinates.y >= levelGrid.GetLength(Y_ROW))
        {
            return false; // we are outside of the grid.
        }

        // get the maximum bounds of the shape based of bounds. (a cube with 0, 0 coordinates + bounds of 1,1 will check a 2x2 area, we remove 1,1 to correct for this)
        Vector2Int boundingSizeAfterCorrection = levelPiece.BoundingSize - Vector2Int.one;

        // check the maximum bounds exceeds the grid.
        if (gridCoordinates.x + boundingSizeAfterCorrection.x >= levelGrid.GetLength(X_ROW) || gridCoordinates.y + boundingSizeAfterCorrection.y >= levelGrid.GetLength(Y_ROW))
        {
            return false; // we are outside of the grid.
        }

        // check within the room bounds for any existing rooms.
        for (int x = 0; x < levelPiece.BoundingSize.x; x++)
        {
            for (int y = 0; y < levelPiece.BoundingSize.y; y++)
            {
                if (levelGrid[gridCoordinates.x + x, gridCoordinates.y + y] != 0) return false; // there is a room within the bounds.
            }
        }

        // no issues founds, we are good to continue.
        return true;
    }

    /// <summary>
    /// Checks to see if the level piece can generate rooms after placement.
    /// </summary>
    /// <param name="gridCoordinates">The target spawn location.</param>
    /// <param name="levelPiece">The level piece to spawn data.</param>
    /// <returns><b>True</b> if the doorways can generate rooms or connect to existing rooms.</returns>
    private bool CanGenerateRoomsAfterPlacement(Vector2Int gridCoordinates, RoomPiece levelPiece)
    {
        // we presume all doors are valid.
        bool doorsAreValid = true;

        // cycle through all our doors.
        foreach (var door in levelPiece.DoorwayData)
        {
            // get the space just after the door.
            Vector2Int spaceAfterDoor = gridCoordinates + door.Location + door.GetFacingAsVector();

            // check if the space after the door exceeds the grid bounds / buffer.
            if (spaceAfterDoor.x < GridEdgeBuffer - 1 || spaceAfterDoor.x >= levelGrid.GetLength(X_ROW) - GridEdgeBuffer || spaceAfterDoor.y < GridEdgeBuffer - 1 || spaceAfterDoor.y >= levelGrid.GetLength(Y_ROW) - GridEdgeBuffer)
            {
                // we know that we cant continue to generated after this room because it goes out of bounds.
                doorsAreValid = false;
                break; // since its invalid we can just exit out of the loop.
            }

            // get the id of the space after the door.
            int connectingRoomID = levelGrid[spaceAfterDoor.x, spaceAfterDoor.y];

            // we then check if the space after the door is a room. if so, we check if we can connect to it.
            if (connectingRoomID != 0)
            {
                // get the data with that corresponding id.
                SpawnedLevelRoomData checkingConnectedRoom = levelData[connectingRoomID];

                // we presume we cannot connect to the room.
                bool canConnectToRoom = false;

                // cycle through the connecting room if there is one door that connects to this piece.
                foreach (DoorwayData connectingRoomDoor in checkingConnectedRoom.DoorwayData)
                {
                    if (checkingConnectedRoom.GridCoordinates + connectingRoomDoor.Location == spaceAfterDoor && LevelGenerationUtil.GetOppositeDirection(door.FacingDirection) == connectingRoomDoor.FacingDirection)
                    {
                        // we can connect to the room so we mark this as success.
                        canConnectToRoom = true;
                        break;
                    }
                }

                // if we cannot connect to the room then its a invalid piece since the doorway will lead to a wall.
                if (!canConnectToRoom)
                {
                    doorsAreValid = false;
                }
            }

        }

        return doorsAreValid;
    }

    /// <summary>
    /// Checks to see if the room blocks any existing doorways.
    /// </summary>
    /// <param name="gridCoordinates">The target spawn location.</param>
    /// <param name="levelPiece">The level piece to spawn data.</param>
    /// <returns><b>True</b> if the room does not block or connect to any existing doorways.</returns>
    private bool RoomDoesNotBlockDoorways(Vector2Int gridCoordinates, RoomPiece levelPiece)
    {
        Vector2Int minBounds = gridCoordinates;
        Vector2Int maxBounds = gridCoordinates + levelPiece.BoundingSize - Vector2Int.one;

        bool goingToOverlapAnyDoors = false;

        // checking top row
        for (int x = minBounds.x; x <= maxBounds.x; x++)
        {
            int yLevel = maxBounds.y + 1;

            if (yLevel >= levelGrid.GetLength(Y_ROW)) break; // null space of row.

            if (x < 0 || x >= levelGrid.GetLength(X_ROW)) continue; // null space of cell.

            // start of checking func.
            int connectingRoomID = levelGrid[x, yLevel];

            if (connectingRoomID == 0) continue;

            //LevelRoomData roomData = levelData[connectingRoomID];

            if (CheckForOverlappingDoorways(new Vector2Int(x, yLevel), gridCoordinates, levelPiece, CompassDirection.North))
            {
                // print("Going to overlap top row set of doors");
                goingToOverlapAnyDoors = true;
            }

        }

        // checking bottom row
        for (int x = minBounds.x; x <= maxBounds.x; x++)
        {
            int yLevel = minBounds.y - 1;

            if (yLevel < 0) break; // null space of row.

            if (x < 0 || x >= levelGrid.GetLength(X_ROW)) continue; // null space of cell.

            // start of checking func.
            int connectingRoomID = levelGrid[x, yLevel];

            if (connectingRoomID == 0) continue;

            //LevelRoomData roomData = levelData[connectingRoomID];

            if (CheckForOverlappingDoorways(new Vector2Int(x, yLevel), gridCoordinates, levelPiece, CompassDirection.South))
            {
                // print("Going to overlap bottom row set of doors");
                goingToOverlapAnyDoors = true;
            }

        }

        // checking right side
        for (int y = minBounds.y; y <= maxBounds.y; y++)
        {
            int xLevel = maxBounds.x + 1;

            if (xLevel >= levelGrid.GetLength(X_ROW)) break; // null space of row.

            if (y < 0 || y >= levelGrid.GetLength(Y_ROW)) continue; // null space of cell.

            // start of checking func.
            int connectingRoomID = levelGrid[xLevel, y];

            if (connectingRoomID == 0) continue;

            //LevelRoomData levelRoomData = levelData[connectingRoomID];

            if (CheckForOverlappingDoorways(new Vector2Int(xLevel, y), gridCoordinates, levelPiece, CompassDirection.East))
            {
                // print("Going to overlap right row set of doors");
                goingToOverlapAnyDoors = true;
            }

        }

        // checking left side
        for (int y = minBounds.y; y <= maxBounds.y; y++)
        {
            int xLevel = minBounds.x - 1;

            if (xLevel < 0) break; // null space of row.

            if (y < 0 || y >= levelGrid.GetLength(Y_ROW)) continue; // null space of cell.

            // start of checking func.
            int connectingRoomID = levelGrid[xLevel, y];

            if (connectingRoomID == 0) continue;

            //LevelRoomData roomData = levelData[connectingRoomID];

            if (CheckForOverlappingDoorways(new Vector2Int(xLevel, y), gridCoordinates, levelPiece, CompassDirection.West))
            {
                // print("Going to overlap left row set of doors");
                goingToOverlapAnyDoors = true;
            }

        }

        return !goingToOverlapAnyDoors;
    }

    /// <summary>
    /// Checks if the level piece is going to overlap any doorways without connecting to them.
    /// </summary>
    /// <param name="targetGridToCheck">The target grid location to check for overlap.</param>
    /// <param name="gridCoordinates">The spawn location of the level piece.</param>
    /// <param name="levelPiece">The level piece to spawn data.</param>
    /// <param name="directionToCheck">The direction of the doorway to check for connection.</param>
    /// <returns><b>True</b> if the piece overlaps a existing doorway.</returns>
    private bool CheckForOverlappingDoorways(Vector2Int targetGridToCheck, Vector2Int gridCoordinates, RoomPiece levelPiece, CompassDirection directionToCheck)
    {
        SpawnedLevelRoomData existingRoomData = levelData[levelGrid[targetGridToCheck.x, targetGridToCheck.y]];

        if (!existingRoomData.HasAnyDoorwayWithFacingDirection(LevelGenerationUtil.GetOppositeDirection(directionToCheck))) return false; // they don't have any doors we need to worry about.

        List<DoorwayData> thisPieceDoorways = levelPiece.GetDoorwaysFacingThatDirection(directionToCheck);
        List<DoorwayData> theRoomDoorways = existingRoomData.GetDoorwaysFacingThatDirection(LevelGenerationUtil.GetOppositeDirection(directionToCheck));

        DoorwayData doorwayToConnectTo = null;

        foreach (DoorwayData roomDoorwayData in theRoomDoorways)
        {
            if (existingRoomData.GridCoordinates + roomDoorwayData.Location == targetGridToCheck)
            {
                doorwayToConnectTo = roomDoorwayData;
            }
        }

        if (doorwayToConnectTo == null) return false; // there is no doorway to connect to.

        if (!levelPiece.HasAnyDoorwayWithFacingDirection(directionToCheck))
        {
            // print("We have no compatible doors");
            return true; // we have no compatible doors.
        }


        Vector2Int theirDoorwayPosition = existingRoomData.GridCoordinates + doorwayToConnectTo.Location + doorwayToConnectTo.GetFacingAsVector();

        bool hasAConnectingDoor = false;

        foreach (DoorwayData ourDoorway in thisPieceDoorways)
        {
            Vector2Int targetDoorLocationToMatchThisDoor = gridCoordinates + ourDoorway.Location;

            if (targetDoorLocationToMatchThisDoor == theirDoorwayPosition)
            {
                hasAConnectingDoor = true;
            }
        }

        if (!hasAConnectingDoor)
        {
            // we cannot find a compatible doorway for their doorway.
            return true;
        }


        // there is no issue in placing this room down.
        return false;
    }

    private bool CheckHowManyRoomsCanGenerateAfter(Vector2Int gridCoordinates, RoomPiece levelPiece, int minRoomAmountToSucceed = 1)
    {
        // we presume all doors are valid.
        int howManyRoomsCanGenerate = 0;

        // cycle through all our doors.
        foreach (var door in levelPiece.DoorwayData)
        {
            // get the space just after the door.
            Vector2Int spaceAfterDoor = gridCoordinates + door.Location + door.GetFacingAsVector();

            // check if the space after the door exceeds the grid bounds / buffer. // TODO may remove buffer to allow generating in buffer but placement check will cock block us.
            if (spaceAfterDoor.x < GridEdgeBuffer - 1 || spaceAfterDoor.x >= levelGrid.GetLength(X_ROW) - GridEdgeBuffer || spaceAfterDoor.y < GridEdgeBuffer - 1 || spaceAfterDoor.y >= levelGrid.GetLength(Y_ROW) - GridEdgeBuffer)
            {
                // we know that we cant continue to generated after this room because it goes out of bounds.
                continue; // since its invalid we can just exit out of the loop.
            }

            // get the id of the space after the door.
            int connectingRoomID = levelGrid[spaceAfterDoor.x, spaceAfterDoor.y];

            // we then check if the space after the door is a room. if so, we check if we can connect to it.
            if (connectingRoomID == 0)
            {
                howManyRoomsCanGenerate++;
            }

        }

        // print($"{levelPiece.GetLevelPiece().name} {howManyRoomsCanGenerate}");

        return howManyRoomsCanGenerate >= minRoomAmountToSucceed;
    }

    private bool CanGenerateASingleRoom(int levelRoomPieceID, int minDoorwayCountToSucceed = 1)
    {
        SpawnedLevelRoomData roomData = levelData[levelRoomPieceID];

        int spacesAfterDoorways = 0;

        foreach (var doorway in roomData.DoorwayData)
        {
            Vector2Int checkingLocation = roomData.GridCoordinates + doorway.Location + doorway.GetFacingAsVector();

            if (levelGrid[checkingLocation.x, checkingLocation.y] == 0)
            {
                spacesAfterDoorways++;
            }
        }

        return spacesAfterDoorways >= minDoorwayCountToSucceed;
    }

    private void AddAllRotatableRoomsToList(SO_LevelPiece[] levelPieces, ref List<RoomPiece> roomPieces)
    {
        List<RoomPiece> rooms = new List<RoomPiece>();

        foreach (var levelPiece in levelPieces)
        {
            // we need to get all four rotations.
            for (int i = 0; i <= 3; i++)
            {
                RoomPiece room = new RoomPiece(levelPiece, 90 * i, LevelPieceCollection.UnitSizeInMeters);
                // print(HasUniqueDoorways(room, rooms));
                if (HasUniqueDoorways(room, rooms))
                {
                    rooms.Add(room);
                    // roomPieces.Add(room);
                }
            }
        }

        roomPieces.AddRange(rooms);
    }

    private bool HasUniqueDoorways(RoomPiece targetPiece, List<RoomPiece> roomPieces)
    {
        if (roomPieces.Count <= 0) return true;

        bool isUnique = false;

        foreach (RoomPiece roomPiece in roomPieces)
        {
            bool hasSameDoorway = true;

            foreach (DoorwayData doorway in targetPiece.DoorwayData)
            {
                if (!roomPiece.DoorwayData.Contains(doorway))
                {
                    hasSameDoorway = false;
                }
            }

            if (!hasSameDoorway)
            {
                isUnique = true;
            }
        }

        return isUnique;
    }


}
