using System;
using UnityEngine;

[Serializable]
public class MiscellaneousStats : ICloneable
{
    public float MaxCollectionRange = 1.5f;
    public float CollectItemRange = 1f;

    public float FlyAccel = 15f;
    public float FlyMaxSpeed = 30f;
    public float FlyDistanceBoost = 10f;

    public float DepositRate = 0.5f;

    public int MaxInventoryScrap = 100;

    public int ScrapRangeUpgradeAmount = 0;

    public float MaxLevelTime = 120f;

    public float CriticalHitChance = 0.1f;

    public object Clone()
    {
        return new MiscellaneousStats
        {
            MaxCollectionRange = MaxCollectionRange,
            CollectItemRange = CollectItemRange,
            FlyAccel = FlyAccel,
            FlyMaxSpeed = FlyMaxSpeed,
            FlyDistanceBoost = FlyDistanceBoost,
            DepositRate = DepositRate,
            MaxInventoryScrap = MaxInventoryScrap,

            ScrapRangeUpgradeAmount = ScrapRangeUpgradeAmount,
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
}
