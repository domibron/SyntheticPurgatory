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
        isDoorOpen = !isDoorOpen;
    }

    public void OpenDoor()
    {
        isDoorOpen = true;
    }

    public void CloseDoor()
    {
        isDoorOpen = false;
    }

    public void OverrideClose()
    {
        doorOverrideState = DoorOverrideState.Closed;
    }

    public void OverrideOpen()
    {
        doorOverrideState = DoorOverrideState.Open;
    }
}
