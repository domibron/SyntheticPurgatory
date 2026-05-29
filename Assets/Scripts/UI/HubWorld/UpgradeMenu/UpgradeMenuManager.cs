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
    KnockBackForce,

    // Misc
    MaxScrapCarry,
    ItemCollectionRange,
    TimeLimit,
    CriticalChance,
}

public class UpgradeMenuManager : MonoBehaviour
{
    public static UpgradeMenuManager Instance { get; private set; }

    [SerializeField]
    MenuManager menuManager;

    [SerializeField]
    string mainMenuKey = "main";

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

        // precaution.
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

    private void OnConfirmation(bool confirmedAction)
    {
        if (!confirmedAction) return; // we dont care if its not confirming.

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

    public (UpgradableStat, UpgradableStat) GetCurrentStat(StatType statType)
    {
        return (ConvertEnumToStat(statType, ref currentPStats, ref currentMiscStats), ConvertEnumToStat(statType, ref upgradedButNotAppliedPStats, ref upgradedButNotAppliedMiscStats));

    }

    public void AddUpgradeOnce(StatType statType)
    {

        currentCost += UpgradeOnce(ref ConvertEnumToStat(statType, ref upgradedButNotAppliedPStats, ref upgradedButNotAppliedMiscStats));


        UpdateStatUI();
    }

    public void RemoveUpgradeOnce(StatType statType)
    {
        currentCost += RemoveUpgradeOnce(ref ConvertEnumToStat(statType, ref currentPStats, ref currentMiscStats), ref ConvertEnumToStat(statType, ref upgradedButNotAppliedPStats, ref upgradedButNotAppliedMiscStats));


        UpdateStatUI();
    }

    private int RemoveUpgradeOnce(ref UpgradableStat currentStat, ref UpgradableStat upgradingStat)
    {
        int diff = upgradingStat.UpgradedAmount - currentStat.UpgradedAmount;
        if (diff - 1 < 0) return 0;

        int costRemoval = -currentStat.UpgradeCost(diff);
        ReduceStatByOne(ref upgradingStat);
        if (diff - 1 > 0)
            costRemoval += currentStat.UpgradeCost(diff - 1);

        return costRemoval;
    }

    private int UpgradeOnce(ref UpgradableStat upgradingStat)
    {
        if (upgradingStat.GetMaxUpgradeCountPossible() == 0) return 0;

        int cost = upgradingStat.UpgradeCost();
        upgradingStat.UpgradeStat();

        return cost;
    }

    // we can just modify the memory directly, no need to copy into a temp variable.
    private void ReduceStatByOne(ref UpgradableStat stat)
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

    public int GetRemainingFromCost(ref UpgradableStat stat, int amount = 1)
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
        AddChipModifiersIn(); // stinky
        OnStatsUpdated?.Invoke();
    }

    private void AddChipModifiersIn()
    {
        ChipRunM.Instance.ModifyStatChipData(ref currentPStats, ref currentMiscStats);
        ChipRunM.Instance.ModifyStatChipData(ref upgradedButNotAppliedPStats, ref upgradedButNotAppliedMiscStats);
    }

    public void GetConfirmApplyStats()
    {
        waitingForConfirmationFor = WaitingForConfirmationFor.ApplyStats;
        if (!ConfirmationBox.Instance.TryOpenConfirmationBox("Apply Stats", "Are you sure you want to apply these stat changes?"))
        {
            waitingForConfirmationFor = WaitingForConfirmationFor.None;
        }
    }

    public void GetConfirmRevertStats()
    {
        waitingForConfirmationFor = WaitingForConfirmationFor.RevertStats;
        if (!ConfirmationBox.Instance.TryOpenConfirmationBox("Revert Changes", "Are you sure you want to revert all your stat changes?"))
        {
            waitingForConfirmationFor = WaitingForConfirmationFor.None;
        }
    }

    public void GetConfirmBackToMenu()
    {
        if (currentCost > 0)
        {
            waitingForConfirmationFor = WaitingForConfirmationFor.BackToMenuWithUnsavedStats;
            if (!ConfirmationBox.Instance.TryOpenConfirmationBox("Lose Changes", "You still have unapplied changes!\nAre you sure you want to lose all changes?"))
            {
                waitingForConfirmationFor = WaitingForConfirmationFor.None;
            }
        }
        else
        {
            BackToMenu();
        }
    }

    private void BackToMenu()
    {
        RevertStats();
        menuManager.OpenMenu(mainMenuKey);
    }

    public static ref UpgradableStat ConvertEnumToStat(StatType statType, ref PlayerStats pStats, ref MiscellaneousStats mStats)
    {
        switch (statType)
        {
            default:
                throw new NullReferenceException("No valid stat for enum!");
            case StatType.MaxHealth:
                return ref pStats.MaxHealthStat;
            case StatType.RegenerationAmount:
                return ref pStats.RegenerationAmountStat;
            case StatType.GroundSpeed:
                return ref pStats.GroundRunSpeedStat;
            case StatType.SlideBoostForce:
                return ref pStats.SlideBoostPercentageStat;
            case StatType.AirBoostForce:
                return ref pStats.AirBoostPercentageStat;
            case StatType.ProjectileDamage:
                return ref pStats.ProjectileDamageStat;
            case StatType.CannonRechargeRate:
                return ref pStats.RechargeSecondsStat;
            case StatType.ShotsPerFullCharge:
                return ref pStats.ShotsPerFullChargeStat;
            case StatType.OverheatForceCoolDown:
                return ref pStats.OverheatForceCoolDownStat;
            case StatType.MeleeDamage:
                return ref pStats.MeleeDamageStat;
            case StatType.MeleeDelay:
                return ref pStats.MeleeAttackDelayStat;
            case StatType.BashDelay:
                return ref pStats.BashAttackDelayStat;
            case StatType.EnemyStaggerTime:
                return ref pStats.MeleeStaggerTimeStat;
            case StatType.Reach:
                return ref pStats.MeleeReachStat;
            case StatType.KnockBackForce:
                return ref pStats.BashForceStat;
            case StatType.MaxScrapCarry:
                return ref mStats.MaxInventoryScrapStat;
            case StatType.ItemCollectionRange:
                return ref mStats.MaxCollectionRangeStat;
            case StatType.TimeLimit:
                return ref mStats.MaxLevelTimeStat;
            case StatType.CriticalChance:
                return ref mStats.CriticalHitChanceStat;

        }
    }

}
