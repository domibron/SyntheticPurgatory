using System;
using UnityEngine;

public class MeleeAttackAnimEvents : MonoBehaviour
{
    [SerializeField]
    private SawnEnemyAI enemyAI;

    public void AttackEvent(int endOfAttacks)
    {
        enemyAI.AttemptAttack(Convert.ToBoolean(endOfAttacks));
    }

}
