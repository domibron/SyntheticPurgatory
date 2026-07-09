using System;
using System.Collections;
using UnityEngine;

public class OverrideSpawners : SequenceBase
{
    public override event Action OnThisSequenceEnd;

    public ModuleLevelM upgradeCardManager;

    private Coroutine A;
    private Coroutine B;
    private Coroutine C;

    public override float GetProgress()
    {
        return 0;
    }

    public override void StartSequence()
    {
        StartCoroutine(BeginOverride());
    }

    IEnumerator BeginOverride()
    {
        upgradeCardManager.ActivateAndArm();


        A = StartCoroutine(EnemySpawn(FindObjectsByType<EnemyGroupSpawner>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)));

        B = StartCoroutine(RandomEnvPiece(FindObjectsByType<RandomizeEnvironmentPiece>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)));

        C = StartCoroutine(CardSpawn(FindObjectsByType<ModuleSpawner>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)));

        while (A != null && B != null && C != null)
        {
            yield return null;
        }


        OnThisSequenceEnd?.Invoke();

        print("Done with calling spawn entities.");
    }

    IEnumerator EnemySpawn(EnemyGroupSpawner[] enemySpawners)
    {
        foreach (EnemyGroupSpawner enemySpawner in enemySpawners)
        {
            enemySpawner.SpawnEnemies();

            yield return null;
        }

        A = null;
    }

    IEnumerator RandomEnvPiece(RandomizeEnvironmentPiece[] propSpawners)
    {
        foreach (RandomizeEnvironmentPiece propSpawner in propSpawners)
        {
            propSpawner.SpawnRandomProps();

            yield return null;
        }

        B = null;
    }

    IEnumerator CardSpawn(ModuleSpawner[] cardSpawners)
    {
        foreach (ModuleSpawner cardSpawner in cardSpawners)
        {
            cardSpawner.SpawnCard();

            yield return null;
        }

        C = null;
    }
}
