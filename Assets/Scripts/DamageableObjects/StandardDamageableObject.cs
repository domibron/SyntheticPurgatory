using System;
using TMPro;
using UnityEngine;



public class StandardDamageableObject : MonoBehaviour, IDamageable
{
    private Health health;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = GetComponent<Health>();
    }

    void IDamageable.TakeDamage(float damage, Vector3 hitPosition)
    {
        // floating numbers pop up.
        health.AddToHealth(-Mathf.Abs(damage));

        // print(hitPointsData.GetGradient(type));
    }
}
