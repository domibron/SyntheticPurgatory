using UnityEngine;

// TODO: Use a base class ideally but eh, screw it.

[CreateAssetMenu(menuName = "ScriptableObjects/Stats/MeleeEnemyStats", fileName = "SO_MeleeEnemyStats")]
public class MeleeEnemyStatsSO : StatsCoreSO
{
    // Please make sure the variables that you want to access are not able to be modified.
    // Example below shows you one way to achive this.

    [SerializeField]
    private float maxHealth = 10f;

    public float MaxHealth { get => maxHealth; }
}
