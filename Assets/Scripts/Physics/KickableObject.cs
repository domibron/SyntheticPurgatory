using UnityEngine;

public class KickableObject : MonoBehaviour, IKickable
{
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void IKickable.KickObject(Vector3 forceAndDir, ForceMode forceMode)
    {
        rb.AddForce(forceAndDir, forceMode);
    }
}
