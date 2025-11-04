using UnityEngine;
using System.Collections.Generic;

public class DamagingArea : MonoBehaviour
{
    [SerializeField] 
    private float tickTime = 0.2f;
    [SerializeField]
    private float damagePerTick = 1;

    private List<Health> objectsInside = new List<Health>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("DamageTick", 2, tickTime);
    }

    private void DamageTick()
    {
        foreach (Health damagableObject in objectsInside) 
        {
            try
            {
                damagableObject.AddToHealth(-damagePerTick);
            }
            catch
            {
                objectsInside.Remove(damagableObject);
            }

        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Health>())
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
