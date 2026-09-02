using UnityEngine;

public class TodEnemyAI : EnemyBase
{

    public override void KnockbackAI(Vector3 forceAndDir, float minimumTime = 0.3F, ForceMode forceMode = ForceMode.VelocityChange, bool playerSourced = true)
    {
        base.KnockbackAI(forceAndDir, minimumTime, forceMode, playerSourced);

        RunManager.Instance.statsHolder.todPunts++;
    }
}
