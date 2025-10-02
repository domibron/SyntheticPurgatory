using System;
using System.Collections;
using UnityEngine;


//by    _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|
// © 2025 domibron


/// <summary>
/// The health class to give objects health.
/// </summary>
public class Health : MonoBehaviour
{
    /// <summary>
    /// The max health of the entity.
    /// </summary>
    [Header("Health Settings")]
    [SerializeField]
    protected float maxHealth = 100;

    /// <summary>
    /// The current health of the entity.
    /// </summary>
    protected float currentHealth;

    /// <summary>
    /// Used to only trigger the on death event once.
    /// </summary>
    protected bool calledOnDeathEvent = false;

    /// <summary>
    /// Called once when the entity has no more health left.
    /// </summary>
    public event Action onDeath;

    /// <summary>
    /// Called when taking damage, provided float is how much damage to take (its positive).
    /// </summary>
    public event Action<float> onTakeDamage;

    /// <summary>
    /// Called when adding to the health, the provided float is what to add to the health.
    /// </summary>
    public event Action<float> onAddToHealth;

    protected virtual void Start()
    {
        Reset();
    }

    /// <summary>
    /// Resets the health and on death event. Used for re-spawning.
    /// </summary>
    public virtual void Reset()
    {
        currentHealth = maxHealth;
        calledOnDeathEvent = false;
    }

    /// <summary>
    /// Use this to add to or remove from the health.
    /// </summary>
    /// <param name="amount">The value to add to the health.</param>
    public virtual void AddToHealth(float amount)
    {
        currentHealth += amount;

        if (amount > 0) InvokeOnAddToHealth(amount);
        else if (amount < 0) InvokeOnTakeDamage(amount);


        if (currentHealth > maxHealth) currentHealth = maxHealth;

        if (currentHealth <= 0 && !calledOnDeathEvent)
        {
            calledOnDeathEvent = true; // prevent spamming the event.
            InvokeOnDeath();
        }
    }

    /// <summary>
    /// Gets a normalized version of the health aka as a percentage from 0 to 1.
    /// </summary>
    /// <returns>The percentage from 0 to 1.</returns>
    public virtual float GetHealthNormalized()
    {
        return currentHealth / maxHealth;
    }

    /// <summary>
    /// Returns the current health value.
    /// </summary>
    /// <returns>The health value.</returns>
    public virtual float ReturnHealthValue()
    {
        return currentHealth;
    }

    // ! These functions exist as you cannot call these events when inheriting.

    /// <summary>
    /// Calls the onTakeDamage event.
    /// </summary>
    /// <param name="amount">The amount of damage to take (positive number).</param>
    protected void InvokeOnTakeDamage(float amount)
    {
        onTakeDamage?.Invoke(amount);
    }

    /// <summary>
    /// Calls the onAddToHealth event.
    /// </summary>
    /// <param name="amount">The amount to add to the current health value.</param>
    protected void InvokeOnAddToHealth(float amount)
    {
        onAddToHealth?.Invoke(amount);
    }

    /// <summary>
    /// Calls the onDeath event.
    /// </summary>
    protected void InvokeOnDeath()
    {
        onDeath?.Invoke();
    }
}
