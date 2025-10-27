using System;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private GameObject playerCanvas;

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
            GameObject canvasObject = Instantiate(playerCanvas);
            GameObject playerObject = Instantiate(playerPrefab, playerSpawnLocation, Quaternion.identity);

            playerObject.transform.GetComponent<PlayerDeath>().deathCanvasScript = canvasObject.transform.GetComponent<DeathCanvas>();
        }
    }

    private void OnLevelGenComplete()
    {
        playerSpawnLocation = levelGenerator.GetPlayerSpawnLocation();
    }

}
