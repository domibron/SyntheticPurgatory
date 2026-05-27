using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This handles all the arena attacks for the boss.
/// </summary>
public class BossArenaAttackManager : BossArenaAttack
{
    /// <summary>
    /// The cars and enemy spawning manager.
    /// </summary>
    [SerializeField]
    OshaViolationManager oshaViolationManager;

    /// <summary>
    /// Crane managers to drop things.
    /// </summary>
    [SerializeField]
    CraneController[] craneControllers;

    /// <summary>
    /// Wall manager to shuffle the walls.
    /// </summary>
    [SerializeField]
    ArenaWallsManager arenaWallsManager;

    /// <summary>
    /// How long to wait before dropping another container.
    /// </summary>
    [SerializeField]
    float dropAttackCoolDown = 15f;

    /// <summary>
    /// The current cool down time for the container drop attack.
    /// </summary>
    float currentDropAttackCoolDown = 0f;

    /// <summary>
    /// A check to prevent doors from closing when enemies are still spawning.
    /// </summary>
    private bool waitingForEnemiesToSpawn = false;

    /// <summary>
    /// Skips the arena attacks for debug purposes. DO NOT LEAVE ENABLED.
    /// </summary>
    [SerializeField]
    bool debugRemoveAttacks = false;

    void Awake()
    {
        // Set up enemy spawn finish listener.
        oshaViolationManager.OnEnemySpawnAttackConcluded += EnemySpawnAttackConcluded;
    }


    // Update is called once per frame
    void Update()
    {
        // Timer for container dropping.
        if (currentDropAttackCoolDown > 0) currentDropAttackCoolDown -= Time.deltaTime;
    }

    /// <summary>
    /// Randomises the layout of the containers and gets the cranes to move them.
    /// </summary>
    public void StartJuggleAttack()
    {
        arenaWallsManager.StartJuggleJob();
    }


    /// <summary>
    /// Called when the enemies have all spawned.
    /// </summary>
    private void EnemySpawnAttackConcluded()
    {
        waitingForEnemiesToSpawn = false;
    }

    // TODO: replace with a tracker and add spawned in enemies to the list and remove them when they die. We have the infrastructure.
    /// <summary>
    /// Expensive check to see if there are enemies in the level.
    /// </summary>
    /// <returns>True if there are still enemies in the level.</returns>
    private bool StillEnemiesInLevel()
    {
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag(Constants.EnemyTag);
        // print(allEnemies.Length);
        return allEnemies.Length > 0;

    }

    /// <summary>
    /// Coroutine that managers the attacks.
    /// </summary>
    /// <returns></returns>
    private IEnumerator BeginAttacks()
    {
        // Set before enemies spawned so we wait until they have all finished spawning.
        waitingForEnemiesToSpawn = true;

        // fuck you, terribly named variables, ahhhhh. ~ just couldn't be bothered to come up with a good name nor set this up correctly.
        bool EnemySpawnAttackCalled = false;
        bool ContainerWallJuggleCalled = true;

        while (!EnemySpawnAttackCalled || !ContainerWallJuggleCalled)
        {
            if (!EnemySpawnAttackCalled)
            {
                if (oshaViolationManager.StartAEnemySpawnAttack(10)) EnemySpawnAttackCalled = true;
            }

            // what the heck. I find this entertaining. ~ Damn, these comments are getting old.

            // We used to juggle the walls when the enemies are being dealt with to create a confusing and chaotic space for the player.
            // The downside and the reason why it was disabled / removed was because we do not guarantee there is a path for the enemies to the player.
            // So unless the enemies jump over the walls or channels are cut out first, this will remain disabled.
            // Another reason is drop attacks cannot be called when the cranes are moving containers since they will be in a job continuously.

            // if (!b) if (arenaWallsManager.StartJuggleJob()) b = true;
            ContainerWallJuggleCalled = true; // disabled juggle walls.

            yield return new WaitForEndOfFrame();
        }

        // Whilst enemies are being spawned / being dealt with the player, we do dozer and container drop attacks in the meantime.
        while (waitingForEnemiesToSpawn || StillEnemiesInLevel())
        {
            yield return new WaitForEndOfFrame();

            TryCraneDropContainerAttack();

            oshaViolationManager.TryStartSingleDozerAttack();
        }

        while (StillEnemiesInLevel() || oshaViolationManager.IsStillInJob())// || AnyCraneStillInJob())
        {
            // print(StillEnemiesInLevel() + " " + oshaViolationManager.IsStillInJob() + " " + AnyCraneStillInJob());
            yield return new WaitForEndOfFrame();
        }

        print("Global Attack Manager Concluded");
        AttackFinished();
    }

    /// <summary>
    /// Try to get a crane to drop a container onto the arena. Will check for the cool down.
    /// </summary>
    /// <returns>True if successful at getting one crane to drop a container.</returns>
    private bool TryCraneDropContainerAttack()
    {
        if (currentDropAttackCoolDown > 0) return false;

        currentDropAttackCoolDown = dropAttackCoolDown;
        StartCoroutine(StartContainerDropAttack());

        return true;
    }

    /// <summary>
    /// Checks to see if at least one crane is in a job.
    /// </summary>
    /// <returns>True if one or more cranes are in a job.</returns>
    private bool AnyCraneStillInJob() // TODO: Figure out why this is not working correctly.
    {
        foreach (CraneController crane in craneControllers)
        {
            if (crane.IsStillInJob()) return true;
        }

        return false;
    }

    // TODO: WTH, this is kinda pointless to spam check like this. Just try to get a crane to drop a container. Exit when you cant.
    /// <summary>
    /// Tries to get one crane to drop a container.
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartContainerDropAttack()
    {
        // Mem alloc, kinda pointless, data is not manipulated.
        // I think originally cranes were removed from the list when they were currently in a job.
        // But something happened and we have this mess.
        List<CraneController> controllers = craneControllers.ToList();

        while (true)
        {
            if (controllers.Count > 0)
            {
                // If a crane was successfully given the drop container job, exit.
                if (controllers[Random.Range(0, controllers.Count)].StartDropContainerJob()) yield break;
            }
            else
            {
                yield break; // exit, there are not cranes available.
            }

            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// Sets up this arena attack manager.
    /// </summary>
    /// <param name="bossAI">The boss that is controlling us.</param>
    public override void SetUpAttack(BossAI bossAI)
    {
        this.bossAI = bossAI;
    }

    // * NOTE The boss will call this once and then wait until the completion event to be called.
    /// <summary>
    /// Boss calls this. Start the attack.
    /// </summary>
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
