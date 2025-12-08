using System;
using UnityEngine;

// TODO: Use a base class ideally but eh, screw it.

[Serializable]
public class PlayerStats : ICloneable
{
    public float MaxHealth = 100f;


    public float RegenerationSpeed = 1f;
    public float RegenerationAmount = 5f;

    public float GroundSpeed = 5.5f;
    public float AirSpeed = 3f;

    // public float GroundAcceleration = 30f;
    // public float AirAcceleration = 20f;

    // this should be calculated from accel and speed to get this. 
    public float GroundAccelerationPercentBase = 5.45454545455f;
    public float AirAccelerationPercentBase = 6.66666666667f;

    public float JumpForce = 9.2f;

    [Obsolete("Use GroundSpeed * SlideBoostPercentage to get slide boost force.", true)]
    public float SlideBoostForce = 9f;

    [Obsolete("Use AirSpeed * AirBoostPercentage to get Air boost force.", true)]
    public float AirBoostForce = 7.5f;

    public float SlideBoostPercentage = 1.65f;
    public float AirBoostPercentage = 2.5f;

    public float GroundFriction = 5f;
    public float AirFriction = 1;



    public float ProjectileDamage = 12f;
    public float RechargeRate = 0.3f;
    public float ShotsPerFullCharge = 10;
    public float StandardSecondsPerShot = 0.4f;
    public float ChargedSecondsPerShot = 0.1f;
    public float DelayAfterFireBeforeRecharging = 0.4f;
    public float OverheatForceCooldown = 3f;

    // public float ProjectileFireRate = 0.3f;
    // public int ProjectileMagSize = 20;
    // public float ReloadTime = 2f;


    public float MeleeAttackDelay = 0.5f;
    public float MeleeDamage = 10f;
    public float MeleeReach = 1.5f;
    public float MeleeStagerTime = 0.4f;

    public float KickForce = 10f;
    public float KickAttackDelay = 0.5f;


    public int SpeedUpgradeAmount = 1;
    public int SlideBoostUpgradeAmount = 1;
    public int AirBoostUpgradeAmount = 1;
    public int MeleeStaggerUpgradeAmount = 1;
    public int MeleeReachUpgradeAmount = 1;

    public object Clone()
    {
        return new PlayerStats
        {
            MaxHealth = MaxHealth,

            RegenerationSpeed = RegenerationSpeed,
            RegenerationAmount = RegenerationAmount,

            GroundSpeed = GroundSpeed,
            AirSpeed = AirSpeed,

            GroundAccelerationPercentBase = GroundAccelerationPercentBase,
            AirAccelerationPercentBase = AirAccelerationPercentBase,

            JumpForce = JumpForce,

            // SlideBoostForce = SlideBoostForce,
            // AirBoostForce = AirBoostForce,

            SlideBoostPercentage = SlideBoostPercentage,
            AirBoostPercentage = AirBoostPercentage,

            ProjectileDamage = ProjectileDamage,
            RechargeRate = RechargeRate,
            ShotsPerFullCharge = ShotsPerFullCharge,
            StandardSecondsPerShot = StandardSecondsPerShot,
            ChargedSecondsPerShot = ChargedSecondsPerShot,
            DelayAfterFireBeforeRecharging = DelayAfterFireBeforeRecharging,
            OverheatForceCooldown = OverheatForceCooldown,
            // ProjectileFireRate = ProjectileFireRate,


            MeleeAttackDelay = MeleeAttackDelay,
            MeleeDamage = MeleeDamage,
            KickForce = KickForce,
            KickAttackDelay = KickAttackDelay,

            MeleeReach = MeleeReach,
            MeleeStagerTime = MeleeStagerTime,

            SpeedUpgradeAmount = SpeedUpgradeAmount,
            SlideBoostUpgradeAmount = SlideBoostUpgradeAmount,
            MeleeStaggerUpgradeAmount = MeleeStaggerUpgradeAmount,
            MeleeReachUpgradeAmount = MeleeReachUpgradeAmount,

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
}
