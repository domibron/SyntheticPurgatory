using System;
using UnityEngine;

// By Vince Pressey

public class HitScrapDropper : MonoBehaviour
{
    ScrapDropper dropper;
    Health healthScript;

    /// <summary>
    /// Total scrap dropped before death, triggered by damage and split evenly
    /// </summary>
    public int TotalHitScrap = 3;
    /// <summary>
    /// Value saved for exhausting hit-scrap supply in case of possible healing exploit
    /// </summary>
    private int hitScrapRemaining;

    /// <summary>
    /// Force applied horizontally to scrap object (X and Z)
    /// </summary>
    [Header("Creation Force"), SerializeField]
    private float sideForce = 1.5f;
    /// <summary>
    /// Force applied vertically to scrap object (Y) 
    /// </summary>
    [SerializeField]
    private float upForce = 3f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthScript = GetComponent<Health>();
        GetComponent<Health>().onHealthChanged += ScrapDropping;
        dropper = GetComponent<ScrapDropper>();

        hitScrapRemaining = TotalHitScrap;
    }

    /// <summary>
    /// Drop scrap based on object health difference
    /// </summary>
    /// <param name="newHP">Health after taking damage</param>
    /// <param name="oldHP">Health before taking damage</param>
    private void ScrapDropping(float newHP, float oldHP)
    {
        float hitHPNotches = healthScript.ReturnMaxHealthValue() / (TotalHitScrap + 1); // Calculate health values to spawn scrap at
        int currentNotch = Mathf.FloorToInt(oldHP / hitHPNotches); // Calculate integer relative to totalHitScrap to know how many scrap is left to be spawned
        float nextNotch = currentNotch * hitHPNotches; // Get exact health value that is next to be checked

        while (newHP <= nextNotch && currentNotch > 0 && hitScrapRemaining > 0) // Spawn scrap if health difference passes hit-scrap threshold
        {
            if (nextNotch != healthScript.ReturnMaxHealthValue()) // Do not spawn scrap on first hit
            {
                dropper.SpawnScrapGroup(1, sideForce, upForce); // Spawn scrap object
                hitScrapRemaining--; // Handle exploit of repetitively healing then damaging a target, only so many can be dropped
            }

            currentNotch--; // Set up next hit-scrap threshold for next while check
            nextNotch = currentNotch * hitHPNotches;
        }

    }


}
