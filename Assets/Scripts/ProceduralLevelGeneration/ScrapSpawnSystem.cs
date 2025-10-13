using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class ScrapSpawnSystem : MonoBehaviour
{
    NavMeshBaker navMeshBaker;

    LevelGenerator levelGenerator;

    int[,] levelData = new int[0, 0];

    NavMeshSurface navMeshSurface;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        levelGenerator = GetComponent<LevelGenerator>();

        navMeshBaker = GetComponent<NavMeshBaker>();

        navMeshSurface = GetComponent<NavMeshSurface>();

        navMeshBaker.onNavMeshSurfaceGenerated += OnNavMeshSurfaceGenerated;
    }

    private void OnNavMeshSurfaceGenerated()
    {

        levelData = levelGenerator.GetLevelData();

        for (int x = 0; x < levelData.GetLength(0); x++)
        {
            for (int y = 0; y < levelData.GetLength(1); y++)
            {
                if (levelData[x, y] <= 0) continue;

                // spawn enemies.
                SpawnRandomScrap(x, y, levelGenerator.LevelPieceCollection.UnitSizeInMeters);
            }
        }
    }

    private void SpawnRandomScrap(int x, int y, float gridSizeUnit)
    {
        Vector3 convertedCoordinated = new Vector3((x * gridSizeUnit) + (gridSizeUnit / 2f), transform.position.y, (y * gridSizeUnit) + (gridSizeUnit / 2f));

        Vector3 randomPoint = convertedCoordinated + UnityEngine.Random.insideUnitSphere * (gridSizeUnit / 2f);

        if (UnityEngine.Random.Range(0f, 1f) < 0.5f) return;

        int worth = UnityEngine.Random.Range(1, 32);


        bool res = NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, gridSizeUnit / 2f, navMeshSurface.layerMask);

        if (res)
        {
            ScrapManager.Instance.SpawnScrap(worth, hit.position + Vector3.up);
        }
    }
}
