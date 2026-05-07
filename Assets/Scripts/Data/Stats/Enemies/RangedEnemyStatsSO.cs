using System;
using UnityEngine;

// TODO: Use a base class ideally but eh, screw it.

[Serializable]
public class RangedEnemyStats : CoreStats, ICloneable
{
    public float health = 0f;

    public float damage = 0f;
    public float attackRange = 0f;
    public float attackSpeed = 0f;

    public float baseSpeed = 0f;
    public float followSpeed = 0f;
    public float fleeSpeed = 0f;
    public float stuckSpeed = 0f;
    public float dodgeSpeed = 0f;


    public override object Clone()
    {
        return new RangedEnemyStats
        {
            health = health,

            damage = damage,
            attackRange = attackRange,
            attackSpeed = attackSpeed,

            baseSpeed = baseSpeed,
            followSpeed = followSpeed,
            fleeSpeed = fleeSpeed,
            stuckSpeed = stuckSpeed,
            dodgeSpeed = dodgeSpeed,
        };
    }

}

[CreateAssetMenu(menuName = "ScriptableObjects/Stats/RangedEnemyStats", fileName = "SO_RangedEnemyStats")]
public class RangedEnemyStatsSO : StatsCoreSO
{
    // Please make sure the variables that you want to access are not able to be modified.
    // Example below shows you one way to achieve this.

    [SerializeField]
    private RangedEnemyStats stats;

    public override object GetStats()
    {
        return stats.Clone();
    }
}
