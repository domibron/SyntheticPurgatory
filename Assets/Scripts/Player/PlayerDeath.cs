using System;
using UnityEngine;

// By Vince Pressey

public class PlayerDeath : MonoBehaviour
{
    public event Action onDeathEvent; // TEMPORARY MAKE PRIVATE + SERIALIZE
    /// <summary>
    /// DeathCanvas object to activate upon death
    /// </summary>
    public DeathCanvas deathCanvasScript;
    /// <summary>
    /// EndStateScreen object to activate upon death
    /// </summary>
    public EndStateScreen endCanvasScript;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        endCanvasScript = deathCanvasScript.transform.GetComponent<EndStateScreen>(); //TEMPORARY
        GetComponent<Health>().onDeath += KillPlayer;
    }

    public void KillPlayer()
    {
        transform.GetComponent<PlayerMovement>().DisablePlayerMovement(2);
        transform.GetComponent<PlayerCombat>().DisablePlayerCombat(true);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        if (GameManager.Instance.GetCurrentLives() > 1)
        {
            deathCanvasScript.ActivateCanvas(true); // Activate death screen
            if (ScrapManager.Instance != null)
                deathCanvasScript.ShowStats(ScrapManager.Instance.currentDepositedScrap, ScrapManager.Instance.currentInventoryScrap);
        }
        else
        {
            endCanvasScript.ActivateCanvas(true);
        }


    }
}
