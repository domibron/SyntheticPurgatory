using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//by    _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|
// © 2025 domibron


public class HealthFloatingNumberSystem : MonoBehaviour
{
    [Header("Number Size Adjustments")]
    public float baseSize = 6f;
    public float numberSizeMultiply = 3f;
    public float minRandomMultiplyAmount = 0.4f;
    public float maxRandomMultiplyAmount = 0.4f;

    [Header("Health Loss Text Settings")]
    public bool healthLossUseGradient = false;

    public TMP_ColorGradient healthLossTextGradient;

    public Color healthLossTextColor = Color.red;

    public float healthLossCharacterSpacing = -30f;

    protected Health health;

    protected FloatingTextSystem floatingTextSystem;

    [Header("Health Gain Text Settings")]

    public bool healthGainUseGradient = false;

    public TMP_ColorGradient healthGainTextGradient;

    public Color healthGainTextColor = Color.green;

    public float healthGainCharacterSpacing = -30f;


    protected virtual void Start()
    {
        health = GetComponent<Health>();

        if (health == null) Debug.LogError($"You need a {typeof(Health)} component!", this);

        floatingTextSystem = GetComponent<FloatingTextSystem>();

        if (floatingTextSystem == null) Debug.LogError($"Need a {nameof(floatingTextSystem)} component to also be added!", this);

        health.onAddToHealth += AddToHealth;
    }

    protected virtual void AddToHealth(float amount) // TODO: redo for readability. Remove magic numbers
    {
        if (amount == 0) return; // we dont want 0 on screen.

        if (amount < 0) // did we take damage.
        {
            if (healthLossUseGradient)
            {
                floatingTextSystem.SpawnText(Mathf.Abs(amount).ToString("F0"),
                    healthLossTextGradient,
                    textSize: (baseSize + (numberSizeMultiply * Mathf.Sqrt(5))) * UnityEngine.Random.Range(minRandomMultiplyAmount, maxRandomMultiplyAmount),
                    characterSpacing: healthLossCharacterSpacing);
            }
            else
            {
                floatingTextSystem.SpawnText(Mathf.Abs(amount).ToString("F0"),
                    healthLossTextColor,
                    textSize: (baseSize + (numberSizeMultiply * Mathf.Sqrt(5))) * UnityEngine.Random.Range(minRandomMultiplyAmount, maxRandomMultiplyAmount),
                    characterSpacing: healthLossCharacterSpacing);
            }
        }
        else // we gotten heald.
        {
            if (healthGainUseGradient)
            {
                floatingTextSystem.SpawnText(Mathf.Abs(amount).ToString("F0"),
                    healthGainTextGradient,
                    textSize: (baseSize + (numberSizeMultiply * Mathf.Sqrt(5))) * UnityEngine.Random.Range(minRandomMultiplyAmount, maxRandomMultiplyAmount),
                    characterSpacing: healthGainCharacterSpacing);
            }
            else
            {
                floatingTextSystem.SpawnText(Mathf.Abs(amount).ToString("F0"),
                    healthGainTextColor,
                    textSize: (baseSize + (numberSizeMultiply * Mathf.Sqrt(5))) * UnityEngine.Random.Range(minRandomMultiplyAmount, maxRandomMultiplyAmount),
                    characterSpacing: healthGainCharacterSpacing);
            }
        }

    }
}
