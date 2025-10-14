using System;
using System.Collections.Generic;
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

    private Dictionary<Stats, object> statClasses = new Dictionary<Stats, object>();

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            SetUpStats();
        }
    }

    private void SetUpStats()
    {
        statClasses = new Dictionary<Stats, object>();

        foreach (var e in Enum.GetNames(typeof(Stats)))
        {
            StatData statData = GetStatDataWithKey(e.ToLower());

            if (statData == null)
            {
                Debug.LogError("Error trying to get stats with key: " + e.ToLower());
                continue;
            }

            Type t = GetStatClassType((Stats)Enum.Parse(typeof(Stats), e));

            statData.ScriptableObject.GetType();


            object value = Convert.ChangeType(statData.ScriptableObject.GetStats(), t);

            statClasses.Add((Stats)Enum.Parse(typeof(Stats), e), value);
        }
    }

    // TODO: have the game chip system to be involved later.

    void Start()
    {
        // MeleeEnemyStats stats = GetStats<MeleeEnemyStats>(Stats.melee);
        // stats.health = 999f;
        // print(((MeleeEnemyStats)GetStatClass<MeleeEnemyStatsSO>(Stats.melee).GetStats()).health);
        // print(stats.health);
    }

    public bool HasStats(Stats key)
    {
        return statClasses.ContainsKey(key);
    }

    public void UpdateStats<T>(Stats key, object newValue)
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

    public static T DeepCopy<T>(T original)
    {
        string json = JsonUtility.ToJson(original);
        T copy = JsonUtility.FromJson<T>(json);
        return copy;
    }

    /// <summary>
    /// Gets the type of the stats.
    /// </summary>
    /// <param name="key">The key associated with the stats.</param>
    /// <returns>The class type.</returns>
    private Type GetStatClassType(Stats key)
    {
        StatData statData = GetStatDataWithKey(key.ToString().ToLower());

        if (statData == null)
        {
            return null;
        }

        return statData.ScriptableObject.GetStats().GetType();
    }

    /// <summary>
    /// Get the referance to the data class.
    /// </summary>
    /// <typeparam name="T">The stats class.</typeparam>
    /// <param name="key">The key associated with the class.</param>
    /// <returns>The referance to the class with the key or null.</returns>
    private T GetStatClass<T>(Stats key) where T : class
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
    private T GetStatClass<T>(string key) where T : class
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
