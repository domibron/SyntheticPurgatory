using UnityEngine;

/// <summary>
/// Base class for boss attacks to link.
/// </summary>
public abstract class BossArenaAttack : MonoBehaviour
{
    protected BossAI bossAI;

    /// <summary>
    /// Called when the boss is set up so any attacks can be set up after.
    /// </summary>
    /// <param name="bossAI">The boss ai this will link to.</param>
    public abstract void SetUpAttack(BossAI bossAI);

    /// <summary>
    /// Begin the attack. You may want to put a coroutine for the attack and start it here.
    /// </summary>
    public abstract void StartAttack();

    /// <summary>
    /// Call this to allow the boss to move onto another attack or this one again.
    /// </summary>
    protected virtual void AttackFinished()
    {
        bossAI.AttackConcluded();
    }
}
