using System;
using UnityEngine;

/// <summary>
/// Base class for all chips on the board, this handles the storing and moving of the chip.
/// <br /><b>NOTE:</b><i> Make sure to override the ModifyStats function to add your own logic.</i>
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjects/Chips/BaseChip", fileName = "SO_BaseChip")]
public class ChipSO : ScriptableObject
{
    /// <summary>
    /// The X represented in the 2d array.
    /// </summary>
    public const int X_ROW = 1;

    /// <summary>
    /// The Y represented in the 2d array.
    /// </summary>
    public const int Y_ROW = 0;


    [SerializeField]
    string chipName = "Name";

    [SerializeField, TextArea]
    string chipDescription = "Description";

    /// <summary>
    /// The placement of each square segment on the chip, 0,0 is where the square the cursor is.
    /// </summary>
    [SerializeField]
    Vector2Int[] blockLayout = { Vector2Int.zero };

    /// <summary>
    /// The max size of the chip. Not Automated from the blockLayout.
    /// </summary>
    [SerializeField]
    Vector2Int size = Vector2Int.one;

    /// <summary>
    /// The image for the list view of chips.
    /// </summary>
    [SerializeField]
    Sprite inventoryImage = null;

    /// <summary>
    /// The grid item of the chip. Used for placement on the board.
    /// </summary>
    [SerializeField]
    GameObject boardItem;

    /// <summary>
    /// Whether you are using a generative chip board item or it's already made.
    /// </summary>
    [SerializeField]
    private bool hasAGenerativeBoardItem = true;

    /// <summary>
    /// The colour of the generative chip board item.
    /// </summary>
    [SerializeField]
    private Color generatedColor = Color.red;

    /// <summary>
    /// This is called to modify the stats per chip. Presume the current stored chip values have been reset and add to the variable.
    /// <br /><b>NOTE:</b><i> Override this function to run your own logic / code to change the stats. Remember this can be out of order.</i>
    /// </summary>
    /// <param name="pStats">The player stats to modify.</param>
    /// <param name="miscStats">The miscellaneous stats to modify.</param>
    public virtual void ModifyStats(ref PlayerStats pStats, ref MiscellaneousStats miscStats)
    {
        // Modify the stats, remember to add to the chip stats. They will all be reset to 0 before this is called.
        // pStats.MaxHealthStat.AddToChipIncreaseAmount(pStats.MaxHealthStat.CurrentValue * 1f); // <- EXAMPLE
    }

    /// <summary>
    /// Gets the total size of the chip.
    /// </summary>
    /// <returns>The size of the chip.</returns>
    public virtual Vector2Int GetSize()
    {
        return size;
    }

    // TODO: decide if this is worth to keep.
    /// <summary>
    /// Returns the inventory image for this chip.
    /// </summary>
    /// <returns>The sprite image for the chip.</returns>
    public virtual Sprite GetInventoryImage()
    {
        return inventoryImage;
    }

    // TODO: decide if this is worth to keep.
    /// <summary>
    /// Gets the board game object.
    /// </summary>
    /// <returns>The board game object.</returns>
    public virtual GameObject GetBoardChipObject()
    {
        return boardItem;
    }

    /// <summary>
    /// Instantiates the board item and returns a reference to the game object that was created.
    /// </summary>
    /// <returns>Reference to the created board game object.</returns>
    public virtual GameObject CreateAndReturnBoardItem()
    {
        GameObject go = Instantiate(boardItem);

        if (IsGenerativeBoardItem()) go.GetComponent<GenerateChip>().GenerateVisualChip(this);

        return go;
    }

    /// <summary>
    /// Gets the colour to use for the generative board item.
    /// </summary>
    /// <returns>The colour the generated board item should be.</returns>
    public virtual Color GetGenerativeColor()
    {
        return generatedColor;
    }

    /// <summary>
    /// Returns whether this is using the generative board item or a custom board item.
    /// </summary>
    /// <returns>True if it is using a generative board item.</returns>
    public bool IsGenerativeBoardItem()
    {
        return hasAGenerativeBoardItem;
    }

    /// <summary>
    /// Checks whether this chip can be placed at that target grid coordinates.
    /// </summary>
    /// <param name="chipBoard">A reference to the chip board to check on.</param>
    /// <param name="targetGridPos">The target placement of the chip on the board.</param>
    /// <param name="currentID">The id of the chip if it's already placed on the board.</param>
    /// <returns>True if the chip can be placed at the target coordinates.</returns>
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


    /// <summary>
    /// Checks to see if the chip can be placed and tries to place the chip at the target grid coordinates if able.
    /// </summary>
    /// <param name="chipBoard">The reference to the board to modify / place the chip on.</param>
    /// <param name="targetGridPos">The target coordinates of the chip on the board.</param>
    /// <param name="currentID">The id to write to the board to represent this chip.</param>
    /// <returns>True if the operation was successful.</returns>
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

    /// <summary>
    /// Removes the chip from the board.
    /// </summary>
    /// <param name="chipBoard">The reference to the board to modify.</param>
    /// <param name="currentID">The id of the chip to remove.</param>
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

    /// <summary>
    /// Checks of the single slot is occupied at the target coordinates on the board.
    /// </summary>
    /// <param name="chipBoard">A reference to the board to check on.</param>
    /// <param name="targetGridPos">The target slot coordinates.</param>
    /// <param name="currentID">The id of the chip to ignore.</param>
    /// <returns>True if the slot is occupied.</returns>
    protected bool IsSlotOccupied(ref int[,] chipBoard, Vector2Int targetGridPos, int currentID = -1)
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
    protected void SetIDAtTargetSlot(ref int[,] chipBoard, Vector2Int targetGridPos, int currentID)
    {
        if (targetGridPos.x < 0 || targetGridPos.y < 0 || targetGridPos.y >= chipBoard.GetLength(Y_ROW) || targetGridPos.x >= chipBoard.GetLength(X_ROW)) return;

        chipBoard[targetGridPos.y, targetGridPos.x] = currentID;
    }

    /// <summary>
    /// Gets the block layer, a collections of positions the chip occupies.
    /// </summary>
    /// <returns>The chip layout as a array of Vector2Int.</returns>
    public virtual Vector2Int[] GetBlockLayout()
    {
        return blockLayout; // TODO: prevent direct ref. Return only copy.
    }


    public string GetNameOfChip()
    {
        return chipName;
    }

    public string GetDescriptionOfChip()
    {
        return chipDescription;
    }
}
