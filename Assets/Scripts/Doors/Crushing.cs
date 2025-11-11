using System.Collections.Generic;
using UnityEngine;

public class Crushing : MonoBehaviour
{
    private List<Collider> detectedColliders = new List<Collider>();

    private float damage = 99999f;

    private bool isActive = true;

    private Door door;

    void Start()
    {
        door = GetComponent<Door>();
    }

    void Update()
    {
        if (door.IsDoorOpen())
        {
            isActive = false;
        }
        else
        {
            isActive = true;
        }
    }

    public void AddCollider(Collider other)
    {
        if (!isActive) return;

        print(other.name);
        if (detectedColliders.Contains(other))
        {
            print("ATGQEAAEGA");
            other.GetComponent<Health>()?.AddToHealth(-damage);
            detectedColliders.Remove(other);
        }
        else
        {
            detectedColliders.Add(other);
        }
    }

    public void RemoveCollider(Collider other)
    {
        print("KILL ME");
        if (detectedColliders.Contains(other))
        {
            detectedColliders.Remove(other);
        }
    }
}
