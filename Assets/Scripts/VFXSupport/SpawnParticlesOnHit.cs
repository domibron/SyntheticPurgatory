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
        health.onHealthChanged += OnHealthChanged;
    }

    private void OnHealthChanged(float newHealth, float oldHealth)
    {
        if (newHealth < oldHealth)
        {
            SpawnParticles();
        }
    }

    private void SpawnParticles()
    {
        particlesEffect.SendEvent("PlayEvent");
    }
}
