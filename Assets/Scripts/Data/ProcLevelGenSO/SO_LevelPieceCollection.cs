using System;
using UnityEngine;

/// <summary>
/// Rooms that have specified amounts to spawn and chance to spawn.
/// </summary>
[Serializable]
public class OneTimeSpawnRoom
{
    // public bool GranteeSpawn = false;

    /// <summary>
    /// Likely hood of this room being picked and spawning in.
    /// </summary>
    [Range(0, 100)]
    public float SpawnChance = 10f;

    /// <summary>
    /// The minimum amount of spawns for this room.
    /// </summary>
    [Min(0)]
    public int minAmountToSpawn = 1;

    /// <summary>
    /// The max amount of spawns for this room.
    /// </summary>
    [Min(0)]
    public int maxAmountToSpawn = 1;

    /// <summary>
    /// The data for the room.
    /// </summary>
    public SO_LevelPiece RoomPiece;
}

/// <summary>
/// Collection of all the rooms used in the level generation system.
/// </summary>
[CreateAssetMenu(fileName = "Level Piece Collection", menuName = "LevelGeneration/LevelPieceCollection")]
public class SO_LevelPieceCollection : ScriptableObject
{
    /*
    The way the level generation works simply is a branching style generation.
    It starts by placing the start room somewhere on the grid.
    Then checks to see if it can place regular or customised rooms.
    If it cannot place a room, it will then try to place a corridor piece.
    The corridor pieces act like a guarantee to place since it takes the smallest size possible. 1 by 1.
    This system does NOT spawn special rooms first then connecting them since there is more math behind doing that.
    There is more things the generation system does but this is a rough simplification about the rooms and prioritization.
    
    We also do not use exit rooms here. DO NOT create and use exit rooms, they will not spawn.
    The original system used to spawn both the start and exit and would generate a path to connect the both but was removed for our new version.
    
    If you want to use exit rooms, use end cap rooms instead.
    */

    /// <summary>
    /// The size of the grid in world space. Used for converting the grid into world and vice versa.
    /// </summary>
    [Min(0.0001f)]
    public float UnitSizeInMeters = 1f;

    /// <summary>
    /// The starting rooms for the player to spawn in. Only one is spawned.
    /// </summary>
    public SO_LevelPiece[] StartRooms;

    /// <summary>
    /// The exit rooms the player can use to exit. Only one is spawned.
    /// <br /><b>WE DO NOT USE THIS IN SYNTHETIC PURGATORY, IT IS A LABYRINTH.</b>
    /// </summary>
    public SO_LevelPiece[] ExitRooms;

    /// <summary>
    /// Rooms with customised spawn parameters.
    /// </summary>
    public OneTimeSpawnRoom[] CustomisedSpawnedRooms;

    /// <summary>
    /// Regular rooms that are not corridors.
    /// </summary>
    public SO_LevelPiece[] RegularRooms;

    /// <summary>
    /// Corridor pieces used to connect the rooms.
    /// </summary>
    public SO_LevelPiece[] Corridors;

    /// <summary>
    /// Rooms that cap a dead end where only one doorway is accessible.
    /// <br />Make sure to have at least a 1 by 1 end cap in this list.
    /// </summary>
    public SO_LevelPiece[] EndCapRooms;

    // * This is removed since Min(0) can do this.
    // void OnValidate()
    // {
    //     if (UnitSizeInMeters <= 0) UnitSizeInMeters = 1;
    // }
}
