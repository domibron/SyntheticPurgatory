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
        // Alter given force to have forced upward direction and to account for the mass of the object
        Vector3 alteredForceDir = new Vector3(forceAndDir.x / (rb.mass / 2), Mathf.Max(forceAndDir.y, 5), forceAndDir.z / (rb.mass / 2)) ;
        rb.AddForce(alteredForceDir, forceMode);
    }
}
