using System;
using UnityEngine;

[Serializable]
public class CollectableStats : ICloneable
{
    public float MaxCollectionRange = 1.5f;
    public float CollectItemRange = 1f;

    public float FlyAccel = 15f;
    public float FlyMaxSpeed = 30f;
    public float FlyDistanceBoost = 10f;

    public float DepositRate = 0.5f;

    public int MaxInventoryScrap = 100;

    public object Clone()
    {
        return new CollectableStats
        {
            MaxCollectionRange = MaxCollectionRange,
            CollectItemRange = CollectItemRange,
            FlyAccel = FlyAccel,
            FlyMaxSpeed = FlyMaxSpeed,
            FlyDistanceBoost = FlyDistanceBoost,
            DepositRate = DepositRate,
            MaxInventoryScrap = MaxInventoryScrap,
        };
    }

}

[CreateAssetMenu(menuName = "ScriptableObjects/Stats/Collectable", fileName = "SO_CollectableStats")]
public class CollectableStatsSO : StatsCoreSO
{
    // Please make sure the variables that you want to access are not able to be modified.
    // Example below shows you one way to achive this.

    [SerializeField]
    private CollectableStats stats;

    public override object GetStats()
    {
        return stats.Clone();
    }
}
