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

/// <summary>
/// Auto shield this should be renamed to, used on enemies.
/// </summary>
public class HealthWithShield : HealthWithBasicShield
{
    [SerializeField]
    private float coolDownTime = 10f;
    private float currentCoolDownTime = 0;

    private bool resetShield = false;


    void Update()
    {
        if (currentCoolDownTime > 0) currentCoolDownTime -= Time.deltaTime;
        else if (currentCoolDownTime < 0 && resetShield) ActivateShield();

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

    public override void ActivateShield()
    {
        //currentShieldHealth = maxShieldHealth;
        resetShield = false;
        base.ActivateShield();
    }

    public override void BreakShield()
    {
        ShieldDeactivate();
        base.BreakShield();
    }


    protected void ShieldDeactivate()
    {
        currentCoolDownTime = coolDownTime;
        resetShield = true;
    }



}
