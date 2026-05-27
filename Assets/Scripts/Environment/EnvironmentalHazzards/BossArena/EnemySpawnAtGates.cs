using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Spawner for a single section fo gates in the boss arena.
/// </summary>
public class EnemySpawnAtGates : MonoBehaviour
{
    /// <summary>
    /// The left gate / door.
    /// </summary>
    [SerializeField]
    Door leftGate;

    /// <summary>
    /// The right gate / door.
    /// </summary>
    [SerializeField]
    Door rightGate;

    /// <summary>
    /// The left monitor warning indicator.
    /// </summary>
    [SerializeField]
    WarningIndicator leftMonitor;

    /// <summary>
    /// The right monitor warning indicator.
    /// </summary>
    [SerializeField]
    WarningIndicator rightMonitor;

    // I HATE THE AUDIO... AGH.
    // TODO: fix later. WWise implementation hopefully.

    /// <summary>
    /// Left side alarm.
    /// </summary>
    [SerializeField]
    AudioSource leftAlarm;

    /// <summary>
    /// Right side alarm.
    /// </summary>
    [SerializeField]
    AudioSource rightAlarm;

    // TODO: have a enemy list table.

    /// <summary>
    /// The enemies prefabs that can spawn.
    /// </summary>
    [SerializeField]
    GameObject[] enemyPrefabs;

    /// <summary>
    /// Left side enemy spawn point.
    /// </summary>
    [SerializeField]
    Transform leftSpawnPoint;

    /// <summary>
    /// Right side enemy spawn point.
    /// </summary>
    [SerializeField]
    Transform rightSpawnPoint;

    /// <summary>
    /// Are we doing a attack right now?
    /// </summary>
    private bool isAttacking = false;

    /// <summary>
    /// Event to let others know we are finished.
    /// </summary>
    public event Action OnJobCompleted;

    /// <summary>
    /// Right gate hold, do not close.
    /// </summary>
    private bool rightGateHold = false;

    /// <summary>
    /// Left gate hold, do not close.
    /// </summary>
    private bool leftGateHold = false;

    /// <summary>
    /// All the enemies have been spawned successfully. Wave spawned.
    /// </summary>
    public event Action OnSpawnedAllEnemies;

    void Awake()
    {
        ResetEverything();
    }

    /// <summary>
    /// Try to start an enemy attack with the given amount.
    /// </summary>
    /// <param name="count">The amount of enemies to spawn.</param>
    /// <returns>True if the job was assigned successfully.</returns>
    public bool StartEnemyAttack(int count)
    {
        if (isAttacking) return false;
        StartCoroutine(SpawnEnemiesAtGates(count));

        return true;
    }

    /// <summary>
    /// Coroutine that is responsible for spawning the enemies and managing the system.
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    private IEnumerator SpawnEnemiesAtGates(int count)
    {
        isAttacking = true;
        StartEverything();
        int enemyCount = 0;

        // spawn the enemies.
        while (enemyCount < count)
        {
            GameObject enemy = Instantiate(enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)], (UnityEngine.Random.Range(0, 2) <= 0 ? leftSpawnPoint.position : rightSpawnPoint.position), Quaternion.identity);

            enemy.GetComponent<EnemyDetection>()?.BecomeAlert(false, 0, 0f);
            enemy.GetComponent<EnemyDetection>()?.ChangeCanLoseAgro(false);
            enemyCount++;

            yield return new WaitForSeconds(UnityEngine.Random.Range(0.3f, 3f));
        }

        yield return new WaitForSeconds(1f);

        // Wait for the gates to close.
        while (leftGateHold || rightGateHold) yield return new WaitForEndOfFrame();

        // We finished.
        OnJobCompleted?.Invoke();
        OnSpawnedAllEnemies?.Invoke();


        ResetEverything();
        isAttacking = false;
    }

    /// <summary>
    /// Resets the section.
    /// </summary>
    private void ResetEverything()
    {
        CloseGates();
        TurnOffMonitors();
        StopAlarms();
    }

    /// <summary>
    /// Sets the alarms and warnings etc.
    /// </summary>
    private void StartEverything()
    {
        OpenGates();
        StartMonitorAlerts();
        PlayAlarms();
    }

    /// <summary>
    /// Open both the gates.
    /// </summary>
    private void OpenGates()
    {
        leftGate.SetDoorState(true);
        rightGate.SetDoorState(true);
    }

    /// <summary>
    /// Close both the gates.
    /// </summary>
    private void CloseGates()
    {
        leftGate.SetDoorState(false);
        rightGate.SetDoorState(false);
    }

    /// <summary>
    /// Turn off both the monitors.
    /// </summary>
    private void TurnOffMonitors()
    {
        leftMonitor.EndMonitor();
        rightMonitor.EndMonitor();
    }

    /// <summary>
    /// Turn on and enable the monitor warning.
    /// </summary>
    private void StartMonitorAlerts()
    {
        leftMonitor.SetBGColor(Color.blue);
        rightMonitor.SetBGColor(Color.blue);

        leftMonitor.StartAlert();
        rightMonitor.StartAlert();
    }

    /// <summary>
    /// Silence the alarms.
    /// </summary>
    private void StopAlarms()
    {
        leftAlarm.Stop();
        rightAlarm.Stop();
    }

    /// <summary>
    /// Activate the annoying alarms.
    /// </summary>
    private void PlayAlarms()
    {

        leftAlarm.Play();
        rightAlarm.Play();

    }

    /// <summary>
    /// Called to set the state that enemies are behind the right gate.
    /// </summary>
    /// <param name="hasDetected">True if there are enemies.</param>
    public void RightGateDetection(bool hasDetected)
    {
        rightGateHold = hasDetected;
    }

    /// <summary>
    /// Called to set the state that enemies are behind the left gate.
    /// </summary>
    /// <param name="hasDetected">True if there are enemies.</param>
    public void LeftGateDetection(bool hasDetected)
    {
        leftGateHold = hasDetected;
    }
}
