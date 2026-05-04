using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//by    _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|
// © 2025 domibron

// ▄▄▄▄    ▄▄▄▄         ▄▄   ▄  ▄▄▄▄ ▄▄▄▄▄▄▄        ▄    ▄  ▄▄▄▄  ▄▄▄▄   ▄▄▄▄▄  ▄▄▄▄▄▄▄     ▄
// █   ▀▄ ▄▀  ▀▄        █▀▄  █ ▄▀  ▀▄   █           ██  ██ ▄▀  ▀▄ █   ▀▄   █    █      ▀▄ ▄▀ 
// █    █ █    █        █ █▄ █ █    █   █           █ ██ █ █    █ █    █   █    █▄▄▄▄▄  ▀█▀  
// █    █ █    █        █  █ █ █    █   █           █ ▀▀ █ █    █ █    █   █    █        █   
// █▄▄▄▀   █▄▄█         █   ██  █▄▄█    █           █    █  █▄▄█  █▄▄▄▀  ▄▄█▄▄  █        █   

public class HealthWithBasicShield : Health
{
	[Header("Shield Settings")]
	[SerializeField]
	protected GameObject shieldObject;

	public bool shieldActiveOnStart = true;

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

	public virtual void BreakShield()
	{
		shieldActive = false;

		shieldObject.SetActive(false);

		InvokeOnShieldBreak();
	}

	public override void AddToHealth(float amount)
	{
		if (shieldActive && amount < 0)
		{
			InvokeOnShieldHit();
			return;
		}

		base.AddToHealth(amount);
	}


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
