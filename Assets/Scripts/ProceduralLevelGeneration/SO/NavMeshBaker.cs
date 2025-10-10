using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBaker : MonoBehaviour
{
    LevelGenerator lg;
    NavMeshSurface navMeshSurface;

    NavMeshData navMeshData;

    public event Action onNavMeshSurfaceGenerated;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LevelGenerator lg = GetComponent<LevelGenerator>();
        lg.onLevelGenerationComplete += OnLevelGenComplete;

        navMeshSurface = GetComponent<NavMeshSurface>();
    }

    private void OnLevelGenComplete()
    {
        UpdateNav();
    }

    void UpdateNav()
    {
        navMeshSurface.BuildNavMesh(); // I hate this. Cant delay or wait, the game will just be frozen.
    }

    // Update is called once per frame
    void Update()
    {

    }


}
