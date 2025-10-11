using System;
using UnityEngine;
using static UnityEngine.Rendering.GPUSort;

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
        transform.GetComponent<ScrapDropper>().SpawnScrapGroup(63);
        Destroy(gameObject); // Destroy enemy
    }
}
