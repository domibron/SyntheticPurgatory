using UnityEngine;

public class Lever : MonoBehaviour, IKickable, IShootable
{
    private int roomID = -1;

    private DoorGenerator doorGenerator;

    private Animator animator;

    private bool state = false;

    private bool inMotion = false;

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
        if (inMotion) { return; }

        if (roomID == -1)
        {
            LevelGenerator levelGen = LevelGenObjectRefGetter.Instance.gameObject.GetComponent<LevelGenerator>();

            roomID = levelGen.GetRoomIDFromCoordinates(levelGen.GetGridCoordinates(transform.position));

            doorGenerator = LevelGenObjectRefGetter.Instance.gameObject.GetComponent<DoorGenerator>();
        }

        doorGenerator.ToggleDoors(roomID);
        state = !state;
        inMotion = true;
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
        inMotion = true;
    }

    public void MotionEnded()
    {
        inMotion = false;
    }
}
