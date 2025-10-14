using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// public class UpgradeInfo

public class UpgradeSystem : MonoBehaviour
{
    public class UpgradeData
    {
        public object ClassData;
        public FieldInfo FieldInfo;

        public UpgradeData(object classData, FieldInfo fieldInfo)
        {
            ClassData = classData;
            FieldInfo = fieldInfo;
        }
    }



    [SerializeField]
    private Transform upgradeItemSpawnPoint;

    [SerializeField]
    private GameObject upgradeItem;

    private Dictionary<string, UpgradeData> upgrades = new Dictionary<string, UpgradeData>();

    [SerializeField]
    private UpgradeAmountsSO upgradeAmountsSO;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // classes here
        PlayerStats pStats;
        pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

        foreach (var field in pStats.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
        {
            // print(field.Name);
            upgrades.Add(nameof(PlayerStats) + "|" + field.Name, new UpgradeData(pStats, field));
        }

        // Set up UI.

        foreach (var key in upgrades.Keys)
        {
            UpgradeItemUI upgradeItemUI = Instantiate(upgradeItem, upgradeItemSpawnPoint).GetComponent<UpgradeItemUI>();
            upgradeItemUI.SetUp(upgrades[key].FieldInfo.Name, upgrades[key].FieldInfo.GetValue(upgrades[key].ClassData).ToString(), key, this);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool UpgradeItem(string key, out string value)
    {
        string[] keys = key.Split('|');
        string className = keys[0];


        value = string.Empty;
        // if (GameManager.Instance.GetCurrentScrapCount() < GetUpgradeCost(key)) return false; // min buy amount.

        if (className == nameof(PlayerStats)) // Add classes here too.
        {
            // if (upgrades[key].FieldInfo.FieldType == typeof(int))
            UpgradeBasedOnType<PlayerStats>(key, Stats.player, GetUpgradeAmount(key));

            // else if (upgrades[key].FieldInfo.FieldType == typeof(float))
            //     UpgradeStat<PlayerStats>(key, Stats.player, 1);



            value = upgrades[key].FieldInfo.GetValue(upgrades[key].ClassData).ToString();
            return true;

        }

        return false;

    }

    private float GetUpgradeAmount(string key)
    {
        if (upgradeAmountsSO.UpgradeAmounts.Count <= 0) return 1f;

        string[] keys = key.Split('|');
        string className = keys[0];
        string variableName = keys[1];

        foreach (var upgradeAmount in upgradeAmountsSO.UpgradeAmounts)
        {
            if (upgradeAmount.ClassName == className && upgradeAmount.VariableName == variableName)
            {
                return upgradeAmount.IncreaseAmount;
            }
        }

        return 1f;
    }

    private float GetUpgradeCost(string key)
    {
        if (upgradeAmountsSO.UpgradeAmounts.Count <= 0) return 1;

        string[] keys = key.Split('|');
        string className = keys[0];
        string variableName = keys[1];

        foreach (var upgradeAmount in upgradeAmountsSO.UpgradeAmounts)
        {
            if (upgradeAmount.ClassName == className && upgradeAmount.VariableName == variableName)
            {
                return upgradeAmount.UpgradeCost;
            }
        }

        return 1;
    }

    private bool UpgradeBasedOnType<T>(string key, Stats statsKey, float amount) where T : class
    {
        if (upgrades[key].FieldInfo.FieldType == typeof(int))
            return UpgradeStat<T>(key, Stats.player, (int)amount);

        else if (upgrades[key].FieldInfo.FieldType == typeof(float))
            return UpgradeStat<T>(key, Stats.player, amount);

        return false;
    }

    // private void UpdateAllStats()
    // {
    //     PlayerStats pStats;
    //     pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

    //     foreach (var field in pStats.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
    //     {
    //         // print(field.Name);
    //         if (!upgrades.ContainsKey(nameof(PlayerStats) + "|" + field.Name))
    //             upgrades.Add(nameof(PlayerStats) + "|" + field.Name, new UpgradeData(pStats, field));
    //         else
    //             upgrades[nameof(PlayerStats) + "|" + field.Name] = new UpgradeData(pStats, field);
    //     }
    // }

    private bool UpgradeStat<T>(string key, Stats statsKey, object newValue) where T : class
    {
        if (!upgrades.ContainsKey(key)) return false;

        if (upgrades[key].FieldInfo.FieldType == typeof(float) && newValue.GetType() == typeof(float))
        {
            UpdateStatManager<T>(key, statsKey, (float)upgrades[key].FieldInfo.GetValue(upgrades[key].ClassData) + (float)newValue);
            // UpdateAllStats();
            return true;
        }
        else if (upgrades[key].FieldInfo.FieldType == typeof(int) && newValue.GetType() == typeof(int))
        {
            UpdateStatManager<T>(key, statsKey, (int)upgrades[key].FieldInfo.GetValue(upgrades[key].ClassData) + (int)newValue);
            // UpdateAllStats();
            return true;
        }
        else if (upgrades[key].FieldInfo.FieldType == typeof(float) && newValue.GetType() == typeof(int))
        {
            UpdateStatManager<T>(key, statsKey, (float)upgrades[key].FieldInfo.GetValue(upgrades[key].ClassData) + (int)newValue);
            // UpdateAllStats();
            return true;
        }
        // else if (upgrades[key].FieldInfo.FieldType == typeof(int) && newValue.GetType() == typeof(float))
        // {
        //     UpdateStatManager<T>(key, statsKey, (int)upgrades[key].FieldInfo.GetValue(upgrades[key].ClassData) + (float)newValue);
        //     // UpdateAllStats();
        //     return true;
        // }
        else
        {
            return false;
        }
    }

    private void UpdateStatManager<T>(string key, Stats statsKey, object newValue) where T : class
    {
        upgrades[key].FieldInfo.SetValue(upgrades[key].ClassData, newValue);
        GameStatsManager.Instance.UpdateStats<T>(statsKey, upgrades[key].ClassData);
    }

    // private bool UpgradeStat<T>(string key, Stats statsKey, int newValue) where T : class
    // {
    //     if (!upgrades.ContainsKey(key)) return false;

    //     if (upgrades[key].FieldInfo.FieldType == typeof(int))
    //     {
    //         upgrades[key].FieldInfo.SetValue(upgrades[key].ClassData, (int)upgrades[key].FieldInfo.GetValue(upgrades[key].ClassData) + newValue);
    //         GameStatsManager.Instance.UpdateStats<T>(statsKey, upgrades[key].ClassData);
    //         // UpdateAllStats();
    //         return true;
    //     }
    //     else
    //     {
    //         return false;
    //     }
    // }
}
