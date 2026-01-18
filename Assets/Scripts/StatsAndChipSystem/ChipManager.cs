using System.Collections.Generic;
using UnityEngine;



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

    // id, chip. This acts as a ref for all chips.
    Dictionary<int, ChipSO> allChips = new Dictionary<int, ChipSO>();

    int[,] chipBoard = new int[3, 4]; // Hard coded size, grid on the chip menu on hub does not update to fit this!

    List<int> allActiveChips = new List<int>();
    List<int> allInventoryChips = new List<int>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

    }

    public bool AddChipToGrid(int id, Vector2Int targetPos)
    {
        allActiveChips.Add(id);
        return allChips[id].PlaceChipAtTargetSlot(ref chipBoard, targetPos, id);
    }

    public void RemoveChipFromBoard(int id)
    {
        allActiveChips.Remove(id);
        allChips[id].RemoveChipFromBoard(ref chipBoard, id); // this is a little silly, i know.
    }

    public void AddChipToInventory(int id)
    {
        allInventoryChips.Add(id);
    }

    public void RemoveChipToInventory(int id)
    {
        allInventoryChips.Remove(id);
    }

    public void AddChipModifiers(ref PlayerStats playerStats, ref MiscellaneousStats miscellaneousStats)
    {
        foreach (var chip in allActiveChips)
        {
            allChips[chip].ModifyStats(ref playerStats, ref miscellaneousStats);
        }
    }

    public void OpenModule(ChipType type)
    {
        ChipSO newChip = GetRandomChipFromModule(type);

        int newID = allChips.Keys.Count;
        allChips.Add(newID, newChip); // add to lookup table.
        AddChipToInventory(newID);
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


}
