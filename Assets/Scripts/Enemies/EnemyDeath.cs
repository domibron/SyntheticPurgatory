using System;
using UnityEngine;

// By Vince Pressey

public class EnemyDeath : MonoBehaviour
{
    public event Action onDeathEvent;

    /// <summary>
    /// Scrap dropped upon death
    /// </summary>
    [Header("Scrap")]
    public int ScrapDrop = 5;
    /// <summary>
    /// Force applied horizontally to scrap object (X and Z)
    /// </summary>
    [SerializeField]
    private float sideForce = 2.5f;
    /// <summary>
    /// Force applied vertically to scrap object (Y) 
    /// </summary>
    [SerializeField] 
    private float upForce = 2f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Health>().onDeath += KillEnemy;
    }

    /// <summary>
    /// Initialize death scrap spawning then delete object
    /// </summary>
    void KillEnemy()
    {
        transform.GetComponent<ScrapDropper>().SpawnScrapGroup(ScrapDrop, sideForce, upForce); // Spawn Scrap
        Destroy(gameObject); // Destroy enemy
    }
}
