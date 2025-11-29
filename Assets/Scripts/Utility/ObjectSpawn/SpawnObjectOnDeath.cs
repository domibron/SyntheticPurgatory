using System;
using UnityEngine;


public class SpawnObjectOnDeath : SpawnObjectAndSetSize
{
    [SerializeField]
    private Health onDeathTarget;

    [SerializeField]
    private bool ensureOnlyOneActivation = true;

    void Awake()
    {
        if (onDeathTarget == null)
            onDeathTarget = GetComponent<Health>();

        onDeathTarget.onDeath += OnDeath;
    }

    private void OnDeath()
    {
        SpawnObject();

        if (ensureOnlyOneActivation)
            this.enabled = false;
    }
}
