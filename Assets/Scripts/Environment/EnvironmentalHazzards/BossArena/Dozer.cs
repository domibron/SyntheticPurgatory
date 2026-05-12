using System;
using System.Collections;
using UnityEngine;

// TODO: Rename this script, its a bad name.
public class Dozer : MonoBehaviour
{
    [SerializeField]
    Door leftGate;

    [SerializeField]
    Door rightGate;


    [SerializeField]
    WarningIndicator leftMonitor;

    [SerializeField]
    WarningIndicator rightMonitor;


    // LEFT
    [SerializeField]
    Transform leftForkliftSpawnLocationOne;

    [SerializeField]
    Transform leftForkliftSpawnLocationTwo;


    // RIGHT
    [SerializeField]
    Transform rightForkliftSpawnLocationOne;

    [SerializeField]
    Transform rightForkliftSpawnLocationTwo;


    [SerializeField]
    AudioSource leftAlarm;

    [SerializeField]
    AudioSource rightAlarm;


    [SerializeField]
    Transform vehicleOutOfBounds;


    [SerializeField]
    GameObject[] vehicles;

    GameObject currentVehicle;

    Vector3 startPos;
    Vector3 endPos;

    [SerializeField]
    private float waitTime = 3f;

    [SerializeField]
    private float speed = 0.3f;

    private bool isAttacking = false;

    [SerializeField]
    DroppablePlatform[] droppablePlatformsInPath;

    // private bool isOverridingGate = false;

    // public event Func<bool> OnJobCompleted;
    public event Action OnJobCompleted;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetEverything();
    }

    // Update is called once per frame
    // void Update()
    // {
    //     // Debug-ing code
    //     if (isAttacking) return;
    //     StartCoroutine(DoDozerAttack());
    // }

    public bool TryToStartAttack()
    {
        // TODO: Check if floor tiles are still intact.
        if (isAttacking || !CanDoAttack()) return false;

        StartCoroutine(DoDozerAttack());
        return true;
    }

    private bool CanDoAttack()
    {
        foreach (var platform in droppablePlatformsInPath)
        {
            if (platform.HasDropped()) return false;
        }

        Collider[] colliders = Physics.OverlapBox(transform.position + Vector3.up * 4f, new Vector3(40f, 8f, 8f));

        if (colliders.Length <= 0) return true;

        foreach (var collider in colliders)
        {
            if (collider.isTrigger && collider.gameObject.CompareTag("ContainerChecker"))
            {
                return false;
            }
        }

        return true;

    }

    private IEnumerator DoDozerAttack()
    {
        StartCoroutine(ReadyAttack(UnityEngine.Random.Range(0, 2) <= 0));
        yield return new WaitForEndOfFrame();

        if (currentVehicle == null)
        {
            Debug.LogError("Well shit, currentVehicle is null");
        }

        yield return new WaitForSeconds(waitTime);


        yield return new WaitForEndOfFrame();

        float currentTime = 0f;

        while (currentTime < 1f)
        {
            yield return new WaitForEndOfFrame();

            currentVehicle.transform.position = Vector3.Lerp(startPos, endPos, currentTime);

            currentTime += Time.deltaTime * speed;
        }

        yield return new WaitForEndOfFrame();

        StartCoroutine(ResetAttack());
    }

    private IEnumerator ReadyAttack(bool isLeft)
    {
        isAttacking = true;




        if (isLeft)
        {
            startPos = Vector3.Lerp(leftForkliftSpawnLocationOne.position, leftForkliftSpawnLocationTwo.position, UnityEngine.Random.Range(0f, 1f));
            endPos = startPos;
            endPos.z = rightForkliftSpawnLocationOne.position.z;
        }
        else
        {
            startPos = Vector3.Lerp(rightForkliftSpawnLocationOne.position, rightForkliftSpawnLocationTwo.position, UnityEngine.Random.Range(0f, 1f));
            endPos = startPos;
            endPos.z = leftForkliftSpawnLocationOne.position.z;
        }

        currentVehicle = vehicles[UnityEngine.Random.Range(0, vehicles.Length)];

        if (!isLeft)
        {
            currentVehicle.transform.rotation = Quaternion.Euler(0, 180f, 0);
        }

        currentVehicle.transform.position = startPos;
        currentVehicle.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        OpenGates();
        StartFlash(isLeft);
        PlayAlarm(isLeft);
    }

    private IEnumerator ResetAttack()
    {
        CloseGates();
        TurnOffMonitors();
        StopAlarms();

        yield return new WaitForSeconds(0.7f);

        if (currentVehicle != null)
        {
            currentVehicle.SetActive(false);
            currentVehicle.transform.position = vehicleOutOfBounds.position;
            currentVehicle.transform.rotation = Quaternion.identity;
        }

        currentVehicle = null;
        isAttacking = false;

        OnJobCompleted?.Invoke();
    }

    private void ResetEverything()
    {
        CloseGates();
        TurnOffMonitors();
        StopAlarms();
        currentVehicle = null;
        isAttacking = false;
        // OnJobCompleted?.Invoke(); // ? maybe?
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

    private void StartFlash(bool isLeftSide)
    {
        leftMonitor.ResetBGColor();
        rightMonitor.ResetBGColor();


        if (isLeftSide)
        {
            leftMonitor.StartFlash();
            rightMonitor.ShowBGOnly();
        }
        else
        {
            rightMonitor.StartFlash();
            leftMonitor.ShowBGOnly();
        }
    }

    private void StopAlarms()
    {
        leftAlarm.Stop();
        rightAlarm.Stop();
    }

    private void PlayAlarm(bool isLeftSide)
    {
        if (isLeftSide)
        {
            leftAlarm.Play();
        }
        else
        {
            rightAlarm.Play();
        }
    }
}
