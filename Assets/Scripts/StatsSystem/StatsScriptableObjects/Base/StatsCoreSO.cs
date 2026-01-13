using System;
using UnityEngine;

[Serializable]
public class UpgradableStat
{
    public float BaseStat = 1;
    public float? MaxStat = null;
    public float IncreaseAmount;
    public float IncreasePerLevel = 1f;
    public int BaseCost;
    public float IncreaseCostAmount;

    public string Prefix = "";
    public string Suffix = "";

    public float CurrentUpgradeAmount;
    public float CurrentValue;
    public int UpgradedAmount;
    public int CurrentCost;

    private bool initilized = false;

    public void Init()
    {
        if (initilized) return;
        initilized = true;

        CurrentValue = BaseStat;
        CurrentCost = BaseCost;
        CurrentUpgradeAmount = IncreaseAmount;
    }

    public int UpgradeStat(int amount)
    {
        int count = GetHowManyTimesToUpgradeBeforeMaxing(amount);

        if (count == -1) count = amount; // we can just apply it directly. 

        // This logic needs to be replaced.
        CurrentValue += CurrentUpgradeAmount * (float)count;

        UpgradedAmount += count;
        CurrentCost = UpgradeCost(count);
        // end.

        if (count == -1) return 0;
        else return amount - count;
    }

    public int GetHowManyTimesToUpgradeBeforeMaxing(int amount)
    {
        if (!MaxStat.HasValue) return -1;

        if (CurrentValue > MaxStat.Value) return 0;

        float temp = CurrentValue;
        float tempIncrease = CurrentUpgradeAmount;

        int count = 0;

        for (int i = 1; i <= amount; i++) // why is this different.
        {
            temp += tempIncrease;
            tempIncrease *= IncreasePerLevel;

            if (temp <= MaxStat.Value)
                count++;
        }

        return count;
    }

    public int UpgradeCost(int amount)
    {
        int cost = CurrentCost;

        for (int i = 1; i < amount; i++)
        {
            cost += Mathf.RoundToInt(cost * IncreaseCostAmount);
        }

        return cost;
    }

    public string GetValueWithPreAndSuf(string text)
    {
        return Prefix + text + Suffix;
    }

}

public class CoreStats : ICloneable
{
    public object Clone()
    {
        var clone = new CoreStats
        {

        };


        return clone;
    }
}

[CreateAssetMenu(menuName = "ScriptableObjects/Stats/BaseStats", fileName = "SO_BaseStats")]
public class StatsCoreSO : ScriptableObject
{
    // [SerializeField]
    CoreStats stats = new CoreStats();

    public virtual object GetStats()
    {
        return new CoreStats();
    }

    // keep empty for now.

    // Please make sure the variables that you want to access are not able to be modified.
    // Example below shows you one way to achive this.

    // [SerializeField]
    // private float maxHealth = 10f;

    // public float MaxHealth { get => maxHealth; }
}
