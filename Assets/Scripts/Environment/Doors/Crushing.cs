using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Crush entities when they are added to the stored list twice.
/// <br />This is directly tied to doors. // TODO: Remove the door link and have a separate script.
/// </summary>
public class Crushing : MonoBehaviour
{
    private List<Collider> detectedColliders = new List<Collider>();

    private bool isActive = true;

    private Door door; // TODO: REMOVE.

    void Start()
    {
        door = GetComponent<Door>(); // TODO: REMOVE.
    }

    void Update()
    {
        if (door.IsDoorOpen()) // TODO: REMOVE.
        {
            isActive = false;
        }
        else
        {
            isActive = true;
        }
    }

    // If this does not work, then replace collider with health.
    public void AddCollider(Collider other)
    {
        if (!isActive) return;


        if (detectedColliders.Contains(other))
        {
            other.GetComponent<Health>()?.InstantKill();

            detectedColliders.Remove(other);
        }
        else
        {
            detectedColliders.Add(other);
        }
    }

    public void RemoveCollider(Collider other)
    {
        if (detectedColliders.Contains(other))
        {
            detectedColliders.Remove(other);
        }
    }
}
