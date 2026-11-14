using System;
using UnityEngine;
using UnityEngine.VFX;

public class SpawnParticlesOnHit : MonoBehaviour
{
    [SerializeField]
    VisualEffect particlesEffect;

    [SerializeField]
    Health health;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (health == null) health = GetComponent<Health>();
        health.HealthUpdated += OnHealthChanged;
    }

    private void OnHealthChanged(HealthChangedContext context)
    {
        if (context.CurrentHealth < context.PreviousHealth)
        {
            SpawnParticles();
        }
    }

    private void SpawnParticles()
    {
        particlesEffect.SendEvent("PlayEvent");
    }
}
