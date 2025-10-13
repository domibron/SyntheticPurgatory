using System;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    NavMeshBaker navMeshBaker;

    public GameObject RangedEnemy;
    public GameObject MeleeEnemy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navMeshBaker = GetComponent<NavMeshBaker>();

        navMeshBaker.onNavMeshSurfaceGenerated += OnNavMeshSurfaceGenerated;
    }

    private void OnNavMeshSurfaceGenerated()
    {

    }
}
