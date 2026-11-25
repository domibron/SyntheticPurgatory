using UnityEngine;

public class StartLocation : MonoBehaviour
{
    [SerializeField]
    private Transform playerStartSpawnLocation;

    public Transform GetSpawnLocation()
    {
        return playerStartSpawnLocation;
    }
}
