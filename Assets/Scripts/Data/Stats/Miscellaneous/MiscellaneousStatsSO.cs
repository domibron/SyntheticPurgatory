using System;
using UnityEngine;

[Serializable]
public class MiscellaneousStats : CoreStats, ICloneable
{
    public UpgradableStat MaxCollectionRangeStat = new(1.5f);
    public float CollectItemIntoInventoryRange = 1f;

    public float FlyAccel = 15f;
    public float FlyMaxSpeed = 30f;
    public float FlyDistanceBoost = 10f;

    public float DepositRate = 0.5f;

    public UpgradableStat MaxInventoryScrapStat = new(100);

    public int ScrapRangeUpgradeAmount = 0;

    public UpgradableStat MaxLevelTimeStat = new(120f);

}

[CreateAssetMenu(menuName = "ScriptableObjects/Stats/Miscellaneous", fileName = "SO_MiscellaneousStats")]
public class MiscellaneousStatsSO : StatsCoreSO
{
    // Please make sure the variables that you want to access are not able to be modified.
    // Example below shows you one way to achieve this.

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
