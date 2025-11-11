using System;
using UnityEngine;

public class Regeneration : MonoBehaviour
{
    private Health health;

    [SerializeField]
    private float regenerationRate = 1f; // one per second.

    [SerializeField]
    private float regnerationDelay = 5f;

    private float currentDelay = 0f;

    void Awake()
    {
        health = GetComponent<Health>();

        health.onHealthChanged += OnHealthChanged;
    }

    public void SetUpRegneration(float regenRate, float delay)
    {
        regenerationRate = regenRate;
        regnerationDelay = delay;
    }

    private void OnHealthChanged(float newHealth, float oldHealth)
    {
        if (newHealth < oldHealth)
        {
            // pause regeneration
            currentDelay = regnerationDelay;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (currentDelay > 0f) currentDelay -= Time.deltaTime;

        if (currentDelay <= 0f && health.GetHealthNormalized() < 1f)
        {
            health.AddToHealth(regenerationRate * Time.deltaTime);
        }
    }
}
