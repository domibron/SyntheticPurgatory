using UnityEngine;

public class KillZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        other.GetComponent<Health>()?.AddToHealth(-999999f);
    }

    void OnTriggerStay(Collider other)
    {
        other.GetComponent<Health>()?.AddToHealth(-999999f);
    }
}
