using System;
using UnityEngine;

[Serializable]
public class UpgradablePlayerStat : ICloneable
{
    // NOTE, ANY PRIVATE VARIABLES WILL NOT BE ABLE TO BE DEEP COPIED USING THE JSON METHOD!

    // these should be private with public getters. not get private set, these need to be serializedField for unity inspector.
    public float BaseStat = 1;
    public bool IsThereAMax = false;
    public float MaxStat = 0;
    public float IncreaseAmount = 1; // base increase
    public float IncreasePerLevel = 1f; // percentage, 0.5f will decrease by 50 percent 1.5f will increase 50 percent.
    public int BaseCost = 1;
    public float IncreaseCostAmount = 1f;

    public string Prefix = "";
    public string Suffix = "";
    public string StringModifiers = "";

    public string StatName = "Name";
    [TextArea]
    public string StatDescription = "Description";

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

    public UpgradablePlayerStat()
    {
        CurrentValue = BaseStat;
        CurrentCost = BaseCost;
        CurrentUpgradeAmount = IncreaseAmount;
    }

    public UpgradablePlayerStat(float baseVal) // If neeeded we can have a custom constructor for cloning.
    {
        BaseStat = baseVal;

        CurrentValue = BaseStat;
        CurrentCost = BaseCost;
        CurrentUpgradeAmount = IncreaseAmount;
    }

    public void ResetStat()
    {
        UpgradedAmount = 0;
        CurrentValue = BaseStat;
        CurrentCost = BaseCost;
        CurrentUpgradeAmount = IncreaseAmount;
    }

    public int UpgradeStat(int amount = 1)
    {
        int count = GetHowManyTimesToUpgradeBeforeMaxing(amount);

        if (count == -1) count = amount; // we can just apply it directly. 

        // the system should be redundent but just in case.
        if (count == 0) return amount - count;

        // can simplify.
        (float curAmount, float incAmount) = GetUpgradeAmounts(count);
        CurrentValue = curAmount;
        CurrentUpgradeAmount = incAmount;
        //emd.

        UpgradedAmount += count;
        CurrentCost = IncreaseUpgradeCost(count);


        if (count == -1) return 0;
        else return amount - count;
    }

    public int GetHowManyTimesToUpgradeBeforeMaxing(int amount = 1)
    {
        if (!IsThereAMax) return -1;

        if (IsExceedingMax(CurrentValue)) return 0; // 

        float temp = CurrentValue;
        float tempIncrease = CurrentUpgradeAmount;

        int count = 0;

        for (int i = 1; i <= amount; i++) // why is this different.
        {
            temp += tempIncrease;
            tempIncrease *= IncreasePerLevel;

            if (!IsExceedingMax(temp))
                count++;
            else
                break;
        }

        return count;
    }

    // TODO: any checks with this needs to be reworked so it upgrades can touch the max rather than be near it.
    public bool IsExceedingMax(float val)
    {
        if (!IsThereAMax) return false;

        if (IsIncreasingStat)
        {
            if (val > MaxStat) return true;
            else return false;
        }
        else
        {
            if (val < MaxStat) return true;
            else return false;
        }
    }

    /// <summary>
    /// Get the values for upgrading the stat a set amount.
    /// </summary>
    /// <param name="amount"></param>
    /// <returns>New current amount, new increase amount.</returns>
    public (float, float) GetUpgradeAmounts(int amount = 1)
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

    public int UpgradeCost(int amount = 1, bool additonalOne = false)
    {
        int cost = CurrentCost;
        int tempCostHolder = CurrentCost;

        int amountToItterate = amount - (additonalOne ? 0 : 1);

        //amountToItterate = GetHowManyTimesToUpgradeBeforeMaxing(amountToItterate);

        for (int i = 1; i <= amountToItterate; i++)
        {
            tempCostHolder = Mathf.RoundToInt(tempCostHolder * IncreaseCostAmount);
            cost += tempCostHolder;
        }

        return cost;
    }

    public int IncreaseUpgradeCost(int amount = 1)
    {
        int cost = CurrentCost;

        for (int i = 1; i <= amount; i++)
        {
            cost = Mathf.RoundToInt(cost * IncreaseCostAmount);
        }

        return cost;
    }

    public string GetTextWithPreAndSuf(string text)
    {
        return Prefix + text + Suffix;
    }

    public string GetValueWithPreAndSuf()
    {
        return Prefix + CurrentValue.ToString(StringModifiers) + Suffix;
    }

    public string GetName()
    {
        return StatName;
    }

    public string GetDescription()
    {
        return StatDescription;
    }

    public void SetChipIncreaseAmount(float amount = 0)
    {
        ChipIncreaseAmount = amount;
    }

    public float GetCurrentValue()
    {
        return CurrentValue + ChipIncreaseAmount;
    }

    public object Clone() // fingers crossed that initilized can work to stop setting current cost and that to base.
    {
        var clone = new UpgradablePlayerStat
        {
            BaseStat = BaseStat,
            IsThereAMax = IsThereAMax,
            MaxStat = MaxStat,
            IncreaseAmount = IncreaseAmount,
            IncreasePerLevel = IncreasePerLevel,
            BaseCost = BaseCost,
            IncreaseCostAmount = IncreaseCostAmount,

            Prefix = Prefix,
            Suffix = Suffix,
            StringModifiers = StringModifiers,

            StatName = StatName,
            StatDescription = StatDescription,


            CurrentValue = CurrentValue,
            UpgradedAmount = UpgradedAmount,
            CurrentCost = CurrentCost,
            CurrentUpgradeAmount = CurrentUpgradeAmount,
            ChipIncreaseAmount = ChipIncreaseAmount,

            // initilized = initilized,
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
