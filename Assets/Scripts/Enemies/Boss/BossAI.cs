using System;
using UnityEngine;


public class BossAI : BaseEnemy
{

    private enum CurrentState
    {
        OperateButtons,
        KeepDistance,
        MeleeCharge,
        FireProjectile,
        EnterArena,
        ExitArena,
    }

    CurrentState currentState = CurrentState.OperateButtons;

    private bool isUsingButtonAttack = false;

    private int buttonAttackCount = 1;

    private const int maxButtonAttackCount = 3; // what the helly

    private bool inControlRoom = true; // fuck me

    private int lastAttackIndex = 0;

    private Transform player;

    [SerializeField]
    private Transform controlRoom;

    [SerializeField]
    private BossArenaAttack[] arenaAttacks;

    private event Action<CurrentState, CurrentState> onCurrentStateChanged;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetUpAttacks();

        // store the player ref.
        player = PlayerRefFetcher.Instance.transform;
    }

    protected override void Update()
    {
        base.Update();


    }

    private void FixedUpdate()
    {

        if (enemyKnockedBack)
        {
            return;
        }
        else if (enemyStunned)
        {
            agent.destination = transform.position;
            return;
        }

        switch (currentState)
        {
            case CurrentState.OperateButtons:

                if (!isUsingButtonAttack && buttonAttackCount < maxButtonAttackCount)
                {
                    // do another button attack
                    PickRandomAttackAndWait();
                }
                else if (!isUsingButtonAttack && buttonAttackCount >= maxButtonAttackCount)
                {
                    SetCurrentState(CurrentState.EnterArena);
                }

                break;
            case CurrentState.EnterArena:
                ExitControlRoom();
                break;
            case CurrentState.ExitArena:
                EnterControlRoom();
                break;
        }

    }


    private void SetCurrentState(CurrentState newState)
    {
        onCurrentStateChanged.Invoke(currentState, newState);
        currentState = newState;
    }






    private void EnterControlRoom()
    {
        agent.SetDestination(controlRoom.position);

        if (inControlRoom && Vector3.Distance(agent.destination, transform.position) < 3f)
        {
            SetCurrentState(CurrentState.OperateButtons);
            agent.SetDestination(transform.position);
        }
    }

    // Get the fuck out of control room // GET OUT! ~ Tuco Salamanca
    private void ExitControlRoom()
    {

        agent.SetDestination(player.position);
    }


    public void SetIsBossInControlRoom(bool isInControlRoom)
    {
        inControlRoom = isInControlRoom;

        if (!isInControlRoom)
        {
            buttonAttackCount = 1;
        }
    }

    public void AttackConcluded()
    {
        isUsingButtonAttack = false;
        buttonAttackCount++;
    }

    private void SetUpAttacks()
    {
        if (arenaAttacks.Length <= 0)
        {
            throw new NullReferenceException("There are no arena attacks!");
        }

        foreach (var attack in arenaAttacks)
        {
            attack.SetUpAttack(this);
        }
    }

    private void PickRandomAttackAndWait()
    {
        if (arenaAttacks.Length <= 0)
        {
            throw new NullReferenceException("There are no arena attacks!");
        }

        int attackIndex = UnityEngine.Random.Range(0, arenaAttacks.Length);

        while (attackIndex != lastAttackIndex && arenaAttacks.Length > 1)
        {
            attackIndex = UnityEngine.Random.Range(0, arenaAttacks.Length);
        }

        lastAttackIndex = attackIndex;

        arenaAttacks[attackIndex].StartAttack();

    }
}
