using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBaker : SequenceBase
{
    LevelGenerator lg;
    [SerializeField]
    NavMeshSurface humanNavSurface;
    [SerializeField]
    NavMeshSurface meleeNavSurface;
    [SerializeField]
    NavMeshSurface rangedNavSurface;

    NavMeshData navMeshData;

    public event Action onNavMeshSurfaceGenerated;
    public override event Action OnThisSequenceEnd;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // LevelGenerator lg = GetComponent<LevelGenerator>();
        // lg.onLevelGenerationComplete += OnLevelGenComplete;

        // humanNavSurface = GetComponent<NavMeshSurface>();
    }

    private void OnLevelGenComplete()
    {
        UpdateNav();
    }

    void UpdateNav()
    {
        humanNavSurface.BuildNavMesh(); // I hate this. Cant delay or wait, the game will just be frozen.
        meleeNavSurface.BuildNavMesh();
        rangedNavSurface.BuildNavMesh();
        // UnityEditor.StaticOcclusionCulling.Compute();
        onNavMeshSurfaceGenerated?.Invoke();
        OnThisSequenceEnd?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void StartSequence()
    {
        UpdateNav();
    }
}
