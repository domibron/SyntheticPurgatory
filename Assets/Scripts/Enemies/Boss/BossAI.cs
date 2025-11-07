using UnityEngine;


public class BossAI : BaseEnemy
{
    private enum CurrentState
    {
        OperateButtons,
        KeepDistance,
        MeleeCharge,
        FireProjectile,
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

}
