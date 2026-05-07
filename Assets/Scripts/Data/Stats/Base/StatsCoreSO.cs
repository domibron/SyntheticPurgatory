using System;
using UnityEngine;


/// <summary>
/// The base stats for anything stat related.
/// </summary>
public class CoreStats : ICloneable
{
    /// <summary>
    /// Returns a array of all the upgradable stats in this stat class.
    /// </summary>
    /// <returns>Array of UpgradableStat.</returns>
    protected virtual UpgradableStat[] GetAllUpgradableStats()
    {
        UpgradableStat[] upgradablePlayerStats =
        {
            // add your stats here.
        };

        return upgradablePlayerStats;
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
    public virtual object Clone()
    {
        var clone = new CoreStats
        {

        };


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
    CoreStats stats = new CoreStats();

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

    // Why do we want to prevent modification? That's the question we should be answering.
    // This is a scriptable object, any modifications allowed will cause the data to be permanent if executed in play mode in the engine.
    // Basically ruining all the data and creating a mess you need to clean up.

    // Please make sure the variables that you want to access in the inspector are not able to be modified by other scripts.
    // Example below shows you one way to achieve this.

    // [SerializeField]
    // private float maxHealth = 10f;

    // public float MaxHealth { get => maxHealth; }
}
