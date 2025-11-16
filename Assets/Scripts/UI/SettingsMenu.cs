using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SettingsMenu : MonoBehaviour
{
    /// <summary>
    /// Object on the canvas that contains all the settings canvas GUI
    /// </summary>
    private GameObject settingsCanvasCollection;

    /// <summary>
    /// Object on the canvas that contains all the pause canvas GUI
    /// </summary>
    [SerializeField]
    private PauseCanvas pauseCanvas;

    InputAction pauseInput;

    [SerializeField]
    private GameObject gameplayPage;
    [SerializeField]
    private GameObject videoPage;
    [SerializeField]
    private GameObject soundPage;
    [SerializeField]
    private GameObject controlsPage;



    void Awake()
    {
        settingsCanvasCollection = transform.GetChild(0).gameObject;

        pauseInput = InputSystem.actions.FindAction("Pause");

        pauseInput.started += KeyCloseSettings;
    }

    private void KeyCloseSettings(InputAction.CallbackContext context)
    {
        CloseSettings();
    }

    /// <summary>
    /// Activate and enable visibility of the settings canvas
    /// </summary>
    /// <param name="state">Whether to turn on or off the settings canvas</param>
    public void ActivateCanvas(bool state)
    {

        if (state)
        {
            OpenSettings();
        }
        else
        {
            CloseSettings();
        }
    }

    public void OpenSettings()
    {
        if (settingsCanvasCollection == null) { return; }

        settingsCanvasCollection.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsCanvasCollection == null) { return; }
        if (!settingsCanvasCollection.gameObject.activeSelf) { return; }

        if (pauseCanvas != null) { pauseCanvas.StartCoroutine("SettingsClosedDelay"); }

        settingsCanvasCollection.SetActive(false);
    }

    public void OpenGameplaySettings()
    {
        gameplayPage.SetActive(true);
        videoPage.SetActive(false);
        soundPage.SetActive(false);
        controlsPage.SetActive(false);
    }

    public void OpenVideoSettings()
    {
        gameplayPage.SetActive(true);
        videoPage.SetActive(true);
        soundPage.SetActive(false);
        controlsPage.SetActive(false);
    }

    public void OpenSoundSettings()
    {
        gameplayPage.SetActive(true);
        videoPage.SetActive(false);
        soundPage.SetActive(true);
        controlsPage.SetActive(false);
    }

    public void OpenControlSettings()
    {
        gameplayPage.SetActive(true);
        videoPage.SetActive(false);
        soundPage.SetActive(false);
        controlsPage.SetActive(true);
    }

}
