using System;
using UnityEngine;


/// <summary>
/// Data for a room used by the procedural level generation to retrieve information about the room, doorways and any other additional core information.
/// </summary>
[CreateAssetMenu(fileName = "Level Piece", menuName = "LevelGeneration/LevelPiece")]
public class SO_LevelPiece : ScriptableObject
{

    /// <summary>
    /// The max size of the room.
    /// </summary>
    public Vector2Int BoundingSize { get => boundingSize; }

    [SerializeField]
    private Vector2Int boundingSize = Vector2Int.one;

    /// <summary>
    /// The doorway data in the room. (Connection points to other rooms).
    /// </summary>
    public DoorwayData[] DoorwayData { get => doorwayData; }

    [SerializeField]
    private DoorwayData[] doorwayData;

    /// <summary>
    /// The room game object to spawn.
    /// </summary>
    public GameObject LevelPiecePrefab { get => levelPiecePrefab; }

    [SerializeField]
    private GameObject levelPiecePrefab;

    // TODO maybe add a func to get doorway data array iirc it still passes the array ref rather than a copy. need a func to copy.

    // ? I think this was moved somewhere else.
    // public bool GranteeSpawn = false;

    // [Range(0f, 100f)]
    // public float SpawnChance = 10f;


}
