using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stat data with associated key.
/// </summary>
[Serializable]
public class StatData
{
    /// <summary>
    /// The key to associate with the stats.
    /// </summary>
    [Tooltip("This will be auto turned into lowercase and make sure only one item exists with this key.")]
    public Stats Key;

    /// <summary>
    /// The stats for the key.
    /// </summary>
    public StatsCoreSO ScriptableObject;
}

/// <summary>
/// All the stats available.
/// </summary>
public enum Stats
{
    player,
    melee,
    ranged,
    tank,
    boss,
    miscellaneous,
}

public class RunStatsM : MonoBehaviour
{
    /// <summary>
    /// The singleton for the stat manager.
    /// </summary>
    public static RunStatsM Instance { get; private set; }

    /// <summary>
    /// All the base stats. The are not modified and do not change.
    /// </summary>
    [SerializeField]
    private StatData[] baseStats = new StatData[0]; // This store the raw stat classes, aka the scriptable objects.

    /// <summary>
    /// The stats that have been loaded and are modified at runtime.
    /// </summary>
    private Dictionary<Stats, object> statClasses = new Dictionary<Stats, object>();

    void Awake()
    {
        // Run manager automatically removes the other copy so we can just override it.

        Instance = this;
        SetUpStats();
    }

    /// <summary>
    /// Load the stats into the <see cref="statClasses"/>.
    /// </summary>
    private void SetUpStats()
    {
        statClasses = new Dictionary<Stats, object>();

        foreach (var e in Enum.GetValues(typeof(Stats)))
        {
            Stats statsKey = (Stats)e;

            StatData statData = GetStatDataWithKey(statsKey);

            if (statData == null)
            {
                Debug.LogError("Error trying to get stats with key: " + statsKey);
                continue;
            }

            Type t = GetStatClassType(statsKey);

            statData.ScriptableObject.GetType(); // what is the point of this line?
                                                 // is it like, a alternative to the function?


            object value = Convert.ChangeType(statData.ScriptableObject.GetStats(), t);

            statClasses.Add(statsKey, value);
        }
    }

    // TODO: have the game chip system to be involved later.

    /// <summary>
    /// Does the key exist in the <see cref="statClasses"/>. 
    /// </summary>
    /// <param name="key">The key to check for.</param>
    /// <returns>True if the key was found.</returns>
    public bool HasStats(Stats key)
    {
        return statClasses.ContainsKey(key);
    }

    // TODO: prevent wrongful assignment of miss matching stats.
    /// <summary>
    /// Replace the current stats with the new one. (Does not confirm matching types)
    /// </summary>
    /// <typeparam name="T">The stat class type.</typeparam>
    /// <param name="key">The key to replace the stats for.</param>
    /// <param name="newValue">The new stats to replace with.</param>
    public void UpdateStats<T>(Stats key, object newValue) where T : class
    {
        if (!statClasses.ContainsKey(key))
        {
            Debug.LogError("Error trying to get stats with key: " + key.ToString().ToLower());
            return;
        }

        // copy data.
        T copy = DeepCopy<T>((T)Convert.ChangeType(newValue, typeof(T)));

        statClasses[key] = copy;
    }

    /// <summary>
    /// Get the stats with the given key.
    /// </summary>
    /// <typeparam name="T">The stat class type.</typeparam>
    /// <param name="key">The key to retrieve the stats for.</param>
    /// <returns>A copy of the stat class as a object.</returns>
    public T GetStats<T>(Stats key) where T : class
    {
        if (!statClasses.ContainsKey(key))
        {
            Debug.LogError("Error trying to get stats with key: " + key.ToString().ToLower());
            return null;
        }

        object stats = statClasses[key];

        T statsSO = (T)Convert.ChangeType(stats, typeof(T));

        return DeepCopy<T>(statsSO);
    }

    /// <summary>
    /// Deep copies the stat class.
    /// </summary>
    /// <typeparam name="T">The stat class type.</typeparam>
    /// <param name="original">The original data to copy.</param>
    /// <returns>A copy of the data.</returns>
    public static T DeepCopy<T>(T original) where T : class
    {
        string json = JsonUtility.ToJson(original);

#if false // used to debug the stats to makes sure values are being copied.
        string path = Application.persistentDataPath;
        string fullName = path + "/" + (DateTime.Now.ToString("dd-MM-yy-hh-mm-ss.ffff") + ".txt");
        print(fullName);
        StreamWriter steamWriter = File.CreateText(fullName);
        steamWriter.Write(json);
        steamWriter.Close();
#endif

        T copy = JsonUtility.FromJson<T>(json);
        return copy;
    }

    /// <summary>
    /// Gets the type of the stats.
    /// </summary>
    /// <param name="key">The key to get the stats type for.</param>
    /// <returns>The class type for the key.</returns>
    private Type GetStatClassType(Stats key)
    {
        StatData statData = GetStatDataWithKey(key);

        if (statData == null)
        {
            return null;
        }

        return statData.ScriptableObject.GetStats().GetType();
    }

    /// <summary>
    /// Get the reference to the data class.
    /// </summary>
    /// <typeparam name="T">The stats class.</typeparam>
    /// <param name="key">The key associated with the class.</param>
    /// <returns>The reference to the class with the key or null.</returns>
    private T GetStatClass<T>(Stats key) where T : class
    {
        StatData statData = GetStatDataWithKey(key);

        if (statData == null)
        {
            return null;
        }

        Type clasType = statData.ScriptableObject.GetType();

        if (clasType != typeof(T))
        {
            return null;
        }

        object value = statData.ScriptableObject;

        return (T)Convert.ChangeType(value, typeof(T));
    }

    // /// <summary>
    // /// Get the referance to the data class. (This is a optional method, use the Stats enum ideally)
    // /// </summary>
    // /// <typeparam name="T">The stats class.</typeparam>
    // /// <param name="key">The key associated with the class. (Will turn into lowercase)</param>
    // /// <returns>The referance to the class with the key or null.</returns>
    // private T GetStatClass<T>(strSting key) where T : class
    // {
    //     StatData statData = GetStatDataWithKey(key);

    //     if (statData == null)
    //     {
    //         return null;
    //     }

    //     Type clasType = statData.ScriptableObject.GetType();

    //     if (clasType != typeof(T))
    //     {
    //         return null;
    //     }

    //     object value = statData.ScriptableObject;

    //     return (T)Convert.ChangeType(value, typeof(T));
    // }

    // * NOTE: This function below is for loading the base from file so they can be used else where and have a modifiable copy at runtime. 

    /// <summary>
    /// Gets the stats data with the given key or null.
    /// </summary>
    /// <param name="key">This will turn the key into lower case.</param>
    /// <returns>The StatData or Null.</returns>
    private StatData GetStatDataWithKey(Stats key)
    {
        foreach (var stat in baseStats)
        {
            if (stat.Key == key)
            {
                return stat;
            }
        }

        return null;
    }
}
