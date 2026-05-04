using System;
using System.Collections;
using UnityEngine;

public class EnemySpawnAtGates : MonoBehaviour
{
    [SerializeField]
    Door leftGate;

    [SerializeField]
    Door rightGate;


    [SerializeField]
    WarningIndicator leftMonitor;

    [SerializeField]
    WarningIndicator rightMonitor;

    [SerializeField]
    AudioSource leftAlarm;

    [SerializeField]
    AudioSource rightAlarm;

    [SerializeField]
    OshaViolationManager oshaViolationManager; // for tracking enemies.

    [SerializeField]
    GameObject[] enemyPrefabs;

    [SerializeField]
    Transform leftSpawnPoint;

    [SerializeField]
    Transform rightSpawnPoint;

    private bool isAttacking = false;

    public event Action OnJobCompleted;

    private bool rightGateHold = false;
    private bool leftGateHold = false;

    public event Action OnSpawnedAllEnemies;

    void Awake()
    {
        ResetEverything();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool StartEnemyAttack(int count)
    {
        if (isAttacking) return false;
        StartCoroutine(SpawnEnemiesAtGates(count));

        return true;
    }

    private IEnumerator SpawnEnemiesAtGates(int count)
    {
        isAttacking = true;
        StartEverything();
        int enemyCount = 0;
        while (enemyCount < count)
        {
            GameObject enemy = Instantiate(enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)], (UnityEngine.Random.Range(0, 2) <= 0 ? leftSpawnPoint.position : rightSpawnPoint.position), Quaternion.identity);

            enemy.GetComponent<EnemyDetection>()?.BecomeAlert(false, 0, 0f);
            enemy.GetComponent<EnemyDetection>()?.ChangeCanLoseAgro(false);
            enemyCount++;

            yield return new WaitForSeconds(UnityEngine.Random.Range(0.3f, 3f));
        }

        yield return new WaitForSeconds(1f);

        while (leftGateHold || rightGateHold) yield return new WaitForEndOfFrame();

        OnJobCompleted?.Invoke();
        OnSpawnedAllEnemies?.Invoke();


        ResetEverything();
        isAttacking = false;
    }

    private void ResetEverything()
    {
        CloseGates();
        TurnOffMonitors();
        StopAlarms();
    }

    private void StartEverything()
    {
        OpenGates();
        StartMonitorAlerts();
        PlayAlarms();
    }

    private void OpenGates()
    {
        leftGate.SetDoorState(true);
        rightGate.SetDoorState(true);
    }

    private void CloseGates()
    {
        leftGate.SetDoorState(false);
        rightGate.SetDoorState(false);
    }

    private void TurnOffMonitors()
    {
        leftMonitor.EndMonitor();
        rightMonitor.EndMonitor();
    }

    private void StartMonitorAlerts()
    {
        leftMonitor.SetBGColor(Color.blue);
        rightMonitor.SetBGColor(Color.blue);

        leftMonitor.StartAlert();
        rightMonitor.StartAlert();
    }

    private void StopAlarms()
    {
        leftAlarm.Stop();
        rightAlarm.Stop();
    }

    private void PlayAlarms()
    {

        leftAlarm.Play();
        rightAlarm.Play();

    }

    public void RightGateDetection(bool hasDetected)
    {
        rightGateHold = hasDetected;
    }

    public void LeftGateDetection(bool hasDetected)
    {
        leftGateHold = hasDetected;
    }
}
