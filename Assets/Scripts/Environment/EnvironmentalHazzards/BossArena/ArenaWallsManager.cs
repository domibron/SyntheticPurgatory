using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// Data class for a single stack of containers, has the crane that places the containers, target location and all currently placed containers.
/// </summary>
[Serializable]
public class ContainerPlacement
{
    // TODO: have rotation also be taken into account.

    /// <summary>
    /// The crane that will add and remove containers.
    /// </summary>
    public CraneController craneControllerResponsible;

    /// <summary>
    /// The target point for the first container.
    /// </summary>
    public GameObject containerPlacementTarget;

    /// <summary>
    /// List of all currently placed containers.
    /// </summary>
    public List<GameObject> containers = new List<GameObject>();

    /// <summary>
    /// Try to get the crane to place a container down with the desired offset.
    /// </summary>
    /// <param name="offset">How high to place the container from the placement point.</param>
    /// <returns>True if the crane was given the job successfully.</returns>
    public bool GetCraneToPlaceContainer(float offset)
    {
        bool res = craneControllerResponsible.PlaceContainerWall(containerPlacementTarget.transform.position + (Vector3.up * offset * containers.Count), out GameObject container);

        if (res)
        {
            containers.Add(container);
        }

        return res;
    }

    /// <summary>
    /// Try to get the crane to remove one container of off the stack.
    /// </summary>
    /// <returns>True if the crane was given the job successfully.</returns>
    public bool GetCraneToRemoveContainer()
    {
        // container = null;
        bool result = craneControllerResponsible.RemoveContainerWall(containers.Last());

        if (result)
        {
            containers.RemoveAt(containers.Count - 1); // ? A stack might serve better.
        }

        return result;
    }

    /// <summary>
    /// Get the number of containers in this stack.
    /// </summary>
    /// <returns></returns>
    public int GetContainerCount()
    {
        return containers.Count;
    }

    /// <summary>
    /// Returns whether there there are containers remaining.
    /// </summary>
    /// <returns>True if there are containers remaining.</returns>
    public bool IsThereContainersRemaining()
    {
        return containers.Count > 0;
    }

    /// <summary>
    /// Spawn in a container into the stack bypassing the crane placement.
    /// </summary>
    /// <param name="stackCount">How many to stack.</param>
    /// <param name="offset">How much to offset them from each other.</param>
    public void SpawnInContainer(int stackCount, float offset)
    {
        for (int i = 0; i < stackCount; i++)
        {
            GameObject container = craneControllerResponsible.SpawnInContainerWall();
            container.transform.position = containerPlacementTarget.transform.position + (Vector3.up * offset * containers.Count);
            containers.Add(container);
        }
    }
}

// ? Why do we make a shitty [][] array when one layer suffices, not like we use both row and column for anything.

/// <summary>
/// A collection of container placement points.
/// </summary>
[Serializable]
public class ContainerRow
{
    // I do not want to rename anything in case it breaks serialization.
    /// <summary>
    /// All the container placement points in this row.
    /// </summary>
    public ContainerPlacement[] containerPlacementPoint = new ContainerPlacement[0];
}

/// <summary>
/// A collection of a grid of container placement points.
/// </summary>
[Serializable]
public class ContainerLayout
{
    /// <summary>
    /// All the rows that make up this container layout.
    /// </summary>
    public ContainerRow[] containerRows = new ContainerRow[0];
}

/// <summary>
/// Handles all the container walls placements, removal and random generation.
/// </summary>
public class ArenaWallsManager : MonoBehaviour
{
    /// <summary>
    /// All the container walls in this arena.
    /// </summary>
    [SerializeField]
    private ContainerLayout containerLayout;

    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    int RowCount = 4;

    [SerializeField]
    int ColumnCount = 10;

    // ? Why a 2d grid? This can be done in a better way.
    /// <summary>
    /// The grid layout of the containers.
    /// </summary>
    int[,] gridPlacement;

    /// <summary>
    /// The max amount of containers allowed to stack.
    /// </summary>
    [SerializeField, Min(1)]
    int totalWallLayers = 2;

    /// <summary>
    /// The height of the container.
    /// </summary>
    [SerializeField]
    float wallHeight = 3.064f;

    /// <summary>
    /// All cranes that are linked in the container placement. No duplicates, only unique.
    /// </summary>
    private List<CraneController> allCranes = new List<CraneController>();

    /// <summary>
    /// Is this arena wall manager currently doing something? 
    /// </summary>
    private bool inJob = false;

    /// <summary>
    /// Invoked when the current job was completed.
    /// </summary>
    public event Action OnJobCompleted;

    void Awake()
    {
        // get all the cranes referenced in the container placements.
        for (int i = 0; i < containerLayout.containerRows.Length; i++)
        {
            for (int j = 0; j < containerLayout.containerRows[i].containerPlacementPoint.Length; j++)
            {
                if (allCranes.Contains(containerLayout.containerRows[i].containerPlacementPoint[j].craneControllerResponsible)) continue;

                allCranes.Add(containerLayout.containerRows[i].containerPlacementPoint[j].craneControllerResponsible);
            }
        }

        // randomly generate the walls.
        StartCoroutine(PregenWithContainers());
    }

    /// <summary>
    /// Check to see if there is a job currently being performed.
    /// </summary>
    /// <returns>True if there is a job currently in progress.</returns>
    public bool IsStillInJob()
    {
        return inJob;
    }

    /// <summary>
    /// Gets a random wait time between 0 and 1 seconds.
    /// </summary>
    /// <returns>Random float value between 0 and 1.</returns>
    private float GetRandomWaitTime()
    {
        return UnityEngine.Random.Range(0f, 1f);
    }

    /// <summary>
    /// Coroutine that gets all the cranes to move back to their default locations.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ResetAllCranes()
    {
        List<CraneController> cranesToReset = allCranes;

        while (cranesToReset.Count > 0)
        {
            int craneIndex = UnityEngine.Random.Range(0, cranesToReset.Count);

            if (cranesToReset[craneIndex].ResetCrane())
            {
                cranesToReset.RemoveAt(craneIndex);
            }
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    /// <summary>
    /// Coroutine that randomly spawns in the container walls skipping the cranes.
    /// </summary>
    /// <returns></returns>
    private IEnumerator PregenWithContainers()
    {
        inJob = true;

        gridPlacement = GenerateData(totalWallLayers);

        List<Vector2Int> placements = new List<Vector2Int>();

        for (int x = 0; x < RowCount; x++)
        {
            for (int y = 0; y < ColumnCount; y++)
            {
                if (gridPlacement[x, y] <= 0) continue;
                placements.Add(new Vector2Int(x, y));
            }
        }


        while (placements.Count > 0)
        {
            Vector2Int randomChoice = placements[UnityEngine.Random.Range(0, placements.Count)];

            containerLayout.containerRows[randomChoice.x].containerPlacementPoint[randomChoice.y].SpawnInContainer(gridPlacement[randomChoice.x, randomChoice.y], wallHeight);

            placements.Remove(randomChoice);

            yield return new WaitForEndOfFrame();
        }



        print("Completed Wall Generation");
        inJob = false;
        // StartCoroutine(JuggleContainerWalls()); // ! DEBUG CODE
    }

    /// <summary>
    /// Tries to start the wall random job using the cranes.
    /// </summary>
    /// <returns>True if the arena manager managed to start this job.</returns>
    public bool StartJuggleJob()
    {
        if (inJob) return false;
        StartCoroutine(JuggleContainerWalls());

        return true;
    }

    /// <summary>
    /// Coroutine that handles the random generation and assignment of the containers to move them around using the cranes.
    /// </summary>
    /// <returns></returns>
    private IEnumerator JuggleContainerWalls()
    {
        inJob = true;
        int[,] newLayout = GenerateData(totalWallLayers);

        List<Vector2Int> placements = new List<Vector2Int>();

        for (int x = 0; x < RowCount; x++)
        {
            for (int y = 0; y < ColumnCount; y++)
            {
                if (newLayout[x, y] == gridPlacement[x, y]) continue;
                placements.Add(new Vector2Int(x, y));
            }
        }


        while (placements.Count > 0)
        {
            Vector2Int randomChoice = placements[UnityEngine.Random.Range(0, placements.Count)];

            if (gridPlacement[randomChoice.x, randomChoice.y] > newLayout[randomChoice.x, randomChoice.y])
            {
                if (containerLayout.containerRows[randomChoice.x].containerPlacementPoint[randomChoice.y].GetCraneToRemoveContainer())
                {
                    if (containerLayout.containerRows[randomChoice.x].containerPlacementPoint[randomChoice.y].GetContainerCount() <= newLayout[randomChoice.x, randomChoice.y])
                        placements.Remove(randomChoice);
                    yield return new WaitForSeconds(GetRandomWaitTime());
                }
            }
            else
            {
                if (containerLayout.containerRows[randomChoice.x].containerPlacementPoint[randomChoice.y].GetCraneToPlaceContainer(wallHeight))
                {
                    if (containerLayout.containerRows[randomChoice.x].containerPlacementPoint[randomChoice.y].GetContainerCount() >= newLayout[randomChoice.x, randomChoice.y])
                        placements.Remove(randomChoice);
                    yield return new WaitForSeconds(GetRandomWaitTime());
                }
            }


            yield return new WaitForEndOfFrame();
        }

        gridPlacement = newLayout;
        inJob = false;
        OnJobCompleted?.Invoke();

        print("Juggled");
        // StartCoroutine(JuggleContainerWalls()); // ! DEBUG CODE
    }

    /// <summary>
    /// Coroutine that gets the cranes to just place down containers.
    /// This was used but was replaced with randomly generating and spawning in the walls directly.
    /// </summary>
    /// <param name="generateNewGrid">True to generate a new layout.</param>
    /// <returns></returns>
    private IEnumerator PlaceContainerWalls(bool generateNewGrid = false)
    {
        if (generateNewGrid)
        {
            gridPlacement = GenerateData(totalWallLayers);
        }

        List<Vector2Int> placements = new List<Vector2Int>();

        for (int x = 0; x < RowCount; x++)
        {
            for (int y = 0; y < ColumnCount; y++)
            {
                if (gridPlacement[x, y] <= 0) continue;
                placements.Add(new Vector2Int(x, y));
            }
        }


        while (placements.Count > 0)
        {
            Vector2Int randomChoice = placements[UnityEngine.Random.Range(0, placements.Count)];

            if (containerLayout.containerRows[randomChoice.x].containerPlacementPoint[randomChoice.y].GetCraneToPlaceContainer(wallHeight))
            {
                if (containerLayout.containerRows[randomChoice.x].containerPlacementPoint[randomChoice.y].GetContainerCount() >= totalWallLayers)
                    placements.Remove(randomChoice);
                yield return new WaitForSeconds(GetRandomWaitTime());
            }

            yield return new WaitForEndOfFrame();
        }



        print("Completed Wall Placement");
    }


    /// <summary>
    /// Coroutine that will remove all the containers from the area.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ClearAllWalls()
    {


        List<Vector2Int> placements = new List<Vector2Int>();

        for (int x = 0; x < RowCount; x++)
        {
            for (int y = 0; y < ColumnCount; y++)
            {
                if (gridPlacement[x, y] <= 0) continue;
                placements.Add(new Vector2Int(x, y));
            }
        }

        while (placements.Count > 0)
        {
            Vector2Int randomChoice = placements[UnityEngine.Random.Range(0, placements.Count)];

            if (containerLayout.containerRows[randomChoice.x].containerPlacementPoint[randomChoice.y].GetCraneToRemoveContainer())
            {
                if (!containerLayout.containerRows[randomChoice.x].containerPlacementPoint[randomChoice.y].IsThereContainersRemaining())
                    placements.Remove(randomChoice);
                yield return new WaitForSeconds(GetRandomWaitTime());
            }

            yield return new WaitForEndOfFrame();
        }


        print("Completed Wall Removal");
    }

    /// <summary>
    /// Generate a new container layout for the room.
    /// </summary>
    /// <param name="maxLayers">The max amount the containers can stack.</param>
    /// <returns>The new grid layout.</returns>
    private int[,] GenerateData(int maxLayers)
    {
        int[,] gridPlacement = new int[RowCount, ColumnCount];

        for (int x = 0; x < RowCount; x++)
        {
            int[] doorways = RandomDoorways(2, ColumnCount);

            for (int y = 0; y < ColumnCount; y++)
            {
                if (doorways.Contains(y))
                {
                    gridPlacement[x, y] = 0;
                }
                else
                {
                    gridPlacement[x, y] = UnityEngine.Random.Range(1, maxLayers + 1);
                }
            }
        }

        return gridPlacement;
    }

    /// <summary>
    /// Generate random holes in the walls so the player can pass through them.
    /// </summary>
    /// <param name="doorwayAmount">The amount of holes to add.</param>
    /// <param name="columnSize">The max size of the column.</param>
    /// <returns>The position of the doorway on the row.</returns>
    private int[] RandomDoorways(int doorwayAmount, int columnSize)
    {
        int[] randomDoorways = new int[doorwayAmount];

        for (int i = 0; i < randomDoorways.Length; i++) randomDoorways[i] = -1;

        for (int i = 0; i < randomDoorways.Length; i++)
        {
            int randomSlot = UnityEngine.Random.Range(0, columnSize);

            while (randomDoorways.Contains(randomSlot))
            {
                randomSlot = UnityEngine.Random.Range(0, columnSize);
            }

            randomDoorways[i] = randomSlot;
        }

        return randomDoorways;
    }
}
