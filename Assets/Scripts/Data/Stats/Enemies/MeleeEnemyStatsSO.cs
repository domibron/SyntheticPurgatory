using System;
using UnityEngine;

// TODO: Use a base class ideally but eh, screw it.

[Serializable]
public class MeleeEnemyStats : CoreStats, ICloneable
{
    public float health = 0f;

    public override object Clone()
    {
        return new MeleeEnemyStats
        {
            health = health,

        };
    }

}


[CreateAssetMenu(menuName = "ScriptableObjects/Stats/MeleeEnemyStats", fileName = "SO_MeleeEnemyStats")]
public class MeleeEnemyStatsSO : StatsCoreSO
{
    // Please make sure the variables that you want to access are not able to be modified.
    // Example below shows you one way to achieve this.

    [SerializeField]
    private MeleeEnemyStats stats;

    public override object GetStats()
    {
        return stats.Clone();
    }
}