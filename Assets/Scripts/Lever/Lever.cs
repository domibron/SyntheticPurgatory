using UnityEngine;

public class Lever : MonoBehaviour, IKickable, IShootable
{
    private int roomID = -1;

    private DoorGenerator doorGenerator;

    private Animator animator;

    private bool state = false;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        animator.SetBool("isOn", state);
    }

    public void HitObject()
    {
        if (roomID == -1)
        {
            LevelGenerator levelGen = LevelGenObjectRefGetter.Instance.gameObject.GetComponent<LevelGenerator>();

            roomID = levelGen.GetRoomIDFromCoordinates(levelGen.GetGridCoordinates(transform.position));

            doorGenerator = LevelGenObjectRefGetter.Instance.gameObject.GetComponent<DoorGenerator>();
        }

        doorGenerator.ToggleDoors(roomID);
        state = !state;
    }

    public void KickObject(Vector3 forceAndDir, ForceMode forceMode = ForceMode.Force)
    {
        if (roomID == -1)
        {
            LevelGenerator levelGen = LevelGenObjectRefGetter.Instance.gameObject.GetComponent<LevelGenerator>();

            roomID = levelGen.GetRoomIDFromCoordinates(levelGen.GetGridCoordinates(transform.position));

            doorGenerator = LevelGenObjectRefGetter.Instance.gameObject.GetComponent<DoorGenerator>();
        }

        doorGenerator.ToggleDoors(roomID);
        state = !state;
    }
}
