using UnityEngine;

public class CrushingDetector : MonoBehaviour
{
    [SerializeField]
    private Crushing crushing;

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Health>() != null)
        {
            crushing.AddCollider(other);
        }
        else if (other.GetComponentInParent<Health>() != null && other.GetComponentInParent<Collider>() != null)
        {
            crushing.AddCollider(other.GetComponentInParent<Collider>());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Health>() != null)
        {
            crushing.RemoveCollider(other);
        }
        else if (other.GetComponentInParent<Health>() != null && other.GetComponentInParent<Collider>() != null)
        {
            crushing.RemoveCollider(other.GetComponentInParent<Collider>());
        }
    }
}
