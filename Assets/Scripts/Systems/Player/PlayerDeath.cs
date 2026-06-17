using UnityEngine;

/// <summary>
/// Handles showing the death canvas and other things when the player dies.
/// </summary>
public class PlayerDeath : MonoBehaviour
{
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
        GetComponent<Health>().onDeath += OnPlayerDeath;
    }

    /// <summary>
    /// Shows the canvas and shows the cursor if key board and mouse.
    /// </summary>
    public void OnPlayerDeath()
    {
        transform.GetComponent<PlayerMovement>().SetDisabledState(PlayerMovement.DisabledType.All);
        transform.GetComponent<PlayerCombat>().DisablePlayerCombat(true);

        if (RunManager.Instance)
            RunManager.Instance.statsHolder.LoseLife();

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

        if (RunManager.Instance.GetCurrentLives() > 1)
        {
            deathCanvasScript.ActivateCanvas(true); // Activate death screen
            if (ScrapLevelM.Instance != null)
                deathCanvasScript.ShowStats(ScrapLevelM.Instance.currentDepositedScrap, ScrapLevelM.Instance.currentInventoryScrap);
        }
        else
        {
            endCanvasScript.ActivateCanvas(true);
        }
    }
}
