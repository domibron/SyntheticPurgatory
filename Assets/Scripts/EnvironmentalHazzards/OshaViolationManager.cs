using System;
using System.Collections;
using UnityEngine;

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

    bool spawnedEnemies = false; // ! DEBUG REMOVE // TODO: REMOVE

    void Awake()
    {
        foreach (Dozer dozer in dozers)
        {
            dozer.OnJobCompleted += OnJobCompleted;
        }


        foreach (EnemySpawnAtGates enemySpawn in enemySpawnAtGates)
        {
            enemySpawn.OnJobCompleted += OnEnemySpawnFinished;
        }
    }

    void Update()
    {
        if (!spawnedEnemies)
        {
            StartAEnemySpawnAttack();
            return;
        }
        // return;
        if (currentCoolDown > 0) currentCoolDown -= Time.deltaTime;

        StartADozerAttack(); // ! DEBUG CODE
    }

    public bool StartAEnemySpawnAttack()
    {
        if (inJob) return false;
        currentCompletedEnemySpawnCount = 0;
        StartCoroutine(StartEnemySpawnAttack());

        return true;
    }

    public bool StartADozerAttack()
    {
        if (inJob || currentCoolDown > 0) return false;

        StartCoroutine(StartDozerAttack());
        return true;
    }

    private IEnumerator StartEnemySpawnAttack()
    {
        inJob = true;

        foreach (EnemySpawnAtGates enemySpawn in enemySpawnAtGates)
        {
            enemySpawn.StartEnemyAttack();
            yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 1f));
        }

        while (currentCompletedEnemySpawnCount < enemySpawnAtGates.Length)
        {
            yield return new WaitForEndOfFrame();
        }

        inJob = false;
        spawnedEnemies = true;
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
