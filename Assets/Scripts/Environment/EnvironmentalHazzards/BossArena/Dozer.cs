using System;
using System.Collections;
using UnityEngine;

// TODO: Rename this script, its a bad name.
public class Dozer : MonoBehaviour
{
    // I really hate this script, too messy, too linked to other scripts, too depended on other things.
    // TODO: refactor and adjust to follow some SOLID principles.
    // Could have a event with a bool is left to delink most of this.

    /// <summary>
    /// The left gate to open and close.
    /// </summary>
    [SerializeField]
    Door leftGate;

    /// <summary>
    /// The right gate to open and close.
    /// </summary>
    [SerializeField]
    Door rightGate;


    /// <summary>
    /// The left monitor to flash or warn if the vehicle is spawning there. 
    /// </summary>
    [SerializeField]
    WarningIndicator leftMonitor;

    /// <summary>
    /// The right monitor to flash or warn if the vehicle is spawning there. 
    /// </summary>
    [SerializeField]
    WarningIndicator rightMonitor;


    //TODO: Rename, bad names here, will leave since only used in arena.

    // These basically are 2 points of the gate where the vehicles will randomly spawn in between to make sure all of the area is covered.
    // This helps prevent the player just hugging the walls even though they still can.

    // LEFT

    /// <summary>
    /// The closest point of the left side spawn.
    /// </summary>
    [SerializeField]
    Transform leftForkliftSpawnLocationOne;

    /// <summary>
    /// The furthest point of the left side spawn.
    /// </summary>
    [SerializeField]
    Transform leftForkliftSpawnLocationTwo;


    // RIGHT

    /// <summary>
    /// The furthest point of the right side spawn.
    /// </summary>
    [SerializeField]
    Transform rightForkliftSpawnLocationOne;

    /// <summary>
    /// The closest point of the right side spawn.
    /// </summary>
    [SerializeField]
    Transform rightForkliftSpawnLocationTwo;



    [SerializeField]
    AudioSource leftAlarm;

    [SerializeField]
    AudioSource rightAlarm;


    /// <summary>
    /// The vehicle will move back out of bounds to save spawning and destroying objects.
    /// </summary>
    [SerializeField]
    Transform vehicleOutOfBounds;

    /// <summary>
    /// The collection of vehicles this event can move and use.
    /// </summary>
    [SerializeField]
    GameObject[] vehicles;

    /// <summary>
    /// The current selected vehicle.
    /// </summary>
    GameObject currentVehicle;

    /// <summary>
    /// The start point of the vehicle's path.
    /// </summary>
    Vector3 startPos;

    /// <summary>
    /// The end point of the vehicle's path.
    /// </summary>
    Vector3 endPos;

    /// <summary>
    /// How long to wait before letting the vehicle move across.
    /// </summary>
    [SerializeField]
    private float waitTime = 3f;

    /// <summary>
    /// The speed at which the vehicle will travel at.
    /// </summary>
    [SerializeField]
    private float speed = 0.3f;

    /// <summary>
    /// Is there already an attack happening.
    /// </summary>
    private bool isAttacking = false;

    /// <summary>
    /// The droppable platforms between the gates.
    /// </summary>
    [SerializeField]
    DroppablePlatform[] droppablePlatformsInPath;

    // private bool isOverridingGate = false;

    // public event Func<bool> OnJobCompleted;

    /// <summary>
    /// Called when the attack concludes.
    /// </summary>
    public event Action OnJobCompleted;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetEverything();
    }

    /// <summary>
    /// Tries to start a attack.
    /// </summary>
    /// <returns><b>TRUE</b> if successful.</returns>
    public bool TryToStartAttack()
    {
        // TODO: Check if floor tiles are still intact.
        if (isAttacking || !CanDoAttack()) return false;

        StartCoroutine(DoDozerAttack());
        return true;
    }

    /// <summary>
    /// Checks to see if all the platforms are there and there is no container being dropped.
    /// </summary>
    /// <returns><b>TRUE</b> if nothing is stopping the attack.</returns>
    private bool CanDoAttack()
    {
        foreach (var platform in droppablePlatformsInPath)
        {
            if (platform.HasDropped()) return false;
        }

        // TODO: fix the bug that allows the dozer to attack when the container is being dropped.
        // TODO: expensive call + phys layer is all.
        Collider[] colliders = Physics.OverlapBox(transform.position + Vector3.up * 4f, new Vector3(40f, 8f, 8f), Quaternion.identity, Physics.AllLayers, QueryTriggerInteraction.Collide);

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
