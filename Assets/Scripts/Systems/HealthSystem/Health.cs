using System;
using UnityEngine;


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
    /// If entity can take damage from toxic sources
    /// </summary>
    [SerializeField]
    protected bool toxicImmunity = false;

    /// <summary>
    /// Used to only trigger the on death event once.
    /// </summary>
    protected bool calledOnDeathEvent = false;

    /// <summary>
    /// Called once when the entity has no more health left.
    /// </summary>
    public event Action onDeath;

    /// <summary>
    /// Called when adding to the health, the provided float is what to add to the health. new, old.
    /// </summary>
    public event OnValueChangedDelegate OnHealthChanged;

    /// <summary>
    /// A delegate for when a value was changed sending both the original and updated values.
    /// </summary>
    /// <param name="newValue">The new value after the change.</param>
    /// <param name="oldValue">The original value before the change.</param>
    public delegate void OnValueChangedDelegate(float newValue, float oldValue);

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
    /// Use this to add to or remove from the health. new, old
    /// </summary>
    /// <param name="amount">The value to add to the health.</param>
    public virtual void AddToHealth(float amount)
    {

        if (amount != 0) InvokeOnHealthChanged(currentHealth + amount, currentHealth);
        currentHealth += amount;


        if (currentHealth > maxHealth) currentHealth = maxHealth;

        if (currentHealth <= 0)
        {
            CallOnDeathEvent();
        }
    }

    /// <summary>
    /// Set the objects health to 0 and invoke death
    /// </summary>
    public virtual void InstantKill()
    {
        currentHealth = 0;

        CallOnDeathEvent();
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
    public virtual float GetHealthValue()
    {
        return currentHealth;
    }

    /// <summary>
    /// Returns the max health value.
    /// </summary>
    /// <returns>The max health value.</returns>
    public virtual float GetMaxHealthValue()
    {
        return maxHealth;
    }

    /// <summary>
    /// Get toxic immunity
    /// </summary>
    /// <returns>Boolean</returns>
    public virtual bool GetToxicImmunity()
    {
        return toxicImmunity;
    }

    /// <summary>
    /// Calls the on death event if it was not called.
    /// </summary>
    protected virtual void CallOnDeathEvent()
    {
        if (!calledOnDeathEvent)
        {
            calledOnDeathEvent = true; // prevent spamming the event.
            InvokeOnDeath();
        }
    }

    /// <summary>
    /// Set the current max health.
    /// </summary>
    /// <param name="value">The new max health.</param>
    /// <param name="setCurrentHealth">Set the current health too.</param>
    public void SetMaxHealth(float value, bool setCurrentHealth = true)
    {
        maxHealth = value;

        if (setCurrentHealth)
            currentHealth = maxHealth;
    }

    /// <summary>
    /// Calls the onAddToHealth event.
    /// </summary>
    /// <param name="amount">The amount to add to the current health value.</param>
    protected void InvokeOnHealthChanged(float newAmount, float oldHealth)
    {
        OnHealthChanged?.Invoke(newAmount, oldHealth);
    }

    /// <summary>
    /// Calls the onDeath event.
    /// </summary>
    protected void InvokeOnDeath()
    {
        onDeath?.Invoke();
    }

}
