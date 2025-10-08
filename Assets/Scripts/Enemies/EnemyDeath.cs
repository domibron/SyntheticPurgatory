using System;
using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    public event Action onDeathEvent;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Health>().onDeath += KillEnemy;
    }

    void KillEnemy()
    {
        Destroy(gameObject); // Destroy enemy
    }
}
