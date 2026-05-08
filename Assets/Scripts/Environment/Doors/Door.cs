using System.Collections;
using UnityEngine;

/// <summary>
/// What state the door is overridden to.
/// </summary>
public enum DoorOverrideState
{
    None,
    Closed,
    Open,
}

/// <summary>
/// Allows for a simple open and close door animation to work.
/// </summary>
public class Door : MonoBehaviour
{
    [SerializeField]
    private bool isDoorOpen = false;
    private bool desiredDoorState = false; // This was used to attempt to implement culling.
    // A better was it to tie into animation events and have the door closed fire a success event here and a open event here to tie into the culling system.
    // That would be in theory a more better and reliable solution than what ever this was. A key thing that was need was knowing when the door was fully closed.

    private DoorOverrideState doorOverrideState = DoorOverrideState.None;

    private Animator animator;

    private bool inMotion = false; // Legacy code?

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (doorOverrideState != DoorOverrideState.None)
        {
            animator.SetBool("isOpen", (doorOverrideState == DoorOverrideState.Open ? true : false));
        }
        else
        {
            animator.SetBool("isOpen", isDoorOpen);
        }

    }

    public void SetOverrideState(DoorOverrideState overrideState)
    {
        doorOverrideState = overrideState;

        switch (doorOverrideState)
        {
            case DoorOverrideState.Closed:
                desiredDoorState = false;
                break;
            case DoorOverrideState.Open:
                desiredDoorState = true;
                break;
        }
    }

    public void ResetOverrideState()
    {
        doorOverrideState = DoorOverrideState.None;
        desiredDoorState = isDoorOpen;
    }

    public void SetDoorState(bool isOpen)
    {
        isDoorOpen = isOpen;
        desiredDoorState = isOpen;
    }

    public void ToggleDoorState()
    {
        StartCoroutine(RandomDoorDelay(!isDoorOpen));
    }

    public void OpenDoor()
    {
        // Due to how the coroutine is set up, you can have a edge case where the door needed stat is to close and has a delay of 0.01 
        // but a open of a delay of 0.2 was called just before. You will will then have a open door, a door that is now in the wrong state.
        StartCoroutine(RandomDoorDelay(true)); // race conditions!
    }

    public void CloseDoor()
    {
        StartCoroutine(RandomDoorDelay(false)); // race conditions!
    }

    /// <summary>
    /// Sets the door override state to closed forcing the door to be kept close.
    /// </summary>
    public void OverrideClose()
    {
        doorOverrideState = DoorOverrideState.Closed;
        desiredDoorState = false;
    }

    /// <summary>
    /// Sets the door override state to open forcing the door to be kept open.
    /// </summary>
    public void OverrideOpen()
    {
        doorOverrideState = DoorOverrideState.Open;
        desiredDoorState = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="doorState"></param>
    /// <param name="maxPossibleDelay"></param>
    /// <returns></returns>
    IEnumerator RandomDoorDelay(bool doorState, float maxPossibleDelay = 0.2f)
    {
        desiredDoorState = doorState;
        yield return new WaitForSeconds(Random.Range(0, maxPossibleDelay));
        SetDoorState(doorState);
    }

    public bool IsDoorOpen()
    {
        return desiredDoorState;
    }

}
