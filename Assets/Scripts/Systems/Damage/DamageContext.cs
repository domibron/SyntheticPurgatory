
using UnityEngine;

/// <summary>
/// Damage types, feel free to replace some of the TYPE variables with your own.\n
/// Please only use one flag for damage types. Immunities can have multiple.
/// </summary>
[System.Flags]
public enum DamageTypes : byte
{
    /// <summary>
    /// DO NOT USE as a damage type!  Immunity use.
    /// </summary>
    None = 0b_0000_0000,

    /// <summary>
    /// Default damage type.
    /// </summary>
    Normal = 0b_0000_0001,

    /// <summary>
    /// Mainly used for toxic water.
    /// </summary>
    Toxic = 0b_0000_0010,

    // Spare types.

    Type3 = 0b_0000_0100,
    Type4 = 0b_0000_1000,
    Type5 = 0b_0001_0000,
    Type6 = 0b_0010_0000,
    Type7 = 0b_0100_0000,
    Type8 = 0b_1000_0000,

    /// <summary>
    /// DO NOT USE as a damage type.  Immunity use.
    /// </summary>
    All = 0b_1111_1111

}

/// <summary>
/// The context about this damage. Contains the damage, type, source and whether it is critical.
/// </summary>
public struct DamageContext
{
    /// <summary>
    /// Where is this damage coming from.
    /// </summary>
    public Vector3? Source;

    /// <summary>
    /// The type of damage.
    /// </summary>
    public DamageTypes Type;

    /// <summary>
    /// The amount of damage that will be applied.
    /// </summary>
    public float Damage;

    /// <summary>
    /// Whether this damage is a critical hit or not.
    /// </summary>
    public bool IsCritical;

    /// <summary>
    /// Create a new damage context.
    /// </summary>
    /// <param name="damage">The damage amount.</param>
    /// <param name="source">Where did this damage come from.</param>
    /// <param name="type">The type of this damage, only use one type.</param>
    /// <param name="isCritical">whether this damage a critical hit or not.</param>
    public DamageContext(float damage, Vector3? source = null, DamageTypes type = DamageTypes.Normal, bool isCritical = false)
    {
        Source = source;

        // Prevents multiple damage types to be given. Should be individual.
        if (Utils.IsSingleFlag(type))
        {
            Type = type;
        }
        else
        {
            Debug.LogError("Multiple types were applied to the damage type. Only one flag can be assigned.");
            Type = DamageTypes.Normal;
        }

        Damage = damage;
        IsCritical = isCritical;
    }
}

