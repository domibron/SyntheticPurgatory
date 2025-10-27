using UnityEngine;

public class Lever : MonoBehaviour, IKickable
{
    private int roomID = -1;

    private DoorGenerator doorGenerator;

    public void KickObject(Vector3 forceAndDir, ForceMode forceMode = ForceMode.Force)
    {
        if (roomID == -1)
        {
            LevelGenerator levelGen = LevelGenObjectRefGetter.Instance.gameObject.GetComponent<LevelGenerator>();

            roomID = levelGen.GetRoomIDFromCoordinates(levelGen.GetGridCoordinates(transform.position));

            doorGenerator = LevelGenObjectRefGetter.Instance.gameObject.GetComponent<DoorGenerator>();
        }

        doorGenerator.ToggleDoors(roomID);
    }
}
