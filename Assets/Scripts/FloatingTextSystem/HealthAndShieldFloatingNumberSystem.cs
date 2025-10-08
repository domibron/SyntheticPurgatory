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

// ▄▄▄▄    ▄▄▄▄         ▄▄   ▄  ▄▄▄▄ ▄▄▄▄▄▄▄        ▄    ▄  ▄▄▄▄  ▄▄▄▄   ▄▄▄▄▄  ▄▄▄▄▄▄▄     ▄
// █   ▀▄ ▄▀  ▀▄        █▀▄  █ ▄▀  ▀▄   █           ██  ██ ▄▀  ▀▄ █   ▀▄   █    █      ▀▄ ▄▀ 
// █    █ █    █        █ █▄ █ █    █   █           █ ██ █ █    █ █    █   █    █▄▄▄▄▄  ▀█▀  
// █    █ █    █        █  █ █ █    █   █           █ ▀▀ █ █    █ █    █   █    █        █   
// █▄▄▄▀   █▄▄█         █   ██  █▄▄█    █           █    █  █▄▄█  █▄▄▄▀  ▄▄█▄▄  █        █   

public class HealthAndShieldFloatingNumberSystem : HealthFloatingNumberSystem
{

    [Header("Shield Text Settings")]

    public bool shieldUseGradient = false;

    public TMP_ColorGradient shieldTextGradient;

    public Color shieldTextColor = Color.blue;

    protected HealthWithBasicShield healthWithBasicShield;

    public string shieldBlockedText = "Blocked";

    public float shieldCharacterSpacing = 0;

    protected override void Start()
    {
        base.Start();

        healthWithBasicShield = GetComponent<HealthWithBasicShield>();

        if (healthWithBasicShield == null) Debug.LogError($"You need to add {nameof(healthWithBasicShield)} component to this object too!", this);

        healthWithBasicShield.onShieldHit += ShieldBlocked;
    }

    protected virtual void ShieldBlocked()
    {
        if (shieldUseGradient)
        {
            floatingTextSystem.SpawnText(shieldBlockedText,
                shieldTextGradient,
                textSize: (baseSize + (numberSizeMultiply * Mathf.Sqrt(5))) * UnityEngine.Random.Range(minRandomMultiplyAmount, maxRandomMultiplyAmount),
                characterSpacing: shieldCharacterSpacing);
        }
        else
        {
            floatingTextSystem.SpawnText(shieldBlockedText,
                shieldTextColor,
                textSize: (baseSize + (numberSizeMultiply * Mathf.Sqrt(5))) * UnityEngine.Random.Range(minRandomMultiplyAmount, maxRandomMultiplyAmount),
                characterSpacing: shieldCharacterSpacing);
        }
    }


}
