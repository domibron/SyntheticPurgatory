using UnityEngine;

public class StartLocation : MonoBehaviour
{
    [SerializeField]
    private Transform playerStartSpawnLocation;

    public Vector3 GetSpawnLocation()
    {
        return playerStartSpawnLocation.position;
    }
}
