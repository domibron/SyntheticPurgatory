using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Chips/PercentIncreaseChip", fileName = "SO_PercentIncreaseChip")]
public class PercentIncreaseChipSO : ChipSO
{
    public float percentIncrease = 0.5f;

    public StatType targetStat = StatType.MaxHealth;

    public override void ModifyStats(ref PlayerStats pStats, ref MiscellaneousStats miscStats)
    {
        // foreach the enum and use the value to cycle through and increase.

    }

    protected virtual void IncreaseStat(ref UpgradablePlayerStat pStat)
    {
        if (pStat.IsIncreasingStat)
        {
            pStat.AddToChipIncreaseAmount(pStat.CurrentValue * percentIncrease);
        }
        else
        {
            pStat.AddToChipIncreaseAmount(pStat.GetCurrentValue() * -percentIncrease); // cant wait for bugs :3
        }
    }
}
