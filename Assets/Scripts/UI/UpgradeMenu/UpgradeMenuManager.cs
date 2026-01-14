using System;
using UnityEngine;

public enum StatType
{
    MaxHealth,
    RegenerationAmount,
    GroundSpeed,
    SlideBoostForce,
    AirBoostForce,

    ProjectileDamage,
    CannonRechargeRate,
    ShotsPerFullCharge,
    OverheatForceCoolDown,

    MeleeDamage,
    MeleeDelay,
    BashDelay,
    EnemyStaggerTime,
    Reach,
    KnockbackForce,

    MaxScrapCarry,
    ItemCollectionRange,
    TimeLimit,
    CriticalChance,
}

public class UpgradeMenuManager : MonoBehaviour
{
    public static UpgradeMenuManager Instance { get; private set; }

    PlayerStats currentPStats;
    PlayerStats upgradedButNotAppliedPStats;

    MiscellaneousStats currentMiscStats;
    MiscellaneousStats upgradedButNotAppliedMiscStats;

    // public event Action OnStatsAppliedOrReset;
    public event Action OnStatsUpdated;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        // precution.
        if (GameStatsManager.Instance == null) throw new NullReferenceException($"Cannot function correctly if {nameof(GameStatsManager)} doesn't exist!");

        // these two hold upgradeable stats. Could simplify the whole system with one dynamic stat class.
        currentPStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);
        currentMiscStats = GameStatsManager.Instance.GetStats<MiscellaneousStats>(Stats.miscellaneous);

        upgradedButNotAppliedPStats = (PlayerStats)currentPStats.Clone();
        upgradedButNotAppliedMiscStats = (MiscellaneousStats)currentMiscStats.Clone();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OnStatsUpdated?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public (UpgradablePlayerStat, UpgradablePlayerStat) GetCurrentStat(StatType statType)
    {
        switch (statType)
        {
            case StatType.MaxHealth:
                return (currentPStats.MaxHealthStat, upgradedButNotAppliedPStats.MaxHealthStat);
        }

        return (null, null);
    }

    public void AddUpgradeOnce(StatType statType)
    {
        switch (statType)
        {
            case StatType.MaxHealth:
                upgradedButNotAppliedPStats.MaxHealthStat.UpgradeStat();
                print("Stat upgraded");
                break;
        }

        OnStatsUpdated?.Invoke();
    }

    public void RemoveUpgradeOnce(StatType statType)
    {
        switch (statType)
        {
            case StatType.MaxHealth:
                ReduceStatByOne(ref upgradedButNotAppliedPStats.MaxHealthStat);
                break;
        }

        OnStatsUpdated?.Invoke();
    }

    // we can just modify the memory directly, no need to copy into a temp variable.
    private void ReduceStatByOne(ref UpgradablePlayerStat stat)
    {
        int currentAmount = stat.UpgradedAmount;

        if (currentAmount <= 0) return;

        currentAmount--;

        stat.ResetStat();

        stat.UpgradeStat(currentAmount);
    }
}
