using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellType
{
    NotWalkable,
    ExistingArea,
    NotSearched,
}

public class Cell
{
    public int x;
    public int y;

    public int gCost;
    public int hCost;
    public int fCost;

    public CellType cellType;
    public Cell previousCell;

    public Cell(int x, int y)
    {
        this.x = x;
        this.y = y;
        cellType = CellType.NotSearched;
    }

    public void CalculateFCost()
    {

        fCost = gCost + hCost; // new area.

    }

    public Vector2Int GetVector2Int()
    {

        return new Vector2Int(x, y);
    }


}


public class PathFinding
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = MOVE_STRAIGHT_COST; // because we move right then up etc.


    private Cell[,] levelData;
    private List<Cell> openList;
    private List<Cell> closedList;

    public PathFinding(int width, int height)
    {
        levelData = new Cell[width, height];

        for (int x = 0; x < levelData.GetLength(0); x++)
        {
            for (int y = 0; y < levelData.GetLength(1); y++)
            {
                levelData[x, y] = new Cell(x, y);
            }
        }
    }

    public List<Vector2Int> FindPath(int startX, int startY, int endX, int endY, List<Vector2Int> ExistingAreas)
    {
        Cell startCell = levelData[startX, startY];
        Cell endCell = levelData[endX, endY];

        openList = new List<Cell> { startCell };
        closedList = new List<Cell>();

        for (int x = 0; x < levelData.GetLength(0); x++)
        {
            for (int y = 0; y < levelData.GetLength(1); y++)
            {
                Cell cell = levelData[x, y];
                cell.gCost = int.MaxValue;
                cell.CalculateFCost();
                cell.previousCell = null;

                if (ExistingAreas.Contains(new Vector2Int(x, y)))
                {
                    cell.cellType = CellType.ExistingArea;
                }
            }
        }

        startCell.gCost = 0;
        startCell.hCost = CalculateDistanceCost(startCell, endCell);
        startCell.CalculateFCost();

        while (openList.Count > 0)
        {
            Cell currentCell = GetLowestFCostCell(openList);
            if (currentCell == endCell)
            {
                return CalculatePath(endCell);
            }

            openList.Remove(currentCell);
            closedList.Add(currentCell);

            foreach (Cell neighbourCell in GetNeighbourList(currentCell))
            {
                if (closedList.Contains(neighbourCell)) continue;

                if (neighbourCell.cellType == CellType.NotWalkable)
                {
                    closedList.Add(neighbourCell);
                    continue;
                }

                int tentativeGCost = currentCell.gCost + CalculateDistanceCost(currentCell, neighbourCell);

                if (tentativeGCost < neighbourCell.gCost)
                {
                    neighbourCell.previousCell = currentCell;
                    neighbourCell.gCost = tentativeGCost;
                    neighbourCell.hCost = CalculateDistanceCost(neighbourCell, endCell);
                    neighbourCell.CalculateFCost();

                    if (!openList.Contains(neighbourCell))
                    {
                        openList.Add(neighbourCell);
                    }
                }
            }
        }

        // out of nodes on the open list
        return null;
    }

    private List<Cell> GetNeighbourList(Cell currentCell)
    {
        List<Cell> neighbourList = new List<Cell>();

        if (currentCell.x - 1 >= 0)
        {
            neighbourList.Add(GetCell(currentCell.x - 1, currentCell.y)); // left
            // if (currentCell.y - 1 >= 0) neighbourList.Add(GetCell(currentCell.x - 1, currentCell.y - 1)); // left bottom
            // if (currentCell.y + 1 < levelData.GetLength(1)) neighbourList.Add(GetCell(currentCell.x - 1, currentCell.y + 1)); // left top
        }
        if (currentCell.x + 1 < levelData.GetLength(0))
        {
            neighbourList.Add(GetCell(currentCell.x + 1, currentCell.y)); // right
            // if (currentCell.y - 1 >= 0) neighbourList.Add(GetCell(currentCell.x + 1, currentCell.y - 1)); // right bottom
            // if (currentCell.y + 1 < levelData.GetLength(1)) neighbourList.Add(GetCell(currentCell.x + 1, currentCell.y + 1)); // right top
        }
        if (currentCell.y - 1 >= 0) neighbourList.Add(GetCell(currentCell.x, currentCell.y - 1)); // bottom
        if (currentCell.y + 1 < levelData.GetLength(1)) neighbourList.Add(GetCell(currentCell.x, currentCell.y + 1)); // top

        return neighbourList;
    }

    private Cell GetCell(int x, int y)
    {
        return levelData[x, y];
    }

    private List<Vector2Int> CalculatePath(Cell endCell)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        path.Add(endCell.GetVector2Int());

        Cell currentCell = endCell;

        while (currentCell.previousCell != null)
        {
            path.Add(currentCell.GetVector2Int());
            currentCell = currentCell.previousCell;
        }

        path.Reverse();
        return path;
    }

    private int CalculateDistanceCost(Cell cellA, Cell cellB)
    {
        int xDistance = Mathf.Abs(cellA.x - cellB.x);
        int yDistance = Mathf.Abs(cellA.y - cellB.y);
        int remaining = Mathf.Abs(xDistance - yDistance);

        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private Cell GetLowestFCostCell(List<Cell> cellList)
    {
        Cell lowestFCostCell = cellList[0];
        for (int i = 1; i < cellList.Count; i++)
        {
            if (cellList[i].fCost < lowestFCostCell.fCost)
            {
                lowestFCostCell = cellList[i];
            }
        }

        return lowestFCostCell;
    }
}
