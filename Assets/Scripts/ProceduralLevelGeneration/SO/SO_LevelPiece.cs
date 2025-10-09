using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "Level Piece", menuName = "LevelGeneration/LevelPiece")]
public class SO_LevelPiece : ScriptableObject
{

    public Vector2Int BoundingSize { get => boundingSize; }

    [SerializeField]
    private Vector2Int boundingSize = Vector2Int.one;


    public DoorwayData[] DoorwayData { get => doorwayData; }

    [SerializeField]
    private DoorwayData[] doorwayData;

    public GameObject LevelPiecePrefab { get => levelPiecePrefab; }

    [SerializeField]
    private GameObject levelPiecePrefab;

    /// TODO maybe add a func to get doorway data array iirc it still passes the array ref rather than a copy. need a func to copy.

    // public bool GranteeSpawn = false;

    // [Range(0f, 100f)]
    // public float SpawnChance = 10f;


}
