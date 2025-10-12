using System;
using UnityEngine;

[Serializable]
public class StatData
{
    [Tooltip("This will be auto turned into lowercase and make sure only one item exsists with this key.")]
    public string Key;
    public StatsCoreSO ScriptableObject;
}

public enum Stats
{
    player,
    melee,
    ranged,
    tank,
    boss,
}

public class GameStatsManager : MonoBehaviour
{
    public static GameStatsManager Instance { get => instance; }

    private static GameStatsManager instance;

    [SerializeField]
    private StatData[] baseStats = new StatData[0];

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // TODO: have the game chip system to be involved later.

    /// <summary>
    /// Gets the type of the stats.
    /// </summary>
    /// <param name="key">The key associated with the stats.</param>
    /// <returns>The class type.</returns>
    public Type GetStatType(Stats key)
    {
        StatData statData = GetStatDataWithKey(key.ToString().ToLower());

        if (statData == null)
        {
            return null;
        }

        return statData.ScriptableObject.GetType();
    }

    /// <summary>
    /// Get the referance to the data class.
    /// </summary>
    /// <typeparam name="T">The stats class.</typeparam>
    /// <param name="key">The key associated with the class.</param>
    /// <returns>The referance to the class with the key or null.</returns>
    public T GetStatClass<T>(Stats key) where T : class
    {
        StatData statData = GetStatDataWithKey(key.ToString().ToLower());

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

    /// <summary>
    /// Get the referance to the data class. (This is a optional method, use the Stats enum ideally)
    /// </summary>
    /// <typeparam name="T">The stats class.</typeparam>
    /// <param name="key">The key associated with the class. (Will turn into lowercase)</param>
    /// <returns>The referance to the class with the key or null.</returns>
    public T GetStatClass<T>(string key) where T : class
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

    /// <summary>
    /// Gets the stats data with the given key or null.
    /// </summary>
    /// <param name="key">This will turn the key into lower case.</param>
    /// <returns>The StatData or Null.</returns>
    private StatData GetStatDataWithKey(string key)
    {
        foreach (var stat in baseStats)
        {
            if (stat.Key.ToLower().Equals(key.ToLower()))
            {
                return stat;
            }
        }

        return null;
    }
}
