using System;
using UnityEngine;

/// <summary>
/// Base class for all chips on the board, this handles the storing and moving of the chip.
/// <br /><b>NOTE:</b><i> Make sure to override the ModifyStats function to add your own logic.</i>
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjects/Chips/BaseChip", fileName = "SO_BaseChip")]
public class ChipSO : ScriptableObject
{



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


    /// <summary>
    /// Gets the board game object.
    /// </summary>
    /// <returns>The board game object.</returns>
    public virtual GameObject GetBoardChipObject()
    {
        return boardItem;
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
