using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// Deals damage to any object inside the trigger zone.
/// </summary>
public class DamagingArea : MonoBehaviour
{
    /// <summary>
    /// The time between damage ticks.
    /// </summary>
    [SerializeField]
    private float tickTime = 0.2f;

    /// <summary>
    /// Damage per tick.
    /// </summary>
    [SerializeField]
    private float damagePerTick = 1;

    /// <summary>
    /// Collection of object that are inside the damage area, can contain null elements.
    /// </summary>
    private List<Health> objectsInside = new List<Health>();


    void Start()
    {
        // Ideally use nameof in case of function name change.
        // Also allows for easier tracking using an IDE since it links the function directly.
        // Means you can F12 it can it will take you to the actual function compared to a string.
        InvokeRepeating(nameof(DamageTick), 0, tickTime);

        // Also a note, I was going to do a update with a timer but left it like this because
        // I can always come back and add it. In short, Invoke repeating will have the set tick rate
        // and cannot change after the fact compared to a timer based system, or a coroutine, or async.
    }

    /// <summary>
    /// Deals damage to all elements in <see cref="objectsInside"/> collection.
    /// </summary>
    private void DamageTick()
    {
        // Fixed a bug when iterating over a list and modifying that list.
        List<Health> objectsToDamage = objectsInside;

        foreach (Health damageableObject in objectsToDamage)
        {
            // if null remove.
            if (!damageableObject)
            {
                objectsInside.Remove(damageableObject);
                continue;
            }

            damageableObject.AddToHealth(-damagePerTick);
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Health>())
        {
            objectsInside.Add(other.GetComponent<Health>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Health>())
        {
            objectsInside.Remove(other.GetComponent<Health>());
        }
    }
}
