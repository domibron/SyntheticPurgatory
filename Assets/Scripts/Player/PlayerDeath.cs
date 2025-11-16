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

    public void KillPlayer()
    {
        transform.GetComponent<PlayerMovement>().DisablePlayerMovement(2);
        transform.GetComponent<PlayerCombat>().DisablePlayerCombat(true);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        deathCanvasScript.ActivateCanvas(true); // Activate death screen
        if (ScrapManager.Instance != null)
            deathCanvasScript.ShowStats(ScrapManager.Instance.currentDepositedScrap, ScrapManager.Instance.currentInventoryScrap);

    }
}
