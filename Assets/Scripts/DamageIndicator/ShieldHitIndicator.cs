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
/// Handles making the shield flash when hit. Requires health with basic shield or just health with shield script to function.
/// </summary>
public class ShieldHitIndicator : MonoBehaviour
{
    /// <summary>
    /// The shield material / shader name.
    /// </summary>
    [SerializeField]
    private string shieldMaterialName = "Shield";

    /// <summary>
    /// The material for the shield.
    /// </summary>
    private Material shieldMaterial;

    /// <summary>
    /// How opaque the shield will be peak flash.
    /// </summary>
    [SerializeField]
    private float opacity = 210f;

    /// <summary>
    /// How long the flash will last.
    /// </summary>
    [SerializeField]
    private float flashDuration = 0.2f;

    /// <summary>
    /// Local variable to store the current time for the flash.
    /// </summary>
    private float damageTimer = 0;

    /// <summary>
    /// The stored alpha of the shield to return back to default and to lerp with.
    /// </summary>
    private float savedAlpha;

    void Start()
    {

        // get all the renderers
        Renderer[] allRenderers = transform.GetComponentsInChildren<Renderer>();

        // look for the shield material.
        foreach (var renderer in allRenderers)
        {
            foreach (var material in renderer.materials)
            {
                // print(material.name + " is on " + transform.name);
                if (!material.name.Contains(shieldMaterialName)) continue;

                shieldMaterial = material;
            }
        }

        // store the alpha.
        savedAlpha = shieldMaterial.GetColor("_Color").a;

        // subscribe to shield hit event.
        GetComponent<HealthWithBasicShield>().onShieldHit += ShieldHit;
    }

    // Update is called once per frame
    void Update()
    {
        // handles the timer.
        if (damageTimer > 0) damageTimer -= Time.deltaTime;

        // handles the flash math.
        float blend = Mathf.Sin((damageTimer / flashDuration) * Mathf.PI);

        // handles the flash alpha using the flash math.
        float alpha = damageTimer > 0 ? Mathf.Lerp(savedAlpha, opacity / 255f, blend) : savedAlpha;


        // sets the alpha to the shield if its not the same.
        if (shieldMaterial.color.a != alpha) shieldMaterial.SetColor("_Color", new Color(shieldMaterial.color.r, shieldMaterial.color.g, shieldMaterial.color.b, alpha));

    }

    /// <summary>
    /// Triggers the shield flash.
    /// </summary>
    public void ShieldHit()
    {
        damageTimer = flashDuration;
    }
}
