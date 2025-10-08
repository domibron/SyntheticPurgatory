using System;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public event Action onDeathEvent;
    public GameObject deathScreen;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Health>().onDeath += EndGame;
    }

    void EndGame()
    {
        deathScreen.SetActive(true); // Activate death screen (WIP)
    }
}
