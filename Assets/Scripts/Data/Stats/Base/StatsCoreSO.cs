using System;
using UnityEngine;

[Serializable]
public class UpgradablePlayerStat : ICloneable
{
    // NOTE, ANY PRIVATE VARIABLES WILL NOT BE ABLE TO BE DEEP COPIED USING THE JSON METHOD!
    // ? We have I Cloneable what are we using json for? are we using that to read data? store data? override values?

    // these should be private with public getters. not get private set, these need to be serializedField for unity inspector.
    // ! But if they are private and we are using the JSON method of copying then it wont copy over.
    // NOTE: This is discussing setting the public variables as private so scripts cannot modify the value directly.
    // * Systems have been built already modifying the values directly. This seems pointless to change now.

    /// <summary>
    /// The base value of the stat.
    /// </summary>
    public float BaseStat = 1;

    /// <summary>
    /// Enables a max for the stat.
    /// </summary>
    public bool StatHasMax = false;

    /// <summary>
    /// The max value the stat can be. If decreasing, this is the minimum.
    /// </summary>
    public float MaxStat = 0;

    /// <summary>
    /// When upgrading how much to increase by, negative values decrease.
    /// <br /><b>NOTE:</b><i> When setting this to a value less than 0, this will be marked as a decreasing stat automatically.</i>
    /// </summary>
    public float IncreaseAmount = 1; // base increase

    /// <summary>
    /// How much to increase the increase / upgrade amount percentage wise. This is a exponential operation.
    /// </summary>
    public float IncreasePerLevel = 1f; // percentage, 0.5f will decrease by 50 percent 1.5f will increase 50 percent.

    /// <summary>
    /// The base cost for the stat.
    /// </summary>
    public int BaseCost = 1;

    /// <summary>
    /// How much the stat cost increases each time it is upgraded.
    /// </summary>
    public float IncreaseCostAmount = 1f;

    /// <summary>
    /// Prefix value for displaying this stat.
    /// </summary>
    public string Prefix = "";

    /// <summary>
    /// Suffix value for displaying this stat.
    /// </summary>
    public string Suffix = "";

    /// <summary>
    /// ToString modifiers, like F2, N0, P1 etc. Use microsoft's documentation on standard numeric format strings.
    /// </summary>
    public string StringModifiers = "";


    public string StatName = "Name";
    [TextArea]
    public string StatDescription = "Description";

    /// <summary>
    /// Gets whether this stat is increasing stat when upgraded or decreasing.
    /// </summary>
    public bool IsIncreasingStat { get => IncreaseAmount > 0; }


    // ****************************************
    // *              READ ONLY               *
    // *            RUNTIME VALUES            *
    // ****************************************

    /// <summary>
    /// The current value of the stat.
    /// </summary>
    [ReadOnly]
    public float CurrentValue;

    /// <summary>
    /// The amount of times the stat was upgraded.
    /// </summary>
    [ReadOnly]
    public int UpgradedAmount;

    /// <summary>
    /// The current cost for the stat.
    /// </summary>
    [ReadOnly]
    public int CurrentCost;

    /// <summary>
    /// The current upgrade increase amount for the stat.
    /// </summary>
    [ReadOnly]
    public float CurrentUpgradeAmount;

    /// <summary>
    /// The current chip increase amount affecting this stat.
    /// </summary>
    [ReadOnly]
    public float ChipIncreaseAmount;


    // ****************************************
    // *            CONSTRUCTORS              *
    // ****************************************

    /// <summary>
    /// Sets the current values to the base amount when created.
    /// </summary>
    public UpgradablePlayerStat()
    {
        CurrentValue = BaseStat;
        CurrentCost = BaseCost;
        CurrentUpgradeAmount = IncreaseAmount;
    }

    /// <summary>
    /// Creates a stat with a base value, useful for setting the starting value if something like the stats manager fails.
    /// </summary>
    /// <param name="baseVal">The base value to set.</param>
    public UpgradablePlayerStat(float baseVal) // If needed we can have a custom constructor for cloning.
    {
        BaseStat = baseVal;

        CurrentValue = BaseStat;
        CurrentCost = BaseCost;
        CurrentUpgradeAmount = IncreaseAmount;
    }


    // ****************************************
    // *            MODIFICATION              *
    // ****************************************

    /// <summary>
    /// Sets the stats to the base amount and removes all upgrades.
    /// <br />This does not reset the chip modifiers.
    /// </summary>
    public void ResetStat()
    {
        UpgradedAmount = 0;
        CurrentValue = BaseStat;
        CurrentCost = BaseCost;
        CurrentUpgradeAmount = IncreaseAmount;
    }

    /// <summary>
    /// Upgrades the stat by the given amount or 1 if left empty.
    /// </summary>
    /// <param name="amount">The amount of times to upgrade this stat.</param>
    /// <returns>The remaining amount of upgrades when reaching the max.</returns>
    public int UpgradeStat(int amount = 1)
    {
        int count = GetMaxUpgradeCountPossible(amount);

        // Checks to see if there is no max. If so, the count is set to the amount.
        if (count == -1) count = amount; // we can just apply it directly. 

        // the system should be redundant but just in case.
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

    /// <summary>
    /// Sets the chip modifier amount. If left empty, this will set the chip modifier to 0 basically resetting it.
    /// </summary>
    /// <param name="amount">The value to set the chip modifier.</param>
    public void SetChipIncreaseAmount(float amount = 0)
    {
        ChipIncreaseAmount = amount;
    }

    /// <summary>
    /// Adds the the chip modifier.
    /// </summary>
    /// <param name="amount">The amount to add to the chip modifier.</param>
    public void AddToChipIncreaseAmount(float amount)
    {
        ChipIncreaseAmount += amount;
    }

    // ****************************************
    // *              GET DATA                *
    // ****************************************

    // TODO: any checks with this needs to be reworked so it upgrades can touch the max rather than be near it.
    /// <summary>
    /// Does this value exceed the max value. Works for both increasing and decreasing stats.
    /// </summary>
    /// <param name="val">The value to compare.</param>
    /// <returns>True if this value exceeds the max. Will return False if the is no max.</returns>
    public bool IsExceedingMax(float val)
    {
        if (!StatHasMax) return false;

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
    /// Get the amount you can upgrade the stat before it reaches the max value.
    /// </summary>
    /// <param name="amount">The amount you want to upgrade by.</param>
    /// <returns>The amount you can upgrade before reaching the max.</returns>
    public int GetMaxUpgradeCountPossible(int amount = 1)
    {
        if (!StatHasMax) return -1;

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

    /// <summary>
    /// Get the values for upgrading the stat a set amount.
    /// </summary>
    /// <param name="amount">The amount to upgrade by.</param>
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

    /// <summary>
    /// Gets the cost of the upgrade with the given amount.
    /// </summary>
    /// <param name="amount">The amount to upgrade by.</param>
    /// <param name="additionalOne">Add an additional one upgrade.</param>
    /// <returns>The cost to reach the upgrade amount.</returns>
    public int UpgradeCost(int amount = 1, bool additionalOne = false)
    {
        int cost = CurrentCost;
        int tempCostHolder = CurrentCost;

        int amountToIterate = amount - (additionalOne ? 0 : 1);

        //amountToIterate = GetHowManyTimesToUpgradeBeforeMaxing(amountToIterate);

        for (int i = 1; i <= amountToIterate; i++)
        {
            tempCostHolder = Mathf.RoundToInt(tempCostHolder * IncreaseCostAmount);
            cost += tempCostHolder;
        }

        return cost;
    }

    /// <summary>
    /// Get the cost value after the upgrade amount.
    /// </summary>
    /// <param name="amount">The amount to upgrade by.</param>
    /// <returns>The cost after the upgrade amount.</returns>
    public int IncreaseUpgradeCost(int amount = 1)
    {
        int cost = CurrentCost;

        for (int i = 1; i <= amount; i++)
        {
            cost = Mathf.RoundToInt(cost * IncreaseCostAmount);
        }

        return cost;
    }


    //TODO: replace as ToString.
    /// <summary>
    /// Gets the stat value as a display text.
    /// </summary>
    /// <returns>The display text.</returns>
    public string GetValueWithPreAndSuf()
    {
        return Prefix + GetCurrentValue().ToString(StringModifiers) + Suffix;
    }

    /// <summary>
    /// Get the name of the stat.
    /// </summary>
    /// <returns></returns>
    public string GetName()
    {
        return StatName;
    }

    /// <summary>
    /// Get the stat's description.
    /// </summary>
    /// <returns></returns>
    public string GetDescription()
    {
        return StatDescription;
    }


    /// <summary>
    /// Get the current value with chip modifiers. Both are combined and compared against the max.
    /// </summary>
    /// <returns>The final stat value.</returns>
    public float GetCurrentValue()
    {
        // returns the max if current exceeds the max or the current value.
        return GetValueOrMax(CurrentValue + ChipIncreaseAmount);

        // This does look like it's old since the chip should be the correct value but additional checks would not hurt.

        // Obsolete code.
        // * im not sure about this #/#/# (weeks ago) - old 2/2/26
        // if (IsIncreasingStat)
        // return CurrentValue + ChipIncreaseAmount;
        // else
        //     return CurrentValue - ChipIncreaseAmount;
    }

    /// <summary>
    /// Returns the max value if the value exceeds the max otherwise it returns the value.
    /// </summary>
    /// <param name="value">The value to compare against the max.</param>
    /// <returns>The value clamped blow / above the max.</returns>
    public float GetValueOrMax(float value)
    {
        if (!StatHasMax) return value; // this is handled inside the isExceedingMax check, doubled just in case of future changes.
        // performance at this level does not matter.
        // huh? you know what needs more performance, the level rendering.

        if (IsExceedingMax(value))
        {
            return MaxStat;
        }

        return value;
    }


    // ****************************************
    // *               CLONING                *
    // ****************************************

    /// <summary>
    /// Returns a copy of this stat with all the values.
    /// </summary>
    /// <returns>The stats as a copy.</returns>
    public object Clone()
    {
        var clone = new UpgradablePlayerStat
        {
            BaseStat = BaseStat,
            StatHasMax = StatHasMax,
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
        };

        return clone;
    }
}

/// <summary>
/// The base stats for anything stat related.
/// </summary>
public class CoreStats : ICloneable
{
    protected virtual UpgradablePlayerStat[] GetAllUpgradableStats()
    {
        UpgradablePlayerStat[] upgradablePlayerStats =
        {
            // add your stats here.
        };

        return upgradablePlayerStats;
    }

    public virtual void ResetAllChipStats()
    {
        foreach (var stat in GetAllUpgradableStats())
        {
            stat.SetChipIncreaseAmount();
        }
    }

    public virtual void RefreshStats()
    {
        foreach (var stat in GetAllUpgradableStats())
        {
            stat.ResetStat();
        }
    }

    public virtual object Clone()
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
