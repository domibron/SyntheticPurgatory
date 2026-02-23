using System;
using System.Collections;
using UnityEngine;

public class OverrideSpawners : SequenceBase
{
    public override event Action OnThisSequenceEnd;

    public UpgradeCardManager upgradeCardManager;

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
        EnemyGroupSpawner[] enemySpawners = FindObjectsByType<EnemyGroupSpawner>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        RandomizeEnvironmentPiece[] propSpawners = FindObjectsByType<RandomizeEnvironmentPiece>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        UpgradeCardSpawner[] cardSpawners = FindObjectsByType<UpgradeCardSpawner>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        upgradeCardManager.ActivateAndArm();



        foreach (EnemyGroupSpawner enemySpawner in enemySpawners)
        {
            enemySpawner.SpawnEnemies();
            yield return null;
        }

        foreach (RandomizeEnvironmentPiece propSpawner in propSpawners)
        {
            propSpawner.SpawnRandomProps();
            yield return null;
        }

        foreach (UpgradeCardSpawner cardSpawner in cardSpawners)
        {
            cardSpawner.SpawnCard();
            yield return null;
        }

        OnThisSequenceEnd?.Invoke();

        print("Done with calling spawn entities.");
    }
}
