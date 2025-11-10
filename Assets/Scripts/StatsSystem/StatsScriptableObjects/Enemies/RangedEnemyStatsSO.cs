using System;
using UnityEngine;

// TODO: Use a base class ideally but eh, screw it.

[Serializable]
public class RangedEnemyStats : ICloneable
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


    public object Clone()
    {
        return new RangedEnemyStats
        {
            health = health,
        };
    }

}

[CreateAssetMenu(menuName = "ScriptableObjects/Stats/RangedEnemyStats", fileName = "SO_RangedEnemyStats")]
public class RangedEnemyStatsSO : StatsCoreSO
{
    // Please make sure the variables that you want to access are not able to be modified.
    // Example below shows you one way to achive this.

    [SerializeField]
    private RangedEnemyStats stats;

    public override object GetStats()
    {
        return stats.Clone();
    }
}
