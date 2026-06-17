using System;
using UnityEngine;

// By Vince Pressey

public class ObjectDeath : MonoBehaviour
{
    //public event Action onDeathEvent;

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

    [SerializeField]
    private bool addToKillCounter = true;
    [SerializeField]
    private int enemyScore = 10;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Health>().onDeath += KillObject;
    }

    /// <summary>
    /// Initialize death scrap spawning then delete object
    /// </summary>
    void KillObject()
    {
        transform.GetComponent<ScrapDropper>().SpawnScrapGroup(ScrapDrop, sideForce, upForce); // Spawn Scrap

        if (addToKillCounter && RunManager.Instance)
        {
            RunManager.Instance.statsHolder.enemiesDefeated++;
            RunManager.Instance.statsHolder.enemiesDefeatedScore += enemyScore;
        }

        Destroy(gameObject); // Destroy object
    }
}
