using UnityEngine;

public class PlayerDisabling : MonoBehaviour
{
    /// <summary>
    /// The player movement disable type.
    /// </summary>
    public enum DisabledType : byte // Can use byte to reduce the size if we use a few values. (ideally used for structs)
    {
        // Use byte to reduce the size of the enum since we only use a handful of values.
        // Ideally use this for structs and data packing to optimise it for memory.

        None = 0b_0000_0000, // 1 << 0 shift zero to the left.
        Movement = 0b_0000_0001, // 1 << 1 shift one to the left.
        Look = 0b_0000_0010, // 1 << 2 shift two to the left.
        Combat = 0b_0000_0100, // 1 << 2 shift two to the left.

        All = 0b_1111_1111,

        // Combine both bit values. 00101 | 01100 = 01101.
        MovementAndLook = Movement | Look,

        // You can also use ^ since its a logical or. 00101 ^ 01100 = 01001.
    }

    /// <summary>
    /// The current disable state of the player movement.
    /// </summary>
    [field: SerializeField]
    public DisabledType CurrentDisabledState { get; set; } = DisabledType.None;

    public bool IsDisabled(DisabledType disabledType)
    {
        return ((byte)CurrentDisabledState & (byte)disabledType) != 0;
    }
}
