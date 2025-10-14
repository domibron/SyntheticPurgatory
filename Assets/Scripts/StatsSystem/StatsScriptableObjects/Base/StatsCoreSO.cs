using System;
using UnityEngine;

// [Serializable]
public class CoreStats : ICloneable
{
    public object Clone()
    {
        var clone = new CoreStats
        {

        };


        return clone;
    }
}

[CreateAssetMenu(menuName = "ScriptableObjects/Stats/BaseStats", fileName = "SO_BaseStats")]
public class StatsCoreSO : ScriptableObject
{
    // [SerializeField]
    CoreStats stats = new CoreStats();

    public virtual object GetStats()
    {
        return new CoreStats();
    }

    // keep empty for now.

    // Please make sure the variables that you want to access are not able to be modified.
    // Example below shows you one way to achive this.

    // [SerializeField]
    // private float maxHealth = 10f;

    // public float MaxHealth { get => maxHealth; }
}
