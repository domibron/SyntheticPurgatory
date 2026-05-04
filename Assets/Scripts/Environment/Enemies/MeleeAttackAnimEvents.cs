using System;
using UnityEngine;

public class MeleeAttackAnimEvents : MonoBehaviour
{
    [SerializeField]
    private MeleeEnemyAI enemyAI;

    public void AttackEvent(int endOfAttacks)
    {
        enemyAI.AttemptAttack(Convert.ToBoolean(endOfAttacks));
    }

}
