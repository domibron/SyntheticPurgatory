using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Managers the player's chip board and chip inventory. Run based manager
/// </summary>
public class ChipRunM : MonoBehaviour
{
    /// <summary>
    /// Singleton for the <see cref="ChipRunM"/> class.
    /// </summary>
    public static ChipRunM Instance { get; private set; }

    /// <summary>
    /// All the possible types of openable chips.
    /// </summary>
    public enum ChipType
    {
        Common,
        Rare,
        Epic,
    }

    /// <summary>
    /// A array of all common chips that can be unlocked from a common module.
    /// </summary>
    [SerializeField]
    ChipSO[] common_Chips;

    /// <summary>
    ///  A array of all rare chips that can be unlocked from a rare module.
    /// </summary>
    [SerializeField]
    ChipSO[] rare_Chips;

    /// <summary>
    ///  A array of all epic chips that can be unlocked from a epic module.
    /// </summary>
    [SerializeField]
    ChipSO[] epic_Chips;

    // id, chip. This acts as a ref for all chips. Where unlocked / collected chips are stored.
    /// <summary>
    /// Chips with their associated id for lookup.
    /// </summary>
    Dictionary<int, ChipSO> allChips = new Dictionary<int, ChipSO>();

    /// <summary>
    /// The chip board placement of chips, used for checking overlap and placement of chips.
    /// </summary>
    int[,] chipBoard = new int[3, 4]; // Hard coded size, grid on the chip menu on hub does not update to fit this!

    /// <summary>
    /// A list and position of all placed chips on the board.
    /// </summary>
    Dictionary<int, Vector2Int> allPlacedChips = new Dictionary<int, Vector2Int>();

    /// <summary>
    /// All the chips that are in the inventory that have not been placed down.
    /// </summary>
    List<int> allInventoryChips = new List<int>();


    void Awake()
    {
        // Game manager automatically removes the clone, just override the reference.
        Instance = this;

        // Initialize the board.
        SetUpBoard();
    }

    /// <summary>
    /// Set all slots of the chip board to empty.
    /// </summary>
    private void SetUpBoard()
    {
        for (int y = 0; y < chipBoard.GetLength(ChipUtil.Y_ROW); y++)
        {
            for (int x = 0; x < chipBoard.GetLength(ChipUtil.X_ROW); x++)
            {
                chipBoard[y, x] = -1;
            }
        }
    }

    /// <summary>
    /// This can add a new chip or move an existing chip.
    /// </summary>
    /// <param name="id">The id of the chip to place on the board.</param>
    /// <param name="targetPos">The target coordinates on the chip board.</param>
    /// <returns>True if the operation was successful.</returns>
    public bool AddChipToBoard(int id, Vector2Int targetPos)
    {
        if (allPlacedChips.ContainsKey(id))
        {
            if (ChipUtil.CanPlaceAtTargetSlot(ref chipBoard, targetPos, allChips[id].GetBlockLayout(), id))
            {
                ChipUtil.RemoveChipFromBoard(ref chipBoard, id); // move the chip. // Remove all old cells otherwise it will be hard to remove both new and old.
                ChipUtil.PlaceChipAtTargetSlot(ref chipBoard, targetPos, allChips[id].GetBlockLayout(), id);
                return true;
            }
        }
        else
        {
            if (ChipUtil.PlaceChipAtTargetSlot(ref chipBoard, targetPos, allChips[id].GetBlockLayout(), id))
            {
                allPlacedChips.Add(id, targetPos);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Removes the chip with the id from the board.
    /// </summary>
    /// <param name="id"></param>
    public void RemoveChipFromBoard(int id)
    {
        allPlacedChips.Remove(id);
        ChipUtil.RemoveChipFromBoard(ref chipBoard, id); // this is a little silly, i know.
    }

    /// <summary>
    /// Adds the chip to the inventory list.
    /// </summary>
    /// <param name="id">The id to add to the list.</param>
    public void AddChipToInventory(int id)
    {
        if (allInventoryChips.Contains(id)) return;

        allInventoryChips.Add(id);
    }

    /// <summary>
    /// Removes the chip from the inventory list.
    /// </summary>
    /// <param name="id">The id to remove from the list.</param>
    public void RemoveChipToInventory(int id)
    {
        allInventoryChips.Remove(id);
    }

    /// <summary>
    /// Add the modifiers to the stat classes.
    /// </summary>
    /// <param name="playerStats">A reference to the player stats.</param>
    /// <param name="miscellaneousStats">A reference to the miscellaneous stats for scrap and other things.</param>
    public void AddChipModifiers(ref PlayerStats playerStats, ref MiscellaneousStats miscellaneousStats)
    {
        foreach (int chip in allPlacedChips.Keys)
        {
            allChips[chip].ModifyStats(ref playerStats, ref miscellaneousStats);
        }
    }

    /// <summary>
    /// Open a module to get a chip and add it to the inventory.
    /// </summary>
    /// <param name="type">The tier of the module to open.</param>
    /// <returns>The chip data that was unlocked.</returns>
    public ChipSO OpenModule(ModuleTier type)
    {
        ChipSO newChip = GetRandomChipFromModule((ChipType)type);

        int newID = allChips.Keys.Count;
        allChips.Add(newID, newChip); // add to lookup table.
        AddChipToInventory(newID);

        return newChip;
    }

    /// <summary>
    /// Get a random chip based on the tier of the chip.
    /// </summary>
    /// <param name="chipType">The tier of the chip.</param>
    /// <returns>The random chip that was selected.</returns>
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

    /// <summary>
    /// Get a random chip from the collection of chips.
    /// </summary>
    /// <param name="collection">The collection of chips to select from.</param>
    /// <returns>A random chip form this collection.</returns>
    private ChipSO GetRandomChipFromCollection(ChipSO[] collection)
    {
        if (collection.Length <= 0) return null;

        // Random.Range will return values between X and Y-1. Random is fuck-y.
        return collection[Random.Range(0, collection.Length)];
    }

    /// <summary>
    /// Get all the chips in the inventory.
    /// </summary>
    /// <returns>The list of ids.</returns>
    public List<int> GetAllInventoryChips()
    {
        return allInventoryChips;
    }

    /// <summary>
    /// Get all the chips that are placed on the board.
    /// </summary>
    /// <returns>A collection of chips with id key and vector2Int position on the board.</returns>
    public Dictionary<int, Vector2Int> GetAllPlacedChips()
    {
        return allPlacedChips;
    }

    /// <summary>
    /// Get the chip data from the id.
    /// </summary>
    /// <param name="id">The id to look for the associated data for.</param>
    /// <returns>The chip data if found or null if not.</returns>
    public ChipSO GetChipDataFromID(int id)
    {
        if (!allChips.ContainsKey(id)) return null;

        return allChips[id];
    }

    /// <summary>
    /// Get the cound of all unlocked chips.
    /// </summary>
    /// <returns>The total chips the player currently has in total.</returns>
    public int GetTotalChipCount()
    {
        return allChips.Count;
    }

    /// <summary>
    /// Get the id of a chip from the coordinates on the board.
    /// </summary>
    /// <param name="pos">The coordinates to look up at.</param>
    /// <returns>The chip id or -1 if null / blank.</returns>
    public int GetChipIdFromGridPos(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0 || pos.y >= chipBoard.GetLength(ChipUtil.Y_ROW) || pos.x >= chipBoard.GetLength(ChipUtil.X_ROW)) return -1;

        return chipBoard[pos.y, pos.x];
    }

    /// <summary>
    /// Get the board as a string. Mainly used for debugging.
    /// </summary>
    /// <returns>The board as a string.</returns>
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

    /// <summary>
    /// Modify the stats with all the currently placed down chips on the board.
    /// </summary>
    /// <param name="pStats">A reference to the player stats.</param>
    /// <param name="mStats">A reference to the miscellaneous stats.</param>
    public void ModifyStatsFromAllBoardChips(ref PlayerStats pStats, ref MiscellaneousStats mStats)
    {
        pStats.ResetAllChipStats();
        mStats.ResetAllChipStats();

        foreach (var id in allPlacedChips.Keys)
        {
            allChips[id].ModifyStats(ref pStats, ref mStats);
        }
    }


}

/// <summary>
/// Utility class for the chip system.
/// </summary>
public static class ChipUtil
{
    /// <summary>
    /// The X represented in the 2d array.
    /// </summary>
    public const int X_ROW = 1;

    /// <summary>
    /// The Y represented in the 2d array.
    /// </summary>
    public const int Y_ROW = 0;

    /// <summary>
    /// Removes the chip from the board.
    /// </summary>
    /// <param name="chipBoard">The reference to the board to modify.</param>
    /// <param name="currentID">The id of the chip to remove.</param>
    public static void RemoveChipFromBoard(ref int[,] chipBoard, int currentID)
    {
        for (int y = 0; y < chipBoard.GetLength(Y_ROW); y++)
        {
            for (int x = 0; x < chipBoard.GetLength(X_ROW); x++)
            {
                if (chipBoard[y, x] == currentID) chipBoard[y, x] = -1;
            }
        }
    }

    /// <summary>
    /// Checks whether this chip can be placed at that target grid coordinates.
    /// </summary>
    /// <param name="chipBoard">A reference to the chip board to check on.</param>
    /// <param name="targetGridPos">The target placement of the chip on the board.</param>
    /// <param name="currentID">The id of the chip if it's already placed on the board.</param>
    /// <returns>True if the chip can be placed at the target coordinates.</returns>
    public static bool CanPlaceAtTargetSlot(ref int[,] chipBoard, Vector2Int targetGridPos, Vector2Int[] blockLayout, int currentID = -1)
    {
        foreach (Vector2Int chipPart in blockLayout)
        {
            Vector2Int checkingPos = targetGridPos + new Vector2Int(chipPart.x, -chipPart.y);

            if (checkingPos.x < 0 || checkingPos.y < 0 || checkingPos.y >= chipBoard.GetLength(Y_ROW) || checkingPos.x >= chipBoard.GetLength(X_ROW))
            {
                // Debug.Log("out of bounds");
                return false; // Out of bounds.
            }

            if (IsSlotOccupied(ref chipBoard, checkingPos, currentID))
            {
                // Debug.Log("Slot is occupied");
                return false; // Slot not available.
            }
        }

        return true;
    }


    /// <summary>
    /// Checks to see if the chip can be placed and tries to place the chip at the target grid coordinates if able.
    /// </summary>
    /// <param name="chipBoard">The reference to the board to modify / place the chip on.</param>
    /// <param name="targetGridPos">The target coordinates of the chip on the board.</param>
    /// <param name="currentID">The id to write to the board to represent this chip.</param>
    /// <returns>True if the operation was successful.</returns>
    public static bool PlaceChipAtTargetSlot(ref int[,] chipBoard, Vector2Int targetGridPos, Vector2Int[] blockLayout, int currentID)
    {
        if (!CanPlaceAtTargetSlot(ref chipBoard, targetGridPos, blockLayout)) return false;

        foreach (Vector2Int chipPart in blockLayout)
        {
            // invert the Y because +1 y means up on screen but not on the board. Top to bottom, 0 to X.
            Vector2Int chipPartPos = targetGridPos + new Vector2Int(chipPart.x, -chipPart.y);

            SetIDAtTargetSlot(ref chipBoard, chipPartPos, currentID);
        }

        return true;
    }



    /// <summary>
    /// Checks of the single slot is occupied at the target coordinates on the board.
    /// </summary>
    /// <param name="chipBoard">A reference to the board to check on.</param>
    /// <param name="targetGridPos">The target slot coordinates.</param>
    /// <param name="currentID">The id of the chip to ignore.</param>
    /// <returns>True if the slot is occupied.</returns>
    public static bool IsSlotOccupied(ref int[,] chipBoard, Vector2Int targetGridPos, int currentID = -1)
    {
        // Y, X - R, C
        //[0, 0] top left,
        //[X, 0] bottom left
        //[0, X] top right
        return chipBoard[targetGridPos.y, targetGridPos.x] != -1 && chipBoard[targetGridPos.y, targetGridPos.x] != currentID;
    }

    /// <summary>
    /// Sets the id at the target coordinates on the board.
    /// </summary>
    /// <param name="chipBoard">A reference to the board to modify.</param>
    /// <param name="targetGridPos">The target coordinates to set the id for.</param>
    /// <param name="currentID">The id to set the slot to.</param>
    public static void SetIDAtTargetSlot(ref int[,] chipBoard, Vector2Int targetGridPos, int currentID)
    {
        if (targetGridPos.x < 0 || targetGridPos.y < 0 || targetGridPos.y >= chipBoard.GetLength(Y_ROW) || targetGridPos.x >= chipBoard.GetLength(X_ROW)) return;

        chipBoard[targetGridPos.y, targetGridPos.x] = currentID;
    }


    /// <summary>
    /// Instantiates the board item and returns a reference to the game object that was created.
    /// </summary>
    /// <returns>Reference to the created board game object.</returns>
    public static GameObject CreateAndReturnBoardItem(ChipSO chip)
    {
        GameObject go = Object.Instantiate(chip.GetBoardChipObject());

        if (chip.IsGenerativeBoardItem()) go.GetComponent<GenerateChip>().GenerateVisualChip(chip);

        return go;
    }
}
