using UnityEngine;

/// <summary>
/// Shield with a automatic restoring shield.
/// </summary>
public class HealthWithShield : HealthWithBasicShield
{
    /// <summary>
    /// The time to wait before reactivating the shield.
    /// </summary>
    [SerializeField]
    private float coolDownTime = 10f;

    /// <summary>
    /// The current time for the shield cool down.
    /// </summary>
    private float currentCoolDownTime = 0;

    /// <summary>
    /// Should we be resetting the shield.
    /// </summary>
    private bool resetShield = false;


    void Update()
    {
        // shield reset.
        if (currentCoolDownTime > 0) currentCoolDownTime -= Time.deltaTime;
        else if (currentCoolDownTime < 0 && resetShield) ActivateShield();
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


    /// <summary>
    /// Start the shield reset timer.
    /// </summary>
    protected void ShieldDeactivate()
    {
        currentCoolDownTime = coolDownTime;
        resetShield = true;
    }



}
