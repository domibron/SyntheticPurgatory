using System;
using System.Collections;
using UnityEngine;


public class BossAI : BaseEnemy
{

    private enum CurrentState
    {
        OperateButtons,
        ThinkingOfAttack,
        MeleeLunge,
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


    private bool isMeleeLunging = false;


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
            case CurrentState.ThinkingOfAttack:
                ThinkingOfAttack();
                break;
            case CurrentState.MeleeLunge:
                if (isMeleeLunging) StartCoroutine(MeleeLunge());
                break;
        }

    }


    private void SetCurrentState(CurrentState newState)
    {
        onCurrentStateChanged.Invoke(currentState, newState);
        currentState = newState;
    }



    private void ThinkingOfAttack()
    {
        float playerDistance = Vector3.Distance(player.position, transform.position);
        if (playerDistance < 5f) // lunge distance.
        {
            SetCurrentState(CurrentState.MeleeLunge);
        }
    }


    private IEnumerator MeleeLunge()
    {
        // setup
        isMeleeLunging = true;

        // Get close.
        while (agent.remainingDistance > 5f)
        {
            agent.destination = player.position;
            yield return null;
        }

        // charge up attack
        agent.speed = 0.1f;
        yield return new WaitForEndOfFrame();

        float timer = 1f;
        while (timer > 0)
        {
            yield return new WaitForEndOfFrame();
            timer -= Time.deltaTime;
        }

        // lunge at the player
        Vector3 lungeTarget = player.position;

        float angleNeeded = MathematicsUtility.GetAngleForFireProjectile(transform.position, lungeTarget, Vector3.Distance(transform.position, lungeTarget) * 4f, ArcType.HighCurve);

        // rb.AddForce()

        while (true)
        {
            if (Vector3.Distance(transform.position, lungeTarget) < 1f) break;
            yield return new WaitForEndOfFrame();
        }

        // recover


        // end
        SetCurrentState(CurrentState.ThinkingOfAttack);
        isMeleeLunging = false;

        yield return null;
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

        if (!inControlRoom)
        {
            SetCurrentState(CurrentState.ThinkingOfAttack);
        }
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
