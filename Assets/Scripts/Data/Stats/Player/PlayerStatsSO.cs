using System;
using UnityEngine;

// TODO: Use a base class ideally but eh, screw it.
// ? Do you mean for like health, move speed etc? because it does use a base class.

/// <summary>
/// Player stats class storing the players upgradable stats and constant stats.
/// </summary>
[Serializable]
public class PlayerStats : CoreStats, ICloneable
{
    public UpgradableStat MaxHealthStat = new UpgradableStat(100f);


    public float RegenerationSpeed = 1f;
    public UpgradableStat RegenerationAmountStat = new(5f);

    public float WalkSpeed = 4f;
    public UpgradableStat GroundRunSpeedStat = new(5.5f);
    public float AirSpeed = 3f; // TODO: should be a fraction from gound speed.

    // public float GroundAcceleration = 30f;
    // public float AirAcceleration = 20f;

    // this should be calculated from accel and speed to get this. Speed * thisPercentage = accel.
    public float GroundAccelerationPercentBase = 5.45454545455f;
    public float AirAccelerationPercentBase = 6.66666666667f;

    public float JumpForce = 9.2f;


    public UpgradableStat SlideBoostPercentageStat = new(1.65f);
    public UpgradableStat AirBoostPercentageStat = new(2.5f);

    public float GroundFriction = 5f;
    public float AirFriction = 1;


    public UpgradableStat ProjectileDamageStat = new(12f);
    public UpgradableStat RechargeSecondsStat = new(0.3f);
    public UpgradableStat ShotsPerFullChargeStat = new(10);
    public float StandardSecondsPerShot = 0.4f;
    public float ChargedSecondsPerShot = 0.1f;
    public float DelayAfterFireBeforeRecharging = 0.4f;
    public UpgradableStat OverheatForceCoolDownStat = new(3f);


    public UpgradableStat MeleeAttackDelayStat = new(0.5f);
    public UpgradableStat MeleeDamageStat = new(10f);
    public UpgradableStat MeleeReachStat = new(1.5f);
    public UpgradableStat MeleeStaggerTimeStat = new(0.4f);

    public UpgradableStat BashForceStat = new(10f);
    public UpgradableStat BashAttackDelayStat = new(0.5f);

    protected override UpgradableStat[] GetAllUpgradableStats()
    {
        UpgradableStat[] upgradablePlayerStats =
        {
            MaxHealthStat,
            RegenerationAmountStat,
            GroundRunSpeedStat,
            SlideBoostPercentageStat,
            AirBoostPercentageStat,
            ProjectileDamageStat,
            RechargeSecondsStat,
            ShotsPerFullChargeStat,
            OverheatForceCoolDownStat,
            MeleeAttackDelayStat,
            MeleeDamageStat,
            MeleeReachStat,
            MeleeStaggerTimeStat,
            BashForceStat,
            BashAttackDelayStat,
        };

        return upgradablePlayerStats;
    }

    public override object Clone()
    {
        return new PlayerStats
        {
            MaxHealthStat = (UpgradableStat)MaxHealthStat.Clone(),

            RegenerationSpeed = RegenerationSpeed,
            RegenerationAmountStat = (UpgradableStat)RegenerationAmountStat.Clone(),

            WalkSpeed = WalkSpeed,
            GroundRunSpeedStat = (UpgradableStat)GroundRunSpeedStat.Clone(),
            AirSpeed = AirSpeed,

            GroundAccelerationPercentBase = GroundAccelerationPercentBase,
            AirAccelerationPercentBase = AirAccelerationPercentBase,

            JumpForce = JumpForce,

            // SlideBoostForce = SlideBoostForce,
            // AirBoostForce = AirBoostForce,

            SlideBoostPercentageStat = (UpgradableStat)SlideBoostPercentageStat.Clone(),
            AirBoostPercentageStat = (UpgradableStat)AirBoostPercentageStat.Clone(),

            ProjectileDamageStat = (UpgradableStat)ProjectileDamageStat.Clone(),
            RechargeSecondsStat = (UpgradableStat)RechargeSecondsStat.Clone(),
            ShotsPerFullChargeStat = (UpgradableStat)ShotsPerFullChargeStat.Clone(),
            StandardSecondsPerShot = StandardSecondsPerShot,
            ChargedSecondsPerShot = ChargedSecondsPerShot,
            DelayAfterFireBeforeRecharging = DelayAfterFireBeforeRecharging,
            OverheatForceCoolDownStat = (UpgradableStat)OverheatForceCoolDownStat.Clone(),
            // ProjectileFireRate = ProjectileFireRate,


            MeleeAttackDelayStat = (UpgradableStat)MeleeAttackDelayStat.Clone(),
            MeleeDamageStat = (UpgradableStat)MeleeDamageStat.Clone(),
            BashForceStat = (UpgradableStat)BashForceStat.Clone(),
            BashAttackDelayStat = (UpgradableStat)BashAttackDelayStat.Clone(),

            MeleeReachStat = (UpgradableStat)MeleeReachStat.Clone(),
            MeleeStaggerTimeStat = (UpgradableStat)MeleeStaggerTimeStat.Clone(),

        };
    }

}

/// <summary>
/// The player scriptable object storing the player stats.
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjects/Stats/PlayerStats", fileName = "SO_PlayerStats")]
public class PlayerStatsSO : StatsCoreSO
{
    // Please make sure the variables that you want to access are not able to be modified directly.
    // Example below shows you one way to achieve this.

    [SerializeField]
    private PlayerStats stats;

    public override object GetStats()
    {
        return stats.Clone();
    }

    void OnValidate() // The work around to reset and set data correctly at unity validate.
    {
        stats.RefreshStats();
        stats.ResetAllChipStats(); // make sure there are no lingering data.
    }
}
