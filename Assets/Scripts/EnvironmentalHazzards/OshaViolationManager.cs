using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;


// TODO: Shit name, rename later
/// <summary>
/// manages the kill dozers gates and attacks one at a time.
/// </summary>
public class OshaViolationManager : MonoBehaviour
{
    [SerializeField]
    Dozer[] dozers;

    private bool inJob = false;

    [SerializeField]
    private float coolDown = 3f;

    [SerializeField]
    EnemySpawnAtGates[] enemySpawnAtGates;

    private float currentCoolDown = 0f;

    private int currentCompletedEnemySpawnCount = 0;

    public event Action OnEnemySpawnAttackConcluded;

    // bool spawnedEnemies = false; // ! DEBUG REMOVE // TODO: REMOVE

    void Awake()
    {
        foreach (Dozer dozer in dozers)
        {
            dozer.OnJobCompleted += OnJobCompleted;
        }


        foreach (EnemySpawnAtGates enemySpawn in enemySpawnAtGates)
        {
            enemySpawn.OnSpawnedAllEnemies += OnEnemySpawnFinished;
        }
    }

    void Update()
    {
        // if (!spawnedEnemies)
        // {
        //     StartAEnemySpawnAttack();
        //     return;
        // }
        // return;
        if (currentCoolDown > 0) currentCoolDown -= Time.deltaTime;

        // StartADozerAttack(); // ! DEBUG CODE
    }

    public bool IsStillInJob()
    {
        return inJob;
    }

    public bool StartAEnemySpawnAttack(int count)
    {
        if (inJob) return false;
        currentCompletedEnemySpawnCount = 0;
        StartCoroutine(StartEnemySpawnAttack(count));

        return true;
    }

    public bool StartADozerAttack()
    {
        if (inJob || currentCoolDown > 0) return false;

        StartCoroutine(StartDozerAttack());
        return true;
    }

    private IEnumerator StartEnemySpawnAttack(int count)
    {
        inJob = true;
        // print("Spawnaegpineragnipaegipn");
        // foreach (EnemySpawnAtGates enemySpawn in enemySpawnAtGates)
        // {
        bool result = enemySpawnAtGates[Random.Range(0, enemySpawnAtGates.Length)].StartEnemyAttack(count);

        while (!result) // keep trying
        {
            result = enemySpawnAtGates[Random.Range(0, enemySpawnAtGates.Length)].StartEnemyAttack(count);
        }
        // yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 1f));
        // }
        yield return new WaitForEndOfFrame();


        while (currentCompletedEnemySpawnCount < 1)
        {
            yield return new WaitForEndOfFrame();
        }

        OnEnemySpawnAttackConcluded?.Invoke();

        inJob = false;
        // spawnedEnemies = true;
    }

    private IEnumerator StartDozerAttack()
    {
        inJob = true;

        Dozer dozer = dozers[UnityEngine.Random.Range(0, dozers.Length)];

        while (!dozer.TryToStartAttack())
        {
            dozer = dozers[UnityEngine.Random.Range(0, dozers.Length)];
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnJobCompleted()
    {
        inJob = false;
        currentCoolDown = coolDown;
    }

    private void OnEnemySpawnFinished()
    {
        currentCompletedEnemySpawnCount++;
    }
}
