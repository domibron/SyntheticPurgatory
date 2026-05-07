using UnityEngine;

/// <summary>
/// Increases the stat by the set percentage. 0.5 is a x1.5 increase.
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjects/Chips/PercentIncreaseChip", fileName = "SO_PercentIncreaseChip")]
public class PercentIncreaseChipSO : ChipSO
{
    /// <summary>
    /// How much to increase is as a percentage. This will multiply the current value then add it to itself, so x = x + (x * percentage).
    /// </summary>
    public float percentIncrease = 0.5f;

    /// <summary>
    /// The target stat to modify.
    /// </summary>
    public StatType targetStat = StatType.MaxHealth;

    public override void ModifyStats(ref PlayerStats pStats, ref MiscellaneousStats mStats)
    {
        // foreach the enum and use the value to cycle through and increase. ~ Ur using ref so much, I just came back from c++ I want to make sure that I have a ref.
        IncreaseStat(ref UpgradeMenuManager.ConvertEnumToStat(targetStat, ref pStats, ref mStats));
    }

    /// <summary>
    /// Increases the stat if the stat is increase otherwise it will decrease the stat.
    /// </summary>
    /// <param name="pStat">A reference to he stat to modify.</param>
    protected virtual void IncreaseStat(ref UpgradableStat pStat)
    {
        if (pStat.IsIncreasingStat)
        {
            pStat.AddToChipIncreaseAmount(pStat.CurrentValue * percentIncrease);
        }
        else
        {
            pStat.AddToChipIncreaseAmount(pStat.BaseStat * -percentIncrease); // built in checks prevent going over the max. No, not to this function call.
        }
    }
}
