using System;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// None functional
/// </summary>
public class RandomizeTexture : MonoBehaviour
{
    [SerializeField]
    VisualEffect visualEffect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // visualEffect.SetTexture();
    }

    private void visualEffect_outputEventReceived(VFXOutputEventArgs args)
    {
        // print("output");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
