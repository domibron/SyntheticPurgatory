using UnityEngine;

/// <summary>
/// Allows the entity with the health script to regenerate health when not taking damage after some time.
/// </summary>
[RequireComponent(typeof(Health))]
public class Regeneration : MonoBehaviour
{
    /// <summary>
    /// The target health component to regenerate.
    /// </summary>
    private Health health;

    /// <summary>
    /// How much to regenerate per second.
    /// </summary>
    [SerializeField]
    private float regenerationRate = 1f; // one per second.

    /// <summary>
    /// How long to wait before starting regeneration.
    /// </summary>
    [SerializeField]
    private float regenerationDelay = 5f;

    /// <summary>
    /// The current wait time before the regeneration can kick in.
    /// </summary>
    private float currentDelay = 0f;

    void Awake()
    {
        health = GetComponent<Health>();

        health.OnHealthChanged += OnHealthChanged;
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

    /// <summary>
    /// Sets the regeneration rate and delay of this component.
    /// </summary>
    /// <param name="regenRate">How much to regenerate per second.</param>
    /// <param name="delay">The delay after taking damage before regenerating.</param>
    public void SetUpRegeneration(float regenRate, float delay)
    {
        regenerationRate = regenRate;
        regenerationDelay = delay;
    }

    /// <summary>
    /// Event listener for the on health changed. Used to reset the delay timer.
    /// </summary>
    /// <param name="newHealth">The new health value.</param>
    /// <param name="oldHealth">The old health value.</param>
    private void OnHealthChanged(float newHealth, float oldHealth)
    {
        if (newHealth < oldHealth)
        {
            // pause regeneration
            currentDelay = regenerationDelay;
        }
    }


}
