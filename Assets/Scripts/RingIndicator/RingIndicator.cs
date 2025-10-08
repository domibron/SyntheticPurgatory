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


public class RingIndicator : MonoBehaviour
{
    public GameObject damageRingGameObject;

    private float ringGrowTime = 0;

    private float currentRingGrowTime = 0;

    private float endRingDiameter = 1;
    private float startRingDiameter = 0;


    private float shrinkTime = 1f;

    private float currentShrinkTime = 1f;

    /// <summary>
    /// 2 is 2x speed. 4 is 4x speed.
    /// </summary>
    private float shrinkRate = 1f;

    private bool showRing = false;

    void Update()
    {
        if (currentRingGrowTime < ringGrowTime)
            currentRingGrowTime += Time.deltaTime;

        if (currentShrinkTime > 0)
            currentShrinkTime -= Time.deltaTime * shrinkRate;

        damageRingGameObject.SetActive(showRing || (currentShrinkTime / shrinkTime) > 0);

        // stop divide by zero error.
        if (showRing)
        {
            damageRingGameObject.transform.localScale = Vector3.LerpUnclamped(Vector3.one * startRingDiameter, Vector3.one * endRingDiameter, EasingFunctions.EaseOutBack(currentRingGrowTime / ringGrowTime));
        }
        else if ((currentShrinkTime / shrinkTime) > 0)
        {
            damageRingGameObject.transform.localScale = Vector3.one * endRingDiameter * EasingFunctions.EaseInOutCubic(currentShrinkTime / shrinkTime);
        }


    }



    public void ShowRing(float chargeTime, float endRadius, float startRadius = 0)
    {
        ringGrowTime = chargeTime;
        currentRingGrowTime = 0;

        endRingDiameter = endRadius * 2f;
        startRingDiameter = startRadius * 2f;

        showRing = true;
    }

    // this should also be in time.    
    public void HideRing(float shrinkSpeed = 2f)
    {
        shrinkRate = shrinkSpeed;

        if (showRing)
            currentShrinkTime = shrinkTime;

        showRing = false;
        //ringDiameter = 1f;
    }
}
