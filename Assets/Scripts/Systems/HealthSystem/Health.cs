using System;
using UnityEngine;

/// <summary>
/// Contains the update of heath and what was the cause.
/// </summary>
public struct HealthChangedContext
{
    /// <summary>
    /// The damage context contains the cause.
    /// </summary>
    public DamageContext? DamageContext;

    /// <summary>
    /// What this entity is immune to.
    /// </summary>
    public DamageTypes Immunities;


    /// <summary>
    /// The current health the entity sits at after the change.
    /// </summary>
    public float CurrentHealth;

    /// <summary>
    /// The previous health before this update.
    /// </summary>
    public float PreviousHealth;

    /// <summary>
    /// The total HP this entity has.
    /// </summary>
    public float MaxHealth;


    /// <summary>
    /// Creates a new health context.
    /// </summary>
    /// <param name="currentHealth">The current health of this entity after the change.</param>
    /// <param name="previousHealth">The health before the change.</param>
    /// <param name="maxHealth">The total health this entity has.</param>
    /// <param name="immunities">What types is the entity immune to.</param>
    /// <param name="damageContext">The cause of the change.</param>
    public HealthChangedContext(float currentHealth, float previousHealth, float maxHealth, DamageTypes immunities = DamageTypes.None, DamageContext? damageContext = null)
    {
        CurrentHealth = currentHealth;
        PreviousHealth = previousHealth;
        MaxHealth = maxHealth;

        Immunities = immunities;

        DamageContext = damageContext;
    }
}

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
    protected DamageTypes immunities = DamageTypes.None;

    /// <summary>
    /// Used to only trigger the on death event once.
    /// </summary>
    protected bool calledOnDeathEvent = false;

    /// <summary>
    /// Called once when the entity has no more health left.
    /// </summary>
    public event Action OnDeath;

    /// <summary>
    /// Called when adding to the health, the provided float is what to add to the health. new, old.
    /// </summary>
    [Obsolete("Please use OnHealthUpdated!", true)]
    public event OnValueChangedDelegate HealthChanged;

    /// <summary>
    /// A delegate for when a value was changed sending both the original and updated values.
    /// </summary>
    /// <param name="newValue">The new value after the change.</param>
    /// <param name="oldValue">The original value before the change.</param>
    [Obsolete("Please use OnHealthUpdatedDelegate!", true)]
    public delegate void OnValueChangedDelegate(float newValue, float oldValue);

    /// <summary>
    /// Called when the health was changed in some way.
    /// </summary>
    public event OnHealthUpdatedDelegate HealthUpdated;

    /// <summary>
    /// A delegate for when the health is updated giving a <see cref="HealthChangedContext"/> about the change and what caused it.
    /// </summary>
    /// <param name="context">The change and cause.</param>
    public delegate void OnHealthUpdatedDelegate(HealthChangedContext context);

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
    /// Adds to the health directly skipping the damage context.
    /// </summary>
    /// <param name="amount">The value to add to the health.</param>
    public virtual void AddToHealth(float amount)
    {
        if (amount == 0) return;

        InvokeOnHealthUpdated(new HealthChangedContext(currentHealth + amount, currentHealth, maxHealth));
        currentHealth += amount;


        if (currentHealth > maxHealth) currentHealth = maxHealth;

        if (currentHealth <= 0)
        {
            CallOnDeathEvent();
        }
    }

    public virtual void UpdateHealth(DamageContext context)
    {
        if ((immunities & context.Type) != 0)
        {
            InvokeOnHealthUpdated(new HealthChangedContext(currentHealth, currentHealth, maxHealth, immunities, context));
            return;
        }

        float damage = context.Damage * (context.IsCritical ? 2f : 1f);

        if (damage != 0)
        {
            InvokeOnHealthUpdated(new HealthChangedContext(currentHealth - damage, currentHealth, maxHealth, immunities, context));
        }

        currentHealth -= damage;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }


        if (currentHealth <= 0)
        {
            CallOnDeathEvent();
        }
    }

    /// <summary>
    /// Set the objects health to 0 and invoke death.
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
        return (immunities & DamageTypes.Toxic) != 0;
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
    [Obsolete("Please use InvokeOnHealthUpdated instead!", false)]
    protected void InvokeOnHealthChanged(float newAmount, float oldHealth)
    {
        HealthChanged?.Invoke(newAmount, oldHealth);
    }

    /// <summary>
    /// Calls the onDeath event.
    /// </summary>
    protected void InvokeOnDeath()
    {
        OnDeath?.Invoke();
    }


    protected void InvokeOnHealthUpdated(HealthChangedContext context)
    {
        HealthUpdated?.Invoke(context);
    }
}
