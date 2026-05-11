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

    private Door door; // TODO: Move to its own script.

    void Start()
    {
        door = GetComponent<Door>(); // TODO: Move to its own script.
    }

    void Update()
    {
        if (door.IsDoorOpen()) // TODO: Move to its own script.
        {
            isActive = false;
        }
        else
        {
            isActive = true;
        }
    }

    // If this does not work, then replace collider with health.
    /// <summary>
    /// Adds the collider to the list, if it already exists then it will get the health component and call InstantKill().
    /// </summary>
    /// <param name="other">The collider to add.</param>
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

    /// <summary>
    /// Removes the collider from the list if it exists.
    /// </summary>
    /// <param name="other">The collider to remove.</param>
    public void RemoveCollider(Collider other)
    {
        if (detectedColliders.Contains(other))
        {
            detectedColliders.Remove(other);
        }
    }
}
