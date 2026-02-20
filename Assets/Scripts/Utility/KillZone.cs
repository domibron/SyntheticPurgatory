using UnityEngine;

public class KillZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        other.GetComponent<Health>()?.InstantKill();
    }

    void OnTriggerStay(Collider other)
    {
        other.GetComponent<Health>()?.InstantKill();
    }
}
