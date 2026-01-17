using System;
using UnityEngine;

[Serializable]
public class MiscellaneousStats : ICloneable
{
    public UpgradablePlayerStat MaxCollectionRangeStat = new(1.5f);
    public float CollectItemIntoInventoryRange = 1f;

    public float FlyAccel = 15f;
    public float FlyMaxSpeed = 30f;
    public float FlyDistanceBoost = 10f;

    public float DepositRate = 0.5f;

    public UpgradablePlayerStat MaxInventoryScrapStat = new(100);

    public int ScrapRangeUpgradeAmount = 0;

    public UpgradablePlayerStat MaxLevelTimeStat = new(120f);

    public UpgradablePlayerStat CriticalHitChanceStat = new(0.1f);

    public void RefreshStats()
    {
        UpgradablePlayerStat[] upgradablePlayerStats =
        {
            MaxCollectionRangeStat,
            MaxInventoryScrapStat,
            MaxLevelTimeStat,
            CriticalHitChanceStat,
        };

        foreach (var stat in upgradablePlayerStats)
        {
            stat.ResetStat();
        }
    }

    public object Clone()
    {
        return new MiscellaneousStats
        {
            MaxCollectionRangeStat = (UpgradablePlayerStat)MaxCollectionRangeStat.Clone(),
            CollectItemIntoInventoryRange = CollectItemIntoInventoryRange,

            FlyAccel = FlyAccel,
            FlyMaxSpeed = FlyMaxSpeed,
            FlyDistanceBoost = FlyDistanceBoost,

            DepositRate = DepositRate,

            MaxInventoryScrapStat = (UpgradablePlayerStat)MaxInventoryScrapStat.Clone(),

            ScrapRangeUpgradeAmount = ScrapRangeUpgradeAmount,

            MaxLevelTimeStat = (UpgradablePlayerStat)MaxLevelTimeStat.Clone(),

            CriticalHitChanceStat = (UpgradablePlayerStat)CriticalHitChanceStat.Clone(),
        };
    }

}

[CreateAssetMenu(menuName = "ScriptableObjects/Stats/Miscellaneous", fileName = "SO_MiscellaneousStats")]
public class MiscellaneousStatsSO : StatsCoreSO
{
    // Please make sure the variables that you want to access are not able to be modified.
    // Example below shows you one way to achive this.

    [SerializeField]
    private MiscellaneousStats stats;

    public override object GetStats()
    {
        return stats.Clone();
    }

    void OnValidate()
    {
        stats.RefreshStats();
    }
}
