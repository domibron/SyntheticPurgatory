using System;
using UnityEngine;

/// <summary>
/// Allows a physics object to be kicked by the player.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class KickableObject : MonoBehaviour, IKickable
{
    private Rigidbody rb;

    public event Action<Vector3> OnKicked;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void IKickable.KickObject(Vector3 forceAndDir, ForceMode forceMode)
    {
        Vector3 alteredForceDir;

        alteredForceDir = new Vector3(forceAndDir.x / (rb.mass / 2), Mathf.Max(forceAndDir.y, 5), forceAndDir.z / (rb.mass / 2));
        rb.AddForce(alteredForceDir, forceMode);
        OnKicked?.Invoke(alteredForceDir);
    }
}
