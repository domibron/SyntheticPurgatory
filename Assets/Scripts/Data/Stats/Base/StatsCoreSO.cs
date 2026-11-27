using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


/// <summary>
/// The base stats for anything stat related.
/// </summary>
[Serializable]
public class CoreStats : ICloneable
{
    /// <summary>
    /// Returns a array of all the upgradable stats in this stat class.
    /// </summary>
    /// <returns>Array of UpgradableStat.</returns>
    public virtual List<UpgradableStat> GetAllUpgradableStats()
    {
        return GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic) // NonPublic gets serializeFields.
            .Where(field => typeof(UpgradableStat).IsAssignableFrom(field.FieldType)) // Also checks inherited members.
            .Select(field => (UpgradableStat)field.GetValue(this))
            .Where(stat => stat != null)
            .ToList();
    }

    public virtual List<FieldInfo> GetAllStats()
    {
        return GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic) // NonPublic gets serializeFields.
            .ToList();
    }


    public void ApplyStats(object target)
    {
        FieldInfo[] fields = target.GetType().GetFields(
        BindingFlags.Public |
        BindingFlags.Instance |
        BindingFlags.NonPublic
        );

        foreach (FieldInfo field in fields)
        {
            var attribute = field.GetCustomAttribute<StatAssignableAttribute>();

            if (attribute == null) continue;

            var stat = GetAllStats().FirstOrDefault(s => s.Name == attribute.TargetStatName);

            if (stat == null)
            {
                Debug.LogError($"No matching stat for {attribute.TargetStatName}!");
                continue;
            }

            object statValue = stat.GetValue(this);

            if (statValue is UpgradableStat upgradableStat)
            {
                statValue = upgradableStat.GetCurrentValue();
            }

            if (statValue is float floatValue)
            {
                field.SetValue(target, floatValue);
            }
            else if (statValue is int intValue)
            {
                field.SetValue(target, intValue);
            }
            else
            {
                Debug.LogError($"Unsupported stat type! {stat.FieldType}");
                continue;
            }
        }
    }

    /// <summary>
    /// Resets all chip stats to 0.
    /// </summary>
    public virtual void ResetAllChipStats()
    {
        foreach (var stat in GetAllUpgradableStats())
        {
            stat.SetChipIncreaseAmount();
        }
    }

    /// <summary>
    /// Used to update the inspector and resets / update the stored values to be the correctly set value.
    /// <br />Unity does some quirky things with classes and scriptable objects so we hook into the inspector as a workaround.
    /// </summary>
    public virtual void RefreshStats()
    {
        foreach (var stat in GetAllUpgradableStats())
        {
            stat.ResetStat();
        }
    }

    /// <summary>
    /// Get a deep copy of this stat class.
    /// </summary>
    /// <returns>A copy of this stat class.</returns>
    public object Clone()
    {
        var clone = (CoreStats)MemberwiseClone();

        foreach (var field in GetType().GetFields(
        BindingFlags.Instance |
        BindingFlags.Public |
        BindingFlags.NonPublic))
        {
            if (field.GetValue(this) is UpgradableStat stat)
            {
                field.SetValue(clone, stat.Clone());
            }
        }

        return clone;
    }
}

/// <summary>
/// The stat class scriptable object with all the data as read only.
/// <br />You can only get a copy of the data to prevent accidental write to the scriptable object.
/// </summary> // Yes, I did make that mistake and reset and change all of the stats of the place by accident. Hence why it returns a read only or copy.
[CreateAssetMenu(menuName = "ScriptableObjects/Stats/BaseStats", fileName = "SO_BaseStats")]
public class StatsCoreSO : ScriptableObject
{
    // * To create your own stat class, inherit this class, then create your own stat class and inherit the CoreStats class
    // * and have a serializedField private of your stat class here.
    // [SerializeField]
    [NonSerialized]
    CoreStats stats = new CoreStats();

    // ? Wouldnt a object scriptable object work better?

    /// <summary>
    /// Get a copy of the stat data.
    /// <br /><b>NOTE:</b><i> This can be any stat class, make sure to know what type it is.</i>
    /// </summary>
    /// <returns>The stat data.</returns>
    public virtual object GetStats() // * Override this function and return the stat class you created and not a combination of this and yours.
    {
        return new CoreStats();
    }




    // DO NOT PUT ANY DATA HERE AS IT WILL BE IGNORED. USE THE STAT CLASSES INSTEAD AS THAT IS HOW DATA IS PASSED THROUGH.

    // keep empty for now.

    // ? Why the hell is this here? We should not use variables here since they will be ignored in the whole system?
    // ? Is this pre architecture code?

    // Why do we want to prevent modification? That's the question we should be answering.
    // This is a scriptable object, any modifications allowed will cause the data to be permanent if executed in play mode in the engine.
    // Basically ruining all the data and creating a mess you need to clean up.

    // Please make sure the variables that you want to access in the inspector are not able to be modified by other scripts.
    // Example below shows you one way to achieve this.

    // [SerializeField]
    // private float maxHealth = 10f;

    // public float MaxHealth { get => maxHealth; }
}
