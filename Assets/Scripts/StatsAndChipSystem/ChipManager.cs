using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class ChipManager : MonoBehaviour
{
    public static ChipManager Instance { get; private set; }

    public enum ChipType
    {
        Common,
        Rare,
        Epic,
    }

    [SerializeField]
    ChipSO[] common_Chips;

    [SerializeField]
    ChipSO[] rare_Chips;

    [SerializeField]
    ChipSO[] epic_Chips;

    // id, chip. This acts as a ref for all chips. Where unlocked / collected chips are stored.
    Dictionary<int, ChipSO> allChips = new Dictionary<int, ChipSO>();

    int[,] chipBoard = new int[3, 4]; // Hard coded size, grid on the chip menu on hub does not update to fit this!

    Dictionary<int, Vector2Int> allPlacedChips = new Dictionary<int, Vector2Int>();
    List<int> allInventoryChips = new List<int>();


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        SetUpBoard();
    }

    void Start()
    {
        // OpenModule(ChipType.Common); // DEBUG REMOVE
        // OpenModule(ChipType.Common);
    }

    private void SetUpBoard()
    {
        for (int y = 0; y < chipBoard.GetLength(ChipSO.Y_ROW); y++)
        {
            for (int x = 0; x < chipBoard.GetLength(ChipSO.X_ROW); x++)
            {
                chipBoard[y, x] = -1;
            }
        }
    }

    /// <summary>
    /// This can add a new chip or move an existing chip.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="targetPos"></param>
    /// <returns>Whether the operation was successful.</returns>
    public bool AddChipToBoard(int id, Vector2Int targetPos)
    {
        if (allPlacedChips.ContainsKey(id))
        {
            if (allChips[id].CanPlaceAtTargetSlot(ref chipBoard, targetPos, id))
            {
                allChips[id].RemoveChipFromBoard(ref chipBoard, id); // move the chip. // Remove all old cells otherwise it will be hard to remove both new and old.
                allChips[id].PlaceChipAtTargetSlot(ref chipBoard, targetPos, id);
                return true;
            }
        }
        else
        {
            if (allChips[id].PlaceChipAtTargetSlot(ref chipBoard, targetPos, id))
            {
                allPlacedChips.Add(id, targetPos);
                return true;
            }
        }

        return false;
    }

    public void RemoveChipFromBoard(int id)
    {
        allPlacedChips.Remove(id);
        allChips[id].RemoveChipFromBoard(ref chipBoard, id); // this is a little silly, i know.
    }

    public void AddChipToInventory(int id)
    {
        if (allInventoryChips.Contains(id)) return;

        allInventoryChips.Add(id);
    }

    public void RemoveChipToInventory(int id)
    {
        allInventoryChips.Remove(id);
    }

    public void AddChipModifiers(ref PlayerStats playerStats, ref MiscellaneousStats miscellaneousStats)
    {
        foreach (int chip in allPlacedChips.Keys)
        {
            allChips[chip].ModifyStats(ref playerStats, ref miscellaneousStats);
        }
    }

    public ChipSO OpenModule(ChipType type)
    {
        ChipSO newChip = GetRandomChipFromModule(type);

        int newID = allChips.Keys.Count;
        allChips.Add(newID, newChip); // add to lookup table.
        AddChipToInventory(newID);

        return newChip;
    }

    public ChipSO GetRandomChipFromModule(ChipType chipType)
    {
        switch (chipType)
        {
            case ChipType.Common:
                return GetRandomChipFromCollection(common_Chips);
            case ChipType.Rare:
                return GetRandomChipFromCollection(rare_Chips);
            case ChipType.Epic:
                return GetRandomChipFromCollection(epic_Chips);
        }

        return null;
    }

    private ChipSO GetRandomChipFromCollection(ChipSO[] collection)
    {
        if (collection.Length <= 0) return null;

        // Random.Range will return values between X and Y-1. Random is fucky in computers.
        return collection[Random.Range(0, collection.Length)];
    }

    public List<int> GetAllInventoryChips()
    {
        return allInventoryChips;
    }

    public Dictionary<int, Vector2Int> GetAllPlacedChips()
    {
        return allPlacedChips;
    }

    public ChipSO GetChipDataFromID(int id)
    {
        if (!allChips.ContainsKey(id)) return null;

        return allChips[id];
    }

    public int GetTotalChipCount()
    {
        return allChips.Count;
    }

    public int GetChipIdFromGridPos(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0 || pos.y >= chipBoard.GetLength(ChipSO.Y_ROW) || pos.x >= chipBoard.GetLength(ChipSO.X_ROW)) return -1;

        return chipBoard[pos.y, pos.x];
    }

    public string BoardToString()
    {
        int[,] data = chipBoard;

        string returnedString = "";

        for (int r = 0; r < data.GetLength(0); r++)
        {
            for (int c = 0; c < data.GetLength(1); c++)
            {
                returnedString += data[r, c].ToString("D2") + " ";
            }
            returnedString += "\n";
        }
        return returnedString;
    }

    public void ModifyStatChipData(ref PlayerStats pStats, ref MiscellaneousStats mStats)
    {
        pStats.ResetAllChipStats();
        mStats.ResetAllChipStats();

        foreach (var id in allPlacedChips.Keys)
        {
            allChips[id].ModifyStats(ref pStats, ref mStats);
        }
    }


}
