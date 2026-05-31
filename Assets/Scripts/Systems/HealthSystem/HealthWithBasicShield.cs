using System;
using UnityEngine;

/// <summary>
/// Health with a basic shield that needs to be broken before damage can be dealt.
/// </summary>
public class HealthWithBasicShield : Health
{
	/// <summary>
	/// The shield object attached to the entity.
	/// </summary>
	[Header("Shield Settings")]
	[SerializeField]
	protected GameObject shieldObject;

	/// <summary>
	/// Should the shield be activated on start.
	/// </summary>
	public bool shieldActiveOnStart = true;

	/// <summary>
	/// Is the shield active.
	/// </summary>
	[HideInInspector]
	public bool shieldActive = true;

	// protected ShieldHitIndicator shieldHitIndicator;

	/// <summary>
	/// Called when the shield blocks the hit.
	/// </summary>
	public event Action onShieldHit;

	/// <summary>
	/// Called when the shield is broken.
	/// </summary>
	public event Action onShieldBreak;

	/// <summary>
	/// Called when the shield is activated.
	/// </summary>
	public event Action onShieldActivate;

	/// <summary>
	/// Get whether the shield is active or not.
	/// </summary>
	public bool IsShieldActive { get => shieldActive; }

	protected override void Start()
	{

		if (!shieldActiveOnStart)
		{
			currentHealth = maxHealth;
			calledOnDeathEvent = false;

			shieldActive = false;
			shieldObject.SetActive(false);
		}
		else
		{
			Reset();
		}
	}

	/// <summary>
	/// Resets both the health and the shield as well as the on death event.
	/// </summary>
	public override void Reset()
	{
		base.Reset();

		ActivateShield();
	}

	/// <summary>
	/// Break the shield.
	/// </summary>
	public virtual void BreakShield()
	{
		shieldActive = false;

		shieldObject.SetActive(false);

		InvokeOnShieldBreak();
	}

	/// <summary>
	/// Add to the health but block negative values if the shield is still up.
	/// </summary>
	/// <param name="amount">The amount to add.</param>
	public override void AddToHealth(float amount)
	{
		if (shieldActive && amount < 0)
		{
			InvokeOnShieldHit();
			return;
		}

		base.AddToHealth(amount);
	}


	/// <summary>
	/// Activates the shield.
	/// </summary>
	public virtual void ActivateShield()
	{
		shieldActive = true;
		shieldObject.SetActive(true);
		InvokeOnShieldActivate();
	}


	protected void InvokeOnShieldHit()
	{
		onShieldHit?.Invoke();
	}

	protected void InvokeOnShieldBreak()
	{
		onShieldBreak?.Invoke();
	}

	protected void InvokeOnShieldActivate()
	{
		onShieldActivate?.Invoke();
	}

}
