using System.Collections;
using UnityEngine;

public enum DoorOverrideState
{
    None,
    Closed,
    Open,
}

public class Door : MonoBehaviour
{
    private bool isDoorOpen = false;

    private DoorOverrideState doorOverrideState = DoorOverrideState.None;

    private Animator animator;

    private bool inMotion = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
    }

    public void ResetOverrideState()
    {
        doorOverrideState = DoorOverrideState.None;
    }

    public void SetDoorState(bool isOpen)
    {
        isDoorOpen = isOpen;
    }

    public void ToggleDoorState()
    {
        StartCoroutine(RandomDoorDelay(!isDoorOpen));
    }

    public void OpenDoor()
    {
        StartCoroutine(RandomDoorDelay(true)); // race conditions!
    }

    public void CloseDoor()
    {
        StartCoroutine(RandomDoorDelay(false)); // race conditions!
    }

    public void OverrideClose()
    {
        doorOverrideState = DoorOverrideState.Closed;
    }

    public void OverrideOpen()
    {
        doorOverrideState = DoorOverrideState.Open;
    }

    IEnumerator RandomDoorDelay(bool doorState, float maxPossibleDelay = 0.2f)
    {
        yield return new WaitForSeconds(Random.Range(0, maxPossibleDelay));
        SetDoorState(doorState);
    }

    public bool IsDoorOpen()
    {
        return isDoorOpen;
    }

}
