using System;
using UnityEngine;
using UnityEngine.Events;

public class EntityOnDeath : MonoBehaviour
{
    public UnityEvent OnDeathEvent;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Health>().onDeath += OnDeath;
    }

    private void OnDeath()
    {
        OnDeathEvent?.Invoke();
    }
}
