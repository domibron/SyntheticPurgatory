using System;
using UnityEngine;

// TODO: Use a base class ideally but eh, screw it.

[Serializable]
public class PlayerStats : ICloneable
{
    public float MaxHealth = 100f;

    public float GroundSpeed = 6f;
    public float AirSpeed = 3f;
    public float JumpSpeed = 9.2f;
    public float SlideBoostForce = 5f;
    public float AirBoostForce = 5f;
    public float GroundFriction = 5f;
    public float AirFriction = 1;

    public float ProjectileDamage = 12f;
    public float ProjectileFireRate = 0.3f;
    public int ProjectileMagSize = 20;
    public float ReloadTime = 2f;
    public float MeleeAttackDelay = 0.5f;
    public float MeleeDamage = 10f;
    public float KickForce = 10f;
    public float KickAttackDelay = 0.5f;

    public float MeleeReach = 1.5f;
    public float MeleeStaggeTime = 0.4f;
    public float RegenerationSpeed = 1f;
    public float RegenerationAmount = 1f;

    public int SpeedUpgradeAmount = 1;
    public int BoostUpgradeAmount = 1;
    public int MeleeStaggerUpgradeAmount = 1;
    public int MeleeReachUpgradeAmount = 1;

    public object Clone()
    {
        return new PlayerStats
        {
            MaxHealth = MaxHealth,

            GroundSpeed = GroundSpeed,
            AirSpeed = AirSpeed,
            JumpSpeed = JumpSpeed,
            SlideBoostForce = SlideBoostForce,
            AirBoostForce = AirBoostForce,

            ProjectileDamage = ProjectileDamage,
            ProjectileFireRate = ProjectileFireRate,
            ProjectileMagSize = ProjectileMagSize,
            MeleeAttackDelay = MeleeAttackDelay,
            MeleeDamage = MeleeDamage,
            KickForce = KickForce,
            KickAttackDelay = KickAttackDelay,

            MeleeReach = MeleeReach,
            MeleeStaggeTime = MeleeStaggeTime,
            RegenerationSpeed = RegenerationSpeed,
            RegenerationAmount = RegenerationAmount,

            SpeedUpgradeAmount = SpeedUpgradeAmount,
            BoostUpgradeAmount = BoostUpgradeAmount,
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
