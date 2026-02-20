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

        GameManager.Instance.statsHolder.LoseLife();

        Cursor.lockState = CursorLockMode.None;

        if (InputManager.Instance != null)
        {
            if (InputManager.Instance.GetCurrentInputDevice() == InputManager.InputDeviceType.Keyboard)
            {
                Cursor.visible = true;
            }
            else
            {
                Cursor.visible = false;
            }
        }
        else
        {
            Cursor.visible = true;
        }

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
