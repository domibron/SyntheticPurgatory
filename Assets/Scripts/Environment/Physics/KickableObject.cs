using System;
using UnityEngine;

/// <summary>
/// Allows a physics object to be kicked by the player.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class KickableObject : MonoBehaviour, IKickable
{
    private Rigidbody rb;

    [SerializeField]
    private bool disableNavAgent;

    [SerializeField]
    private BaseEnemy enemyClass; // TODO: remove. Use IKickable on the enemy instead.

    public event Action<Vector3> OnKicked;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void IKickable.KickObject(Vector3 forceAndDir, ForceMode forceMode)
    {
        Vector3 alteredForceDir;

        if (disableNavAgent)
        {
            enemyClass.KnockbackAI(0.3f, true); // TODO: Move into the AI system itself, this class is only for rigidbodies not AI.

            // Alter given force to have forced upward direction and to account for the mass of the object
            alteredForceDir = new Vector3(forceAndDir.x * 5 / (rb.mass / 2), Mathf.Max(forceAndDir.y, 5), forceAndDir.z * 5 / (rb.mass / 2));
            rb.AddForce(alteredForceDir, forceMode);
            OnKicked?.Invoke(alteredForceDir);

            return;
        }

        alteredForceDir = new Vector3(forceAndDir.x / (rb.mass / 2), Mathf.Max(forceAndDir.y, 5), forceAndDir.z / (rb.mass / 2));
        rb.AddForce(alteredForceDir, forceMode);
        OnKicked?.Invoke(alteredForceDir);
    }
}
