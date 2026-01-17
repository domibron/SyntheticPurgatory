using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Stats/BaseChip", fileName = "SO_BaseChip")]
public class ChipSO : ScriptableObject
{
    [SerializeField]
    Vector2Int[] blockLayout = { Vector2Int.zero };

    [SerializeField]
    Vector2Int size = Vector2Int.one;

    [SerializeField]
    Sprite inventoryImage = null;

    [SerializeField]
    GameObject BoardItem;

    public virtual void ModifyStats(ref PlayerStats pStats, ref MiscellaneousStats miscStats)
    {
        // Modify the stats, remember to add to the chip stats. They will all be reset to 0 before this is called.
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

    public virtual bool CanPlaceAtTargetSlot(int[,] chipBoard, Vector2Int targetGridPos, int currentID = -1)
    {
        foreach (Vector2Int chipPart in blockLayout)
        {
            Vector2Int checkingPos = targetGridPos + new Vector2Int(chipPart.x, -chipPart.y);

            if (checkingPos.x < 0 || checkingPos.y < 0 || checkingPos.y >= chipBoard.GetLength(0) || checkingPos.x >= chipBoard.GetLength(1))
                return false; // Out of bounds.

            if (IsSlotOccupied(chipBoard, checkingPos, currentID))
                return false; // Slot not available.
        }

        return true;
    }



    public virtual bool PlaceChipAtTargetSlot(ref int[,] chipBoard, Vector2Int targetGridPos, int currentID)
    {
        if (!CanPlaceAtTargetSlot(chipBoard, targetGridPos)) return false;

        foreach (Vector2Int chipPart in blockLayout)
        {
            // invert the Y because +1 y means up on screen but not on the board. Top to bottom, 0 to X.
            Vector2Int chipPartPos = targetGridPos + new Vector2Int(chipPart.x, -chipPart.y);

            SetIDAtTargetSlot(ref chipBoard, targetGridPos, currentID);
        }

        return true;
    }

    public virtual void RemoveChipFromBoard(ref int[,] chipBoard, int currentID)
    {
        for (int y = 0; y < chipBoard.GetLength(0); y++)
        {
            for (int x = 0; x < chipBoard.GetLength(1); x++)
            {
                if (chipBoard[y, x] == currentID) chipBoard[y, x] = -1;
            }
        }
    }



    protected bool IsSlotOccupied(int[,] chipBoard, Vector2Int targetGridPos, int currentID = -1)
    {
        //0, 0 top left,
        //0, X bottom left
        //X, 0 top right
        return chipBoard[targetGridPos.y, targetGridPos.x] != -1 && chipBoard[targetGridPos.y, targetGridPos.x] != currentID;
    }

    protected void SetIDAtTargetSlot(ref int[,] chipBoard, Vector2Int targetGridPos, int currentID)
    {
        if (targetGridPos.x < 0 || targetGridPos.y < 0 || targetGridPos.y >= chipBoard.GetLength(0) || targetGridPos.x >= chipBoard.GetLength(1)) return;

        chipBoard[targetGridPos.y, targetGridPos.x] = currentID;
    }


}
