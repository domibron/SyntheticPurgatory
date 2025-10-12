using System;
using UnityEngine;

[Serializable]
public class StatData
{
    public string Key;
    public StatsCoreSO ScriptableObject;
}

public class GameStatsManager : MonoBehaviour
{
    public static GameStatsManager Instance { get => instance; }

    public static GameStatsManager instance;

    [SerializeField]
    public StatData[] baseStats = new StatData[0];

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // if (baseStats.Length > 0)
        // {
        //     // print(baseStats[0].ScriptableObject.GetType().ToString());
        //     //BetterStatsTest coreStats = baseStats[0].ScriptableObject as BetterStatsTest;
        //     //print(coreStats.Health);

        //     // BetterStatsTest better = GetStatClass<BetterStatsTest>("E");

        //     // if (better != null)
        //     //     print(GetStatClass<BetterStatsTest>("E").Health);
        //     // else
        //     //     print("Its null");
        //     // GetComponent<Transform>();
        // }
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Get the referance to the data class.
    /// </summary>
    /// <typeparam name="T">The stats class.</typeparam>
    /// <param name="key">The key associated with the class.</param>
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

    private StatData GetStatDataWithKey(string key)
    {
        foreach (var stat in baseStats)
        {
            if (stat.Key.Equals(key))
            {
                return stat;
            }
        }

        return null;
    }
}
