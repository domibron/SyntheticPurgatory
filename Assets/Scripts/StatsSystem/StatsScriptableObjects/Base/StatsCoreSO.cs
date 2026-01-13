using System;
using UnityEngine;

[Serializable]
public class UpgradablePlayerStat : ICloneable
{
    // these should be private with public getters. not get private set, these need to be serializedField for unity inspector.
    public float BaseStat = 1;
    public float? MaxStat = null;
    public float IncreaseAmount = 1; // base increase
    public float IncreasePerLevel = 1f; // percentage, 0.5f will decrease by 50 percent 1.5f will increase 50 percent.
    public int BaseCost = 1;
    public float IncreaseCostAmount = 1f;

    public string Prefix = "";
    public string Suffix = "";

    public bool IsIncreasingStat { get => IncreaseAmount > 0; }

    [ReadOnly]
    public float CurrentValue; // the current value of the stat.
    [ReadOnly]
    public int UpgradedAmount = 0; // how many times did we upgrade.
    [ReadOnly]
    public int CurrentCost; // the current cost.
    [ReadOnly]
    public float CurrentUpgradeAmount; // the current increase.
    [ReadOnly]
    public float ChipIncreaseAmount = 0f;

    private bool initilized = false;

    public UpgradablePlayerStat()
    {
        if (initilized) return;
        initilized = true;

        CurrentValue = BaseStat;
        CurrentCost = BaseCost;
        CurrentUpgradeAmount = IncreaseAmount;
    }

    public UpgradablePlayerStat(float baseVal) // If neeeded we can have a custom constructor for cloning.
    {
        if (initilized) return;
        BaseStat = baseVal;

        initilized = true;

        CurrentValue = BaseStat;
        CurrentCost = BaseCost;
        CurrentUpgradeAmount = IncreaseAmount;
    }

    public int UpgradeStat(int amount)
    {
        int count = GetHowManyTimesToUpgradeBeforeMaxing(amount);

        if (count == -1) count = amount; // we can just apply it directly. 


        // can simplify.
        (float curAmount, float incAmount) = UpgradeAmount(count);
        CurrentValue = curAmount;
        CurrentUpgradeAmount = incAmount;
        //emd.

        UpgradedAmount += count;
        CurrentCost = UpgradeCost(count, true);


        if (count == -1) return 0;
        else return amount - count;
    }

    public int GetHowManyTimesToUpgradeBeforeMaxing(int amount)
    {
        if (!MaxStat.HasValue) return -1;

        if (ExceedsMax(CurrentValue)) return 0; // 

        float temp = CurrentValue;
        float tempIncrease = CurrentUpgradeAmount;

        int count = 0;

        for (int i = 1; i <= amount; i++) // why is this different.
        {
            temp += tempIncrease;
            tempIncrease *= IncreasePerLevel;

            if (!ExceedsMax(temp))
                count++;
            else
                break;
        }

        return count;
    }

    // TODO: any checks with this needs to be reworked so it upgrades can touch the max rather than be near it.
    public bool ExceedsMax(float val)
    {
        if (!MaxStat.HasValue) return false;

        if (IsIncreasingStat)
        {
            if (val > MaxStat.Value) return true;
            else return false;
        }
        else
        {
            if (val < MaxStat.Value) return true;
            else return false;
        }
    }

    /// <summary>
    /// Get the values for upgrading the stat a set amount.
    /// </summary>
    /// <param name="amount"></param>
    /// <returns>New current amount, new increase amount.</returns>
    public (float, float) UpgradeAmount(int amount)
    {
        float curAmount = CurrentValue;
        float tempIncrease = CurrentUpgradeAmount;

        for (int i = 1; i <= amount; i++)
        {
            curAmount += tempIncrease;
            tempIncrease *= IncreasePerLevel;
        }

        return (curAmount, tempIncrease);
    }

    public int UpgradeCost(int amount, bool additonalOne = false)
    {
        int cost = CurrentCost;

        for (int i = 1; i <= amount - (additonalOne ? 0 : 1); i++)
        {
            cost += Mathf.RoundToInt(cost * IncreaseCostAmount);
        }

        return cost;
    }

    public string GetTextWithPreAndSuf(string text)
    {
        return Prefix + text + Suffix;
    }

    public void SetChipIncreaseAmount(float amount = 0)
    {
        ChipIncreaseAmount = amount;
    }

    public object Clone() // fingers crossed that initilized can work to stop setting current cost and that to base.
    {
        var clone = new UpgradablePlayerStat
        {
            BaseStat = BaseStat,
            MaxStat = MaxStat,
            IncreaseAmount = IncreaseAmount,
            IncreasePerLevel = IncreasePerLevel,
            BaseCost = BaseCost,

            Prefix = Prefix,
            Suffix = Suffix,

            CurrentValue = CurrentValue,
            UpgradedAmount = UpgradedAmount,
            CurrentCost = CurrentCost,
            CurrentUpgradeAmount = CurrentUpgradeAmount,
            ChipIncreaseAmount = ChipIncreaseAmount,

            initilized = initilized,
        };

        return clone;
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
