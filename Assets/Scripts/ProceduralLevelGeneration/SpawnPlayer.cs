using System;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;

    private Vector3 playerSpawnLocation;

    private LevelGenerator levelGenerator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        levelGenerator = GetComponent<LevelGenerator>();
        levelGenerator.onLevelGenerationComplete += OnLevelGenComplete;
        GetComponent<NavMeshBaker>().onNavMeshSurfaceGenerated += OnNavMeshSurfaceGenerated;
    }

    private void OnNavMeshSurfaceGenerated()
    {
        if (playerPrefab != null)
        {
            Instantiate(playerPrefab, playerSpawnLocation, Quaternion.identity);
        }
    }

    private void OnLevelGenComplete()
    {
        playerSpawnLocation = levelGenerator.GetPlayerSpawnLocation();
    }

}
