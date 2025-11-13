using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ContainerPlacement
{
    public CraneController craneControllerResponsible;
    public GameObject containerPlacementTarget;
    public List<GameObject> containers = new List<GameObject>();

    public bool GetCraneToPlaceContainer(float offset)
    {

        bool res = craneControllerResponsible.PlaceContainerWall(containerPlacementTarget.transform.position + (Vector3.up * offset * containers.Count), out GameObject container);

        if (res)
        {
            containers.Add(container);
        }

        return res;
    }

    public bool GetCraneToRemoveContainer()
    {
        // container = null;
        bool result = craneControllerResponsible.RemoveContainerWall(containers.Last());

        if (result)
        {
            containers.RemoveAt(containers.Count - 1);
        }

        return result;
    }

    public int GetContainerCount()
    {
        return containers.Count;
    }

    public bool StillHaveContainers()
    {
        return containers.Count > 0;
    }

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

[Serializable]
public class ContainerRow
{
    public ContainerPlacement[] containerPlacementPoint = new ContainerPlacement[0];
}

[Serializable]
public class ContainerLayout
{
    public ContainerRow[] containerRows = new ContainerRow[0];
}

public class ArenaWallsManager : MonoBehaviour
{
    [SerializeField]
    private ContainerLayout containerLayout;

    [SerializeField]
    int RowCount = 4;

    [SerializeField]
    int ColumnCount = 10;

    int[,] gridPlacement;

    [SerializeField, Min(1)]
    int totalWallLayers = 2;

    [SerializeField]
    float wallHeight = 3.064f;

    void Start()
    {
        StartCoroutine(PregenWithContainers());
    }

    private IEnumerator PregenWithContainers()
    {
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

    }

    private IEnumerator JuggleContainerWalls()
    {
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
                    yield return new WaitForSeconds(1f);
                }
            }
            else
            {
                if (containerLayout.containerRows[randomChoice.x].containerPlacementPoint[randomChoice.y].GetCraneToPlaceContainer(wallHeight))
                {
                    if (containerLayout.containerRows[randomChoice.x].containerPlacementPoint[randomChoice.y].GetContainerCount() >= newLayout[randomChoice.x, randomChoice.y])
                        placements.Remove(randomChoice);
                    yield return new WaitForSeconds(1f);
                }
            }


            yield return new WaitForEndOfFrame();
        }

        gridPlacement = newLayout;

        print("Juggled");

    }

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
                yield return new WaitForSeconds(1f);
            }

            yield return new WaitForEndOfFrame();
        }



        print("Completed Wall Placement");
    }

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
                if (!containerLayout.containerRows[randomChoice.x].containerPlacementPoint[randomChoice.y].StillHaveContainers())
                    placements.Remove(randomChoice);
                yield return new WaitForSeconds(1f);
            }

            yield return new WaitForEndOfFrame();
        }


        print("Completed Wall Removal");
    }

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

    private int[] RandomDoorways(int doorwayAmount, int columnSize)
    {
        int[] randomDoorways = new int[doorwayAmount];

        for (int i = 0; i < randomDoorways.Length; i++)
        {
            randomDoorways[i] = UnityEngine.Random.Range(0, columnSize);
        }

        return randomDoorways;
    }
}
