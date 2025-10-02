using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class OneTimeSpawnRoom
{
    // public bool GranteeSpawn = false;


    [Range(0, 100)]
    public float SpawnChance = 10f;

    [Min(0)]
    public int minAmountToSpawn = 1;
    public int maxAmountToSpawn = 1;

    public SO_LevelPiece RoomPiece;
}

[CreateAssetMenu(fileName = "Level Piece Collection", menuName = "LevelGeneration/LevelPieceCollection")]
public class SO_LevelPieceCollection : ScriptableObject
{

    public float UnitSizeInMeters = 1f;

    public SO_LevelPiece[] StartRooms;
    public SO_LevelPiece[] ExitRooms;

    public OneTimeSpawnRoom[] OneTimeSpawnRooms;

    public SO_LevelPiece[] RegularRooms;
    public SO_LevelPiece[] Corridors;
    public SO_LevelPiece[] EndCapRooms;

    void OnValidate()
    {
        if (UnitSizeInMeters <= 0) UnitSizeInMeters = 1;
    }
}
