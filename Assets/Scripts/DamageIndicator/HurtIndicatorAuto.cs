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


/// <summary>
/// Handles making the entity flash when taking damage. Requires health script to function.
/// </summary>
public class HurtIndicatorAuto : MonoBehaviour
{
    /// <summary>
    /// The name of the material / shader graph for making the entity flash.
    /// </summary>
    [SerializeField]
    private string materialName = "HurtColor";

    /// <summary>
    /// Stores all the materials that need to be edited to make the entity flash.
    /// </summary>
    private List<Material> allMaterials = new List<Material>();

    /// <summary>
    /// How opaque the flash will be.
    /// </summary>
    [SerializeField]
    protected float opacity = 150f;

    /// <summary>
    /// How long to flash will last.
    /// </summary>
    [SerializeField]
    protected float hurtDuration = 0.2f;

    /// <summary>
    /// Local variable to store the current time for the flash.
    /// </summary>
    protected float damageTimer = 0;


    void Start()
    {
        // Store all found into an array.
        Renderer[] allRenderers = transform.GetComponentsInChildren<Renderer>();

        // Go through all of the renderers and their materials.
        foreach (var renderer in allRenderers)
        {
            foreach (var material in renderer.materials)
            {
                // add the material to the collection if it matches the shader name.
                if (!material.name.Contains(materialName)) continue;
                allMaterials.Add(material);
            }
        }

        // subscript to the take damage health event to flash when taking damage.
        GetComponent<Health>().onHealthChanged += TakenDamage;
    }

    // Update is called once per frame
    void Update()
    {
        // handles the timer ticking.
        if (damageTimer > 0) damageTimer -= Time.deltaTime;

        // this makes the color fade in and out when using just time.
        float blend = Mathf.Sin((damageTimer / hurtDuration) * Mathf.PI);

        // handles the fading to apply on the materials.
        float alpha = damageTimer > 0 ? Mathf.Lerp(0, opacity / 255f, blend) : 0;


        // cycle through all the materials in the collection.
        foreach (var material in allMaterials)
        {
            // if the alpha is the same, we dont need to do anything.
            if (material.color.a == alpha) continue;

            // override the alpha with the new one.
            material.color = new Color(material.color.r, material.color.g, material.color.b, alpha);

        }
    }


    /// <summary>
    /// Function to hook into onTakeDamage event on health scripts.
    /// </summary>
    /// <param name="amount">The amount of damage to take.</param>
    protected virtual void TakenDamage(float newHealth, float oldHealth)
    {
        float amount = newHealth - oldHealth;
        if (amount < 0)
            Flash();
    }

    /// <summary>
    /// Triggers the damage flash.
    /// </summary>
    public virtual void Flash()
    {
        damageTimer = hurtDuration;
    }

}
