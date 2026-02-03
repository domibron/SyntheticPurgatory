using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Chips/BaseChip", fileName = "SO_BaseChip")]
public class ChipSO : ScriptableObject
{
    public const int X_ROW = 1;
    public const int Y_ROW = 0;

    [SerializeField]
    Vector2Int[] blockLayout = { Vector2Int.zero };

    [SerializeField]
    Vector2Int size = Vector2Int.one;

    [SerializeField]
    Sprite inventoryImage = null;

    [SerializeField]
    GameObject BoardItem;

    /// <summary>
    /// This is called to modify the stats per chip. Presume the current stored chip values have been reset and add to the variable.
    /// </summary>
    /// <param name="pStats">The player stats to modify.</param>
    /// <param name="miscStats">The miscellaneous stats to modify.</param>
    public virtual void ModifyStats(ref PlayerStats pStats, ref MiscellaneousStats miscStats)
    {
        // Modify the stats, remember to add to the chip stats. They will all be reset to 0 before this is called.
        //pStats.MaxHealthStat.AddToChipIncreaseAmount(pStats.MaxHealthStat.CurrentValue * 1f); // TEMP
    }

    public virtual Vector2Int GetSize()
    {
        return size;
    }

    public virtual Sprite GetInventoryImage()
    {
        return inventoryImage;
    }

    public virtual GameObject GetBoardChipObject()
    {
        return BoardItem;
    }

    public virtual bool CanPlaceAtTargetSlot(ref int[,] chipBoard, Vector2Int targetGridPos, int currentID = -1)
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



    public virtual bool PlaceChipAtTargetSlot(ref int[,] chipBoard, Vector2Int targetGridPos, int currentID)
    {
        if (!CanPlaceAtTargetSlot(ref chipBoard, targetGridPos)) return false;

        foreach (Vector2Int chipPart in blockLayout)
        {
            // invert the Y because +1 y means up on screen but not on the board. Top to bottom, 0 to X.
            Vector2Int chipPartPos = targetGridPos + new Vector2Int(chipPart.x, -chipPart.y);

            SetIDAtTargetSlot(ref chipBoard, chipPartPos, currentID);
        }

        return true;
    }

    public virtual void RemoveChipFromBoard(ref int[,] chipBoard, int currentID)
    {
        for (int y = 0; y < chipBoard.GetLength(Y_ROW); y++)
        {
            for (int x = 0; x < chipBoard.GetLength(X_ROW); x++)
            {
                if (chipBoard[y, x] == currentID) chipBoard[y, x] = -1;
            }
        }
    }



    protected bool IsSlotOccupied(ref int[,] chipBoard, Vector2Int targetGridPos, int currentID = -1)
    {
        // Y, X - R, C
        //[0, 0] top left,
        //[X, 0] bottom left
        //[0, X] top right
        return chipBoard[targetGridPos.y, targetGridPos.x] != -1 && chipBoard[targetGridPos.y, targetGridPos.x] != currentID;
    }

    protected void SetIDAtTargetSlot(ref int[,] chipBoard, Vector2Int targetGridPos, int currentID)
    {
        if (targetGridPos.x < 0 || targetGridPos.y < 0 || targetGridPos.y >= chipBoard.GetLength(Y_ROW) || targetGridPos.x >= chipBoard.GetLength(X_ROW)) return;

        chipBoard[targetGridPos.y, targetGridPos.x] = currentID;
    }


}
