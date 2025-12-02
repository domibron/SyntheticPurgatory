using System;
using UnityEngine;


public class SpawnObjectOnDeath : SpawnObjectAndSetSize
{
    [SerializeField]
    private Health onDeathTarget;

    [SerializeField]
    private bool ensureOnlyOneActivation = true;

    [SerializeField]
    private bool addForce = false;
    [SerializeField]
    private Transform forceOrigin;

    void Awake()
    {
        if (onDeathTarget == null)
            onDeathTarget = GetComponent<Health>();

        onDeathTarget.onDeath += OnDeath;

        if (forceOrigin == null)
            forceOrigin = transform;
    }

    private void OnDeath()
    {
        SpawnObject(forceOrigin.position, addForce);

        if (ensureOnlyOneActivation)
            this.enabled = false;
    }
}
