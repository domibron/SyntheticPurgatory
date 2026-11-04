using System;
using UnityEngine;

public class SpawnPlayer : SequenceBase
{
    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private GameObject playerCanvas;

    private Vector3 playerSpawnLocation;

    private LevelGenerator levelGenerator;

    public override event Action OnThisSequenceEnd;

    private float currentProgress = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        levelGenerator = GetComponent<LevelGenerator>();
        // levelGenerator.onLevelGenerationComplete += OnLevelGenComplete;
        // GetComponent<NavMeshBaker>().onNavMeshSurfaceGenerated += OnNavMeshSurfaceGenerated;
    }

    public override void StartSequence()
    {
        currentProgress = 0f;
        playerSpawnLocation = levelGenerator.GetPlayerSpawnLocation();

        if (playerPrefab != null)
        {
            // We dont store the values here anmore.
            Instantiate(playerCanvas);
            Instantiate(playerPrefab, playerSpawnLocation, Quaternion.identity);

            /// DO NOT put code here if you need to link a system with the player, use <see cref="PlayerRefFetcher.Instance"/> instead.
            /// Just make sure to wait for the player to spawn first. Look at this link for more info for start corutine.
            /// <see cref="https://docs.unity.com/en-us/engine/6000.0/script-reference/unityengine/monobehaviour/start#remarks"/>.
            // playerObject.transform.GetComponent<PlayerDeath>().deathCanvasScript = canvasObject.transform.GetComponent<DeathCanvas>();
        }

        currentProgress = 1f;
        OnThisSequenceEnd?.Invoke();
    }

    public override float GetProgress()
    {
        return currentProgress;
    }
}
