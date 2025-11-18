using System.Collections;
using UnityEngine;

public class BossArenaAttackManager : MonoBehaviour
{
    [SerializeField]
    OshaViolationManager oshaViolationManager;

    [SerializeField]
    CraneController[] craneControllers;

    [SerializeField]
    ArenaWallsManager arenaWallsManager;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(BeginAttacks());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator BeginAttacks()
    {
        oshaViolationManager.StartAEnemySpawnAttack();

        while (true)
        {
            yield return new WaitForEndOfFrame();

            TriggerCraneDropContainerAttack();

            arenaWallsManager.StartJuggleJob();

            oshaViolationManager.StartADozerAttack();
        }
    }

    private void TriggerCraneDropContainerAttack()
    {
        foreach (var controller in craneControllers)
        {
            controller.StartDropContainerJob();
        }
    }
}
