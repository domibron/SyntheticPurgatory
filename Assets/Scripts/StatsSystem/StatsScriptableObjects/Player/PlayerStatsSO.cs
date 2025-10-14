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
    public float DashSpeed = 6f;
    public float SlideBoostForce = 5f;
    public float AirBoostForce = 5f;

    public float ProjectileDamage = 12f;
    public float ProjectileFireRate = 0.3f;
    public int ProjectileMagSize = 20;
    public float MeleeAttackDelay = 0.5f;
    public float MeleeDamage = 10f;
    public float KickForce = 10f;
    public float KickAttackDelay = 0.5f;

    public object Clone()
    {
        return new PlayerStats
        {
            MaxHealth = MaxHealth,

            GroundSpeed = GroundSpeed,
            AirSpeed = AirSpeed,
            JumpSpeed = JumpSpeed,
            DashSpeed = DashSpeed,
            SlideBoostForce = SlideBoostForce,
            AirBoostForce = AirBoostForce,

            ProjectileDamage = ProjectileDamage,
            ProjectileFireRate = ProjectileFireRate,
            ProjectileMagSize = ProjectileMagSize,
            MeleeAttackDelay = MeleeAttackDelay,
            MeleeDamage = MeleeDamage,
            KickForce = KickForce,
            KickAttackDelay = KickAttackDelay,
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
