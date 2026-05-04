using UnityEngine;

public abstract class BossArenaAttack : MonoBehaviour
{
    protected BossAI bossAI;

    public abstract void SetUpAttack(BossAI bossAI);

    public abstract void StartAttack();
}
