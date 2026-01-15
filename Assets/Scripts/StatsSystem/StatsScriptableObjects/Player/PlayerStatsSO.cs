using System;
using UnityEngine;

// TODO: Use a base class ideally but eh, screw it.

[Serializable]
public class PlayerStats : ICloneable
{
    public UpgradablePlayerStat MaxHealthStat = new UpgradablePlayerStat(100f);


    public float RegenerationSpeed = 1f;
    public UpgradablePlayerStat RegenerationAmountStat = new(5f);

    public UpgradablePlayerStat GroundSpeedStat = new(5.5f);
    public float AirSpeed = 3f; // TODO: should be a fraction from gound speed.

    // public float GroundAcceleration = 30f;
    // public float AirAcceleration = 20f;

    // this should be calculated from accel and speed to get this. Speed * thisPercentage = accel.
    public float GroundAccelerationPercentBase = 5.45454545455f;
    public float AirAccelerationPercentBase = 6.66666666667f;

    public float JumpForce = 9.2f;


    public UpgradablePlayerStat SlideBoostPercentageStat = new(1.65f);
    public UpgradablePlayerStat AirBoostPercentageStat = new(2.5f);

    public float GroundFriction = 5f;
    public float AirFriction = 1;


    public UpgradablePlayerStat ProjectileDamageStat = new(12f);
    public UpgradablePlayerStat RechargeSecondsStat = new(0.3f);
    public UpgradablePlayerStat ShotsPerFullChargeStat = new(10);
    public float StandardSecondsPerShot = 0.4f;
    public float ChargedSecondsPerShot = 0.1f;
    public float DelayAfterFireBeforeRecharging = 0.4f;
    public UpgradablePlayerStat OverheatForceCooldownStat = new(3f);


    public UpgradablePlayerStat MeleeAttackDelayStat = new(0.5f);
    public UpgradablePlayerStat MeleeDamageStat = new(10f);
    public UpgradablePlayerStat MeleeReachStat = new(1.5f);
    public UpgradablePlayerStat MeleeStaggerTimeStat = new(0.4f);

    public UpgradablePlayerStat BashForceStat = new(10f);
    public UpgradablePlayerStat BashAttackDelayStat = new(0.5f);

    public void RefreshStats()
    {
        UpgradablePlayerStat[] upgradablePlayerStats =
        {
            MaxHealthStat,
            RegenerationAmountStat,
            GroundSpeedStat,
            SlideBoostPercentageStat,
            AirBoostPercentageStat,
            ProjectileDamageStat,
            RechargeSecondsStat,
            ShotsPerFullChargeStat,
            OverheatForceCooldownStat,
            MeleeAttackDelayStat,
            MeleeDamageStat,
            MeleeReachStat,
            MeleeStaggerTimeStat,
            BashForceStat,
            BashAttackDelayStat,
        };

        foreach (var stat in upgradablePlayerStats)
        {
            stat.ResetStat();
        }
    }

    public object Clone()
    {
        return new PlayerStats
        {
            MaxHealthStat = (UpgradablePlayerStat)MaxHealthStat.Clone(),

            RegenerationSpeed = RegenerationSpeed,
            RegenerationAmountStat = (UpgradablePlayerStat)RegenerationAmountStat.Clone(),

            GroundSpeedStat = (UpgradablePlayerStat)GroundSpeedStat.Clone(),
            AirSpeed = AirSpeed,

            GroundAccelerationPercentBase = GroundAccelerationPercentBase,
            AirAccelerationPercentBase = AirAccelerationPercentBase,

            JumpForce = JumpForce,

            // SlideBoostForce = SlideBoostForce,
            // AirBoostForce = AirBoostForce,

            SlideBoostPercentageStat = (UpgradablePlayerStat)SlideBoostPercentageStat.Clone(),
            AirBoostPercentageStat = (UpgradablePlayerStat)AirBoostPercentageStat.Clone(),

            ProjectileDamageStat = (UpgradablePlayerStat)ProjectileDamageStat.Clone(),
            RechargeSecondsStat = (UpgradablePlayerStat)RechargeSecondsStat.Clone(),
            ShotsPerFullChargeStat = (UpgradablePlayerStat)ShotsPerFullChargeStat.Clone(),
            StandardSecondsPerShot = StandardSecondsPerShot,
            ChargedSecondsPerShot = ChargedSecondsPerShot,
            DelayAfterFireBeforeRecharging = DelayAfterFireBeforeRecharging,
            OverheatForceCooldownStat = (UpgradablePlayerStat)OverheatForceCooldownStat.Clone(),
            // ProjectileFireRate = ProjectileFireRate,


            MeleeAttackDelayStat = (UpgradablePlayerStat)MeleeAttackDelayStat.Clone(),
            MeleeDamageStat = (UpgradablePlayerStat)MeleeDamageStat.Clone(),
            BashForceStat = (UpgradablePlayerStat)BashForceStat.Clone(),
            BashAttackDelayStat = (UpgradablePlayerStat)BashAttackDelayStat.Clone(),

            MeleeReachStat = (UpgradablePlayerStat)MeleeReachStat.Clone(),
            MeleeStaggerTimeStat = (UpgradablePlayerStat)MeleeStaggerTimeStat.Clone(),

        };
    }

}

[CreateAssetMenu(menuName = "ScriptableObjects/Stats/PlayerStats", fileName = "SO_PlayerStats")]
public class PlayerStatsSO : StatsCoreSO
{
    // Please make sure the variables that you want to access are not able to be modified.
    // Example below shows you one way to achive this.

    [SerializeField]
    private PlayerStats stats;

    public override object GetStats()
    {
        return stats.Clone();
    }

    void OnValidate()
    {
        stats.RefreshStats();
    }
}
