using System;
using UnityEngine;
using UnityEngine.Serialization;

// TODO: Use a base class ideally but eh, screw it.
// ? Do you mean for like health, move speed etc? because it does use a base class.

/// <summary>
/// Player stats class storing the players upgradable stats and constant stats.
/// </summary>
[Serializable]
public class PlayerStats : CoreStats, ICloneable
{

    // ===========
    //   Health.
    // =========== 
    public UpgradableStat MaxHealthStat = new UpgradableStat(100f);

    public float RegenerationSpeed = 1f;
    public UpgradableStat RegenerationAmountStat = new(5f);


    // =============
    //   Movement.
    // =============
    public UpgradableStat RunSpeed = new(8f);
    public UpgradableStat WalkSpeed = new(5f);

    public float WalkAccelRate = 0.2f;
    public float RunAccelRate = 0.2f;
    public float AirAccelRate = 0.5f;

    public UpgradableStat JumpHeight = new(2f);

    public UpgradableStat SlideForce = new(5f);
    public float SlideFriction = 0.5f;


    // ======================
    //   Projectile Weapon.
    // ======================
    public UpgradableStat ProjectileDamageStat = new(12f);
    public UpgradableStat RechargeSecondsStat = new(0.3f);
    public UpgradableStat ShotsPerFullChargeStat = new(10);
    public float StandardSecondsPerShot = 0.4f;
    public float ChargedSecondsPerShot = 0.1f;
    public float DelayAfterFireBeforeRecharging = 0.4f;
    public UpgradableStat OverheatForceCoolDownStat = new(3f);


    // =================
    //   Melee Weapon.
    // =================
    public UpgradableStat MeleeAttackDelayStat = new(0.5f);
    public UpgradableStat MeleeDamageStat = new(10f);
    public UpgradableStat MeleeReachStat = new(1.5f);
    public UpgradableStat MeleeStaggerTimeStat = new(0.4f);

    // =========
    //   Bash.
    // =========
    public UpgradableStat BashForceStat = new(10f);
    public UpgradableStat BashAttackDelayStat = new(0.5f);



    public UpgradableStat CriticalHitChance = new(0.01f);

}

/// <summary>
/// The player scriptable object storing the player stats.
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjects/Stats/PlayerStats", fileName = "SO_PlayerStats")]
public class PlayerStatsSO : StatsCoreSO
{
    // Please make sure the variables that you want to access are not able to be modified directly.
    // Example below shows you one way to achieve this.

    [SerializeField, FormerlySerializedAs("stats")]
    private PlayerStats pStats;

    public override object GetStats()
    {
        return pStats.Clone();
    }

    void OnValidate() // The work around to reset and set data correctly at unity validate.
    {
        pStats.RefreshStats();
        pStats.ResetAllChipStats(); // make sure there are no lingering data.
    }
}
