using System;
using UnityEngine;

public enum StatType
{
    // Stats
    MaxHealth,
    RegenerationAmount,
    GroundSpeed,
    SlideBoostForce,
    AirBoostForce,

    // Cannon
    ProjectileDamage,
    CannonRechargeRate,
    ShotsPerFullCharge,
    OverheatForceCoolDown,

    // Melee
    MeleeDamage,
    MeleeDelay,
    BashDelay,
    EnemyStaggerTime,
    Reach,
    KnockbackForce,

    // Misc
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

    private int currentCost = 0;

    private GameManager gameManager;

    // public event Action OnStatsAppliedOrReset;
    public event Action OnStatsUpdated;

    enum WaitingForConfirmationFor
    {
        ApplyStats,
        RevertStats,
        BackToMenuWithUnsavedStats,
        None,
    }

    WaitingForConfirmationFor waitingForConfirmationFor = WaitingForConfirmationFor.None;

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

        gameManager = GameManager.Instance;
        ConfirmationBox.Instance.OnConfirmation += OnConfirmation;
    }

    void OnDisable()
    {
        ConfirmationBox.Instance.OnConfirmation -= OnConfirmation;
    }

    private void OnConfirmation(bool cofirmedAction)
    {
        if (!cofirmedAction) return; // we dont care if its not confirming.

        switch (waitingForConfirmationFor)
        {
            case WaitingForConfirmationFor.None:
                return; // We are not waiting for any confirmations so we ignore the message.
            case WaitingForConfirmationFor.ApplyStats:
                ApplyStats();
                break;
            case WaitingForConfirmationFor.RevertStats:
                RevertStats();
                break;
            case WaitingForConfirmationFor.BackToMenuWithUnsavedStats:
                BackToMenu();
                break;
        }
    }

    public (UpgradablePlayerStat, UpgradablePlayerStat) GetCurrentStat(StatType statType)
    {
        switch (statType)
        {
            // STATS
            case StatType.MaxHealth:
                return (currentPStats.MaxHealthStat, upgradedButNotAppliedPStats.MaxHealthStat);
            case StatType.RegenerationAmount:
                return (currentPStats.RegenerationAmountStat, upgradedButNotAppliedPStats.RegenerationAmountStat);
            case StatType.GroundSpeed:
                return (currentPStats.GroundSpeedStat, upgradedButNotAppliedPStats.GroundSpeedStat);
            case StatType.SlideBoostForce:
                return (currentPStats.SlideBoostPercentageStat, upgradedButNotAppliedPStats.SlideBoostPercentageStat);
            case StatType.AirBoostForce:
                return (currentPStats.AirBoostPercentageStat, upgradedButNotAppliedPStats.AirBoostPercentageStat);

            // CANNON
            case StatType.ProjectileDamage:
                return (currentPStats.ProjectileDamageStat, upgradedButNotAppliedPStats.ProjectileDamageStat);
            case StatType.CannonRechargeRate:
                return (currentPStats.RechargeSecondsStat, upgradedButNotAppliedPStats.RechargeSecondsStat);
            case StatType.ShotsPerFullCharge:
                return (currentPStats.ShotsPerFullChargeStat, upgradedButNotAppliedPStats.ShotsPerFullChargeStat);
            case StatType.OverheatForceCoolDown:
                return (currentPStats.OverheatForceCooldownStat, upgradedButNotAppliedPStats.OverheatForceCooldownStat);

            // MELEE
            case StatType.MeleeDamage:
                return (currentPStats.MeleeDamageStat, upgradedButNotAppliedPStats.MeleeDamageStat);
            case StatType.MeleeDelay:
                return (currentPStats.MeleeAttackDelayStat, upgradedButNotAppliedPStats.MeleeAttackDelayStat);
            case StatType.BashDelay:
                return (currentPStats.BashAttackDelayStat, upgradedButNotAppliedPStats.BashAttackDelayStat);
            case StatType.EnemyStaggerTime:
                return (currentPStats.MeleeStaggerTimeStat, upgradedButNotAppliedPStats.MeleeStaggerTimeStat);
            case StatType.Reach:
                return (currentPStats.MeleeReachStat, upgradedButNotAppliedPStats.MeleeReachStat);
            case StatType.KnockbackForce:
                return (currentPStats.BashForceStat, upgradedButNotAppliedPStats.BashForceStat);

            // MISCELLANEOUS
            case StatType.MaxScrapCarry:
                return (currentMiscStats.MaxInventoryScrapStat, upgradedButNotAppliedMiscStats.MaxInventoryScrapStat);
            case StatType.ItemCollectionRange:
                return (currentMiscStats.MaxCollectionRangeStat, upgradedButNotAppliedMiscStats.MaxCollectionRangeStat);
            case StatType.TimeLimit:
                return (currentMiscStats.MaxLevelTimeStat, upgradedButNotAppliedMiscStats.MaxLevelTimeStat);
            case StatType.CriticalChance:
                return (currentMiscStats.CriticalHitChanceStat, upgradedButNotAppliedMiscStats.CriticalHitChanceStat);
        }

        return (null, null);
    }

    public void AddUpgradeOnce(StatType statType)
    {
        switch (statType)
        {
            // STATS
            case StatType.MaxHealth:
                currentCost += UpgradeOnce(ref upgradedButNotAppliedPStats.MaxHealthStat);
                break;
            case StatType.RegenerationAmount:
                currentCost += UpgradeOnce(ref upgradedButNotAppliedPStats.RegenerationAmountStat);
                break;
            case StatType.GroundSpeed:
                currentCost += UpgradeOnce(ref upgradedButNotAppliedPStats.GroundSpeedStat);
                break;
            case StatType.SlideBoostForce:
                currentCost += UpgradeOnce(ref upgradedButNotAppliedPStats.SlideBoostPercentageStat);
                break;
            case StatType.AirBoostForce:
                currentCost += UpgradeOnce(ref upgradedButNotAppliedPStats.AirBoostPercentageStat);
                break;

            // CANNON
            case StatType.ProjectileDamage:
                currentCost += UpgradeOnce(ref upgradedButNotAppliedPStats.ProjectileDamageStat);
                break;
            case StatType.CannonRechargeRate:
                currentCost += UpgradeOnce(ref upgradedButNotAppliedPStats.RechargeSecondsStat);
                break;
            case StatType.ShotsPerFullCharge:
                currentCost += UpgradeOnce(ref upgradedButNotAppliedPStats.ShotsPerFullChargeStat);
                break;
            case StatType.OverheatForceCoolDown:
                currentCost += UpgradeOnce(ref upgradedButNotAppliedPStats.OverheatForceCooldownStat);
                break;

            // MELEE
            case StatType.MeleeDamage:
                currentCost += UpgradeOnce(ref upgradedButNotAppliedPStats.MeleeDamageStat);
                break;
            case StatType.MeleeDelay:
                currentCost += UpgradeOnce(ref upgradedButNotAppliedPStats.MeleeAttackDelayStat);
                break;
            case StatType.BashDelay:
                currentCost += UpgradeOnce(ref upgradedButNotAppliedPStats.BashAttackDelayStat);
                break;
            case StatType.EnemyStaggerTime:
                currentCost += UpgradeOnce(ref upgradedButNotAppliedPStats.MeleeStaggerTimeStat);
                break;
            case StatType.Reach:
                currentCost += UpgradeOnce(ref upgradedButNotAppliedPStats.MeleeReachStat);
                break;
            case StatType.KnockbackForce:
                currentCost += UpgradeOnce(ref upgradedButNotAppliedPStats.BashForceStat);
                break;

            // MISCELLANEOUS
            case StatType.MaxScrapCarry:
                currentCost += UpgradeOnce(ref upgradedButNotAppliedMiscStats.MaxInventoryScrapStat);
                break;
            case StatType.ItemCollectionRange:
                currentCost += UpgradeOnce(ref upgradedButNotAppliedMiscStats.MaxCollectionRangeStat);
                break;
            case StatType.TimeLimit:
                currentCost += UpgradeOnce(ref upgradedButNotAppliedMiscStats.MaxLevelTimeStat);
                break;
            case StatType.CriticalChance:
                currentCost += UpgradeOnce(ref upgradedButNotAppliedMiscStats.CriticalHitChanceStat);
                break;
        }

        UpdateStatUI();
    }

    public void RemoveUpgradeOnce(StatType statType)
    {
        switch (statType)
        {
            // STATS
            case StatType.MaxHealth:
                currentCost += RemoveUpgradeOnce(ref currentPStats.MaxHealthStat, ref upgradedButNotAppliedPStats.MaxHealthStat);
                break;
            case StatType.RegenerationAmount:
                currentCost += RemoveUpgradeOnce(ref currentPStats.RegenerationAmountStat, ref upgradedButNotAppliedPStats.RegenerationAmountStat);
                break;
            case StatType.GroundSpeed:
                currentCost += RemoveUpgradeOnce(ref currentPStats.GroundSpeedStat, ref upgradedButNotAppliedPStats.GroundSpeedStat);
                break;
            case StatType.SlideBoostForce:
                currentCost += RemoveUpgradeOnce(ref currentPStats.SlideBoostPercentageStat, ref upgradedButNotAppliedPStats.SlideBoostPercentageStat);
                break;
            case StatType.AirBoostForce:
                currentCost += RemoveUpgradeOnce(ref currentPStats.AirBoostPercentageStat, ref upgradedButNotAppliedPStats.AirBoostPercentageStat);
                break;

            // CANNON
            case StatType.ProjectileDamage:
                currentCost += RemoveUpgradeOnce(ref currentPStats.ProjectileDamageStat, ref upgradedButNotAppliedPStats.ProjectileDamageStat);
                break;
            case StatType.CannonRechargeRate:
                currentCost += RemoveUpgradeOnce(ref currentPStats.RechargeSecondsStat, ref upgradedButNotAppliedPStats.RechargeSecondsStat);
                break;
            case StatType.ShotsPerFullCharge:
                currentCost += RemoveUpgradeOnce(ref currentPStats.ShotsPerFullChargeStat, ref upgradedButNotAppliedPStats.ShotsPerFullChargeStat);
                break;
            case StatType.OverheatForceCoolDown:
                currentCost += RemoveUpgradeOnce(ref currentPStats.OverheatForceCooldownStat, ref upgradedButNotAppliedPStats.OverheatForceCooldownStat);
                break;

            // MELEE
            case StatType.MeleeDamage:
                currentCost += RemoveUpgradeOnce(ref currentPStats.MeleeDamageStat, ref upgradedButNotAppliedPStats.MeleeDamageStat);
                break;
            case StatType.MeleeDelay:
                currentCost += RemoveUpgradeOnce(ref currentPStats.MeleeAttackDelayStat, ref upgradedButNotAppliedPStats.MeleeAttackDelayStat);
                break;
            case StatType.BashDelay:
                currentCost += RemoveUpgradeOnce(ref currentPStats.BashAttackDelayStat, ref upgradedButNotAppliedPStats.BashAttackDelayStat);
                break;
            case StatType.EnemyStaggerTime:
                currentCost += RemoveUpgradeOnce(ref currentPStats.MeleeStaggerTimeStat, ref upgradedButNotAppliedPStats.MeleeStaggerTimeStat);
                break;
            case StatType.Reach:
                currentCost += RemoveUpgradeOnce(ref currentPStats.MeleeReachStat, ref upgradedButNotAppliedPStats.MeleeReachStat);
                break;
            case StatType.KnockbackForce:
                currentCost += RemoveUpgradeOnce(ref currentPStats.BashForceStat, ref upgradedButNotAppliedPStats.BashForceStat);
                break;

            // MISCELLANEOUS
            case StatType.MaxScrapCarry:
                currentCost += RemoveUpgradeOnce(ref currentMiscStats.MaxInventoryScrapStat, ref upgradedButNotAppliedMiscStats.MaxInventoryScrapStat);
                break;
            case StatType.ItemCollectionRange:
                currentCost += RemoveUpgradeOnce(ref currentMiscStats.MaxCollectionRangeStat, ref upgradedButNotAppliedMiscStats.MaxCollectionRangeStat);
                break;
            case StatType.TimeLimit:
                currentCost += RemoveUpgradeOnce(ref currentMiscStats.MaxLevelTimeStat, ref upgradedButNotAppliedMiscStats.MaxLevelTimeStat);
                break;
            case StatType.CriticalChance:
                currentCost += RemoveUpgradeOnce(ref currentMiscStats.CriticalHitChanceStat, ref upgradedButNotAppliedMiscStats.CriticalHitChanceStat);
                break;
        }

        UpdateStatUI();
    }

    private int RemoveUpgradeOnce(ref UpgradablePlayerStat currentStat, ref UpgradablePlayerStat upgradingStat)
    {
        int diff = upgradingStat.UpgradedAmount - currentStat.UpgradedAmount;
        if (diff - 1 < 0) return 0;

        int costRemoval = -currentStat.UpgradeCost(diff);
        ReduceStatByOne(ref upgradingStat);
        if (diff - 1 > 0)
            costRemoval += currentStat.UpgradeCost(diff - 1);

        return costRemoval;
    }

    private int UpgradeOnce(ref UpgradablePlayerStat upgradingStat)
    {
        if (upgradingStat.GetHowManyTimesToUpgradeBeforeMaxing() == 0) return 0;

        int cost = upgradingStat.UpgradeCost();
        upgradingStat.UpgradeStat();

        return cost;
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

    private void CopyOverStats(bool copyToCurrent = true)
    {
        if (copyToCurrent)
        {
            currentPStats = (PlayerStats)upgradedButNotAppliedPStats.Clone();
            currentMiscStats = (MiscellaneousStats)upgradedButNotAppliedMiscStats.Clone();

            GameStatsManager.Instance.UpdateStats<PlayerStats>(Stats.player, currentPStats);
            GameStatsManager.Instance.UpdateStats<MiscellaneousStats>(Stats.miscellaneous, currentMiscStats);
        }
        else
        {
            upgradedButNotAppliedPStats = (PlayerStats)currentPStats.Clone();
            upgradedButNotAppliedMiscStats = (MiscellaneousStats)currentMiscStats.Clone();
        }

        UpdateStatUI(); // even though this is called, you should still call this for any operations after such as resetting current cost.
    }

    public int GetRemainingScrap()
    {
        if (gameManager == null) return 0;

        return gameManager.GetCurrentScrapCount() - currentCost;
    }

    public int GetRemainingFromCost(ref UpgradablePlayerStat stat, int amount = 1)
    {
        return GetRemainingScrap() + stat.UpgradeCost(amount);
    }

    public void RevertStats()
    {
        currentCost = 0;
        CopyOverStats(false);

        UpdateStatUI();
    }

    public void ApplyStats()
    {
        gameManager.RemoveFromDepositedScrap(currentCost);
        CopyOverStats();
        currentCost = 0;

        UpdateStatUI();
    }

    public int GetCurrentCost()
    {
        return currentCost;
    }

    public void UpdateStatUI()
    {
        OnStatsUpdated?.Invoke();
    }

    public void GetConfirmApplyStats()
    {
        waitingForConfirmationFor = WaitingForConfirmationFor.ApplyStats;
        ConfirmationBox.Instance.TryOpenConfirmationBox("Apply Stats", "Are you sure you want to apply these stat changes?");
    }

    public void GetConfirmRevertStats()
    {
        waitingForConfirmationFor = WaitingForConfirmationFor.RevertStats;
        ConfirmationBox.Instance.TryOpenConfirmationBox("Revert Changes", "Are you sure you want to revert all your stat changes?");
    }

    public void GetConfirmBackToMenu()
    {
        if (currentCost > 0)
        {
            waitingForConfirmationFor = WaitingForConfirmationFor.BackToMenuWithUnsavedStats;
            ConfirmationBox.Instance.TryOpenConfirmationBox("Lose Changes", "You still have unapplied changes!\nAre you sure you want to lose all changes?");
        }
        else
        {
            BackToMenu();
        }
    }

    private void BackToMenu()
    {
        RevertStats();
        // TODO LOAD THE HUB MAIN MENU
    }
}
