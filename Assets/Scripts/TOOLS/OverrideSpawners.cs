using System;
using System.Collections;
using UnityEngine;

public class OverrideSpawners : SequenceBase
{
    public override event Action OnThisSequenceEnd;

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
        EnemyGroupSpawner[] enemySpawners = Transform.FindObjectsByType<EnemyGroupSpawner>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        RandomizeEnvironmentPiece[] propSpawners = Transform.FindObjectsByType<RandomizeEnvironmentPiece>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

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

        OnThisSequenceEnd?.Invoke();
    }
}
