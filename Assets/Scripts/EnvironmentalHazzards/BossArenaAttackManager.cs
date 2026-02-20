using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossArenaAttackManager : BossArenaAttack
{
    [SerializeField]
    OshaViolationManager oshaViolationManager;

    [SerializeField]
    CraneController[] craneControllers;

    [SerializeField]
    ArenaWallsManager arenaWallsManager;

    [SerializeField]
    float dropAttackCoolDown = 15f;
    float currentDropAttackCoolDown = 0f;

    private bool waitingForEnemiesToSpawn = false;

    [SerializeField]
    bool debugRemoveAttacks = false;

    void Awake()
    {
        oshaViolationManager.OnEnemySpawnAttackConcluded += EnemySpawnAttackConcluded;

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }



    // Update is called once per frame
    void Update()
    {
        if (currentDropAttackCoolDown > 0) currentDropAttackCoolDown -= Time.deltaTime;
    }

    public void StartJuggleAttack()
    {
        arenaWallsManager.StartJuggleJob();
    }

    private void EnemySpawnAttackConcluded()
    {
        waitingForEnemiesToSpawn = false;
    }

    private bool StillEnemiesInLevel()
    {
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag(Constants.EnemyTag);
        // print(allEnemies.Length);
        return allEnemies.Length > 0;

    }

    private IEnumerator BeginAttacks()
    {
        waitingForEnemiesToSpawn = true;

        // fuck you, terribly named variables, ahhhhh. ~ just couldn't be bothered to come up with a good name nor set this up correctly.
        bool a = false;
        bool b = true;

        while (!a || !b)
        {
            if (!a)
            {
                if (oshaViolationManager.StartAEnemySpawnAttack(10)) a = true;
            }

            // what the heck. I find this entertaining.
            // if (!b) if (arenaWallsManager.StartJuggleJob()) b = true;
            b = true; // disabled juggle walls.

            yield return new WaitForEndOfFrame();
        }

        while (waitingForEnemiesToSpawn || StillEnemiesInLevel())
        {
            yield return new WaitForEndOfFrame();

            TriggerCraneDropContainerAttack();

            oshaViolationManager.StartADozerAttack();
        }

        while (StillEnemiesInLevel() || oshaViolationManager.IsStillInJob())// || AnyCraneStillInJob())
        {
            // print(StillEnemiesInLevel() + " " + oshaViolationManager.IsStillInJob() + " " + AnyCraneStillInJob());
            yield return new WaitForEndOfFrame();
        }

        print("Global Attack Manager Concluded");
        AttackFinished();
    }

    private bool TriggerCraneDropContainerAttack()
    {
        if (currentDropAttackCoolDown > 0) return false;

        currentDropAttackCoolDown = dropAttackCoolDown;
        StartCoroutine(StartContainerDropAttack());

        return true;
    }

    private bool AnyCraneStillInJob() // TODO: Figure out why this is not working correctly.
    {
        foreach (CraneController crane in craneControllers)
        {
            if (crane.IsStillInJob()) return true;
        }

        return false;
    }

    private IEnumerator StartContainerDropAttack()
    {
        List<CraneController> controllers = craneControllers.ToList();

        while (true)
        {
            if (controllers.Count > 0)
            {
                if (controllers[Random.Range(0, controllers.Count)].StartDropContainerJob()) yield break;
            }
            else
            {
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private void AttackFinished()
    {
        bossAI.AttackConcluded();
    }

    public override void SetUpAttack(BossAI bossAI)
    {
        this.bossAI = bossAI;
    }

    public override void StartAttack()
    {
        if (debugRemoveAttacks)
        {
            AttackFinished();
            return;
        }

        StartCoroutine(BeginAttacks());
    }
}
