using UnityEngine;

public class CrushingDetector : MonoBehaviour
{
    [SerializeField]
    private Crushing crushing;

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Health>() != null)
            crushing.AddCollider(other);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Health>() != null)
            crushing.RemoveCollider(other);
    }
}
