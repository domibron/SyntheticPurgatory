using System;
using UnityEngine;

// By Vince Pressey

public class PlayerDeath : MonoBehaviour
{
    public event Action onDeathEvent;
    /// <summary>
    /// Object to activate upon death
    /// </summary>
    public DeathCanvas deathCanvasScript;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Health>().onDeath += KillPlayer;
    }

    void KillPlayer()
    {
        transform.GetComponent<PlayerMovement>().DisablePlayerMovement(true);
        transform.GetComponent<PlayerCombat>().DisablePlayerCombat(true);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        deathCanvasScript.ActivateCanvas(true); // Activate death screen
        deathCanvasScript.ShowStats(ScrapManager.Instance.currentDepositedScrap, ScrapManager.Instance.currentInventoryScrap);

    }
}
