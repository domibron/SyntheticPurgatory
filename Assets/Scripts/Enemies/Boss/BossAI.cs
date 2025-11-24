using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class BossAI : BaseEnemy
{

    private enum CurrentState
    {
        OperateButtons,
        ThinkingOfAttack,
        MeleeLunge,
        FireProjectile,
        EnterArena,
        EnterControlRoom,
    }

    CurrentState currentState = CurrentState.OperateButtons;

    private bool isUsingButtonAttack = false;

    private int buttonAttackCount = 0;

    private const int maxButtonAttackCount = 1; // what the helly

    private bool inControlRoom = true; // fuck me

    private int lastAttackIndex = 0;

    private Transform player;

    [SerializeField]
    private Transform controlRoom;

    [SerializeField]
    private BossArenaAttackManager arenaAttacks;


    [SerializeField]
    private LayerMask ground;

    private bool isMeleeLunging = false;
    private bool isFiringGun = false;

    private float defaultSpeed = 0f;

    [SerializeField]
    private GameObject bossProjectile;

    [SerializeField]
    private float gunCooldown = 3f;
    private float currentGunCooldown = 0f;

    private event Action<CurrentState, CurrentState> onCurrentStateChanged;

    private bool playerEnteredArena = false;

    [SerializeField]
    private float fallBackAfterTakenDamage = 0.3333333f;

    private Health health;

    private float lastHealthPercentage = 1f;

    [SerializeField]
    private Door bossDoor;

    private bool wantsToGoToControlRoom = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetUpAttacks();

        defaultSpeed = agent.speed;

        // store the player ref.
        player = PlayerRefFetcher.Instance.transform;

        health = GetComponent<Health>();

        lastHealthPercentage = health.GetHealthNormalized();


        health.onDeath += OnDeath;
        health.onHealthChanged += OnTakeDamage;
    }

    private void OnTakeDamage(float newHealth, float oldHealth)
    {

        if (health.GetHealthNormalized() <= lastHealthPercentage - fallBackAfterTakenDamage)
        {
            lastHealthPercentage = health.GetHealthNormalized(); // this could lead to issues down the line.
            wantsToGoToControlRoom = true;
        }
    }

    private void OnDeath()
    {
        Destroy(this.gameObject);
        print("Win!!!!");
    }

    protected override void Update()
    {
        base.Update();

        if (currentGunCooldown > 0) currentGunCooldown -= Time.deltaTime;

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

        if (!playerEnteredArena) return;

        switch (currentState)
        {
            case CurrentState.OperateButtons:

                if (wantsToGoToControlRoom) wantsToGoToControlRoom = false;

                if (!isUsingButtonAttack && buttonAttackCount < maxButtonAttackCount)
                {
                    // do another button attack
                    PickRandomAttackAndWait();
                }
                else if (!isUsingButtonAttack && buttonAttackCount >= maxButtonAttackCount)
                {
                    // arenaAttacks.StartJuggleAttack(); // maybe not
                    SetCurrentState(CurrentState.EnterArena);
                }

                break;
            case CurrentState.EnterArena:
                ExitControlRoom();
                break;
            case CurrentState.EnterControlRoom:
                EnterControlRoom();
                break;
            case CurrentState.ThinkingOfAttack:
                ThinkingOfAttack();
                break;
            case CurrentState.MeleeLunge:
                if (!isMeleeLunging) StartCoroutine(MeleeLunge());
                break;
            case CurrentState.FireProjectile:
                if (!isFiringGun) StartCoroutine(FireGun());
                break;
        }

    }

    public void PlayerEnteredArena()
    {
        playerEnteredArena = true;
    }

    private void SetCurrentState(CurrentState newState)
    {
        onCurrentStateChanged?.Invoke(currentState, newState);
        currentState = newState;
    }



    private void ThinkingOfAttack()
    {

        if (wantsToGoToControlRoom)
        {
            SetCurrentState(CurrentState.EnterControlRoom);
        }

        if (Vector3.Distance(controlRoom.position, transform.position) > 5f)
        {
            bossDoor.SetDoorState(false);
        }

        float playerDistance = Vector3.Distance(player.position, transform.position);
        if (playerDistance < 5f) // lunge distance.
        {
            SetCurrentState(CurrentState.MeleeLunge);
        }
        else if (!Physics.Linecast(player.position, transform.position, LayerMask.GetMask("Default", "Ground")) && currentGunCooldown <= 0f)
        {
            // TODO: this please, check the fucking layers please. Later, later
            SetCurrentState(CurrentState.FireProjectile);
        }
        else if (UnityEngine.AI.NavMesh.SamplePosition(player.position, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            agent.destination = player.position;
        }
        else if (agent.remainingDistance < 1f)
        {
            // Dumbass mode enabled.
            UnityEngine.AI.NavMesh.SamplePosition(transform.position + UnityEngine.Random.insideUnitSphere.normalized * 5f, out hit, 10f, NavMesh.AllAreas);
            agent.destination = hit.position;
        }


        // else // TODO: this is shit, fix this terrible shit.
        // {
        //     SetCurrentState(CurrentState.EnterControlRoom);
        // }
    }


    private IEnumerator FireGun()
    {
        isFiringGun = true;
        agent.speed = 0f;

        // charge up the attack
        float timer = 1f;
        while (timer > 0)
        {
            yield return new WaitForEndOfFrame();
            timer -= Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(new Vector3(player.position.x, transform.position.y, player.position.z) - transform.position, Vector3.up);
            // print("timer wait " + timer);
        }

        // fire!
        transform.rotation = Quaternion.LookRotation(new Vector3(player.position.x, transform.position.y, player.position.z) - transform.position, Vector3.up);
        GameObject projectile = Instantiate(bossProjectile, transform.position + transform.up + transform.forward, Quaternion.identity);
        projectile.GetComponent<ProjectileScript>().ProjectileDamage = 20f;

        Vector3 dirNeeded = (player.position - projectile.transform.position).normalized;
        projectile.GetComponent<Rigidbody>()?.AddForce(dirNeeded * 20f, ForceMode.VelocityChange);

        // recover
        timer = 1f;
        while (timer > 0)
        {
            yield return new WaitForEndOfFrame();
            timer -= Time.deltaTime;
            // print("timer recover " + timer);
        }

        currentGunCooldown = gunCooldown;
        agent.speed = defaultSpeed;
        isFiringGun = false;
        SetCurrentState(CurrentState.ThinkingOfAttack);
        yield return null;
    }


    private IEnumerator MeleeLunge()
    {
        // setup
        isMeleeLunging = true;

        float quitTimer = 5f;

        // Get close.
        while (agent.remainingDistance > 5f)
        {
            agent.destination = player.position;
            yield return new WaitForEndOfFrame();
            quitTimer -= Time.deltaTime;

            if (quitTimer <= 0)
            {
                SetCurrentState(CurrentState.ThinkingOfAttack); // Rethink your actions!
                yield break; // Get the fuck out
            }
            // print("distance");
        }

        agent.destination = transform.position;

        // charge up attack
        agent.speed = 0.1f;
        yield return new WaitForEndOfFrame();

        float timer = 1f;
        while (timer > 0)
        {
            yield return new WaitForEndOfFrame();
            timer -= Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(new Vector3(player.position.x, transform.position.y, player.position.z) - transform.position, Vector3.up);
            // print("timer wait " + timer);
        }

        agent.enabled = false;

        // lunge at the player
        Vector3 lungeTarget = player.position;
        float force = Vector3.Distance(transform.position, lungeTarget) * 2f;
        float angleNeeded = MathematicsUtility.GetAngleForFireProjectile(transform.position, lungeTarget, force, ArcType.DirectCurve);

        Vector3 playerPosWithYSameAsUs = new Vector3(player.position.x, transform.position.y, player.position.z);

        transform.rotation = Quaternion.LookRotation(playerPosWithYSameAsUs - transform.position, Vector3.up);

        Vector3 dir = playerPosWithYSameAsUs - transform.position;
        dir.y = 0f;

        Vector3 directionRight = Quaternion.AngleAxis(-90, Vector3.up) * dir;

        Vector3 jumpAngle = Quaternion.AngleAxis(angleNeeded, directionRight) * (playerPosWithYSameAsUs - transform.position);

        rb.AddForce(-rb.linearVelocity + (jumpAngle.normalized * force), ForceMode.VelocityChange);
        print(rb.linearVelocity);
        // while (true)
        // {
        //     if (Vector3.Distance(transform.position, lungeTarget) < 1f) break;
        //     yield return new WaitForEndOfFrame();
        // }

        while (Physics.CheckSphere(transform.position, 0.3f, ground)) yield return new WaitForEndOfFrame();

        do
        {
            yield return new WaitForEndOfFrame();
        }
        while (!Physics.CheckSphere(transform.position, 0.3f, ground) && !Physics.CheckBox(transform.position + (transform.forward * 0.5f),
            new Vector3(1f, 2f, 0.5f), transform.rotation, LayerMask.GetMask(Constants.PlayerLayer)));

        // so bad. we already checked for this.
        if (Physics.CheckSphere(transform.position, 0.3f, ground))
        {
            // print("BOOM!");
            GroundSlam();
        }
        else if (Physics.CheckBox(transform.position + (transform.forward * 0.5f),
            new Vector3(1f, 2f, 0.5f), transform.rotation, LayerMask.GetMask(Constants.PlayerLayer)))
        {
            LungeHitPlayer();
        }

        rb.AddForce(-rb.linearVelocity, ForceMode.VelocityChange);

        // recover
        timer = 1f;
        while (timer > 0)
        {
            yield return new WaitForEndOfFrame();
            timer -= Time.deltaTime;
            // print("timer recover " + timer);
        }


        // while (!agent.isActiveAndEnabled)
        // {
        //     yield return new WaitForEndOfFrame();
        //     agent.enabled = false;
        //     yield return new WaitForEndOfFrame();
        //     agent.enabled = true;
        // }

        print("End of melee");
        UnityEngine.AI.NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 10f, NavMesh.AllAreas);

        transform.position = hit.position;
        agent.enabled = true;

        // end
        agent.speed = defaultSpeed;
        isMeleeLunging = false;
        SetCurrentState(CurrentState.ThinkingOfAttack);
        yield return null;
    }

    private void GroundSlam()
    {
        if (Vector3.Distance(transform.position, player.position) < 4f)
        {
            player.GetComponent<Health>()?.AddToHealth(-30f);
        }
    }

    private void LungeHitPlayer()
    {
        player.GetComponent<Health>()?.AddToHealth(-50f);
    }

    private void EnterControlRoom()
    {
        agent.SetDestination(controlRoom.position);
        bossDoor.SetDoorState(true);


        if (inControlRoom && Vector3.Distance(agent.destination, transform.position) < 3f)
        {
            agent.SetDestination(transform.position);
            bossDoor.SetDoorState(false);
            SetCurrentState(CurrentState.OperateButtons);
        }
    }

    // Get the fuck out of control room // GET OUT! ~ Tuco Salamanca
    private void ExitControlRoom()
    {
        agent.SetDestination(player.position);
        bossDoor.SetDoorState(true);

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
            buttonAttackCount = 0;
        }
    }

    public void AttackConcluded()
    {
        print("A boss attack concluded.");
        isUsingButtonAttack = false;
        buttonAttackCount++;
    }

    private void SetUpAttacks()
    {
        // if (arenaAttacks.Length <= 0)
        // {
        //     Debug.LogError("There are no arena attacks!");
        //     return;
        // }

        // foreach (var attack in arenaAttacks)
        // {
        //     attack.SetUpAttack(this);
        // }

        arenaAttacks.SetUpAttack(this);
    }

    private void PickRandomAttackAndWait()
    {
        if (isUsingButtonAttack) return;

        // if (arenaAttacks.Length <= 0)
        // {
        //     AttackConcluded();
        //     Debug.LogError("There are no arena attacks!");
        //     return;
        // }

        // int attackIndex = UnityEngine.Random.Range(0, arenaAttacks.Length);

        // while (attackIndex != lastAttackIndex && arenaAttacks.Length > 1)
        // {
        //     attackIndex = UnityEngine.Random.Range(0, arenaAttacks.Length);
        // }

        isUsingButtonAttack = true;

        // lastAttackIndex = attackIndex;

        // arenaAttacks[attackIndex].StartAttack();

        arenaAttacks.StartAttack();

    }
}
