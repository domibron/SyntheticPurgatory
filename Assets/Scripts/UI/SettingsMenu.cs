using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[Serializable]
public class SettingsData
{

}

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

    [Serializable]
    public struct ScreenResolution
    {
        public int width;
        public int height;
    }


    void Awake()
    {
        settingsCanvasCollection = transform.GetChild(0).gameObject;

        pauseInput = InputSystem.actions.FindAction("Pause");

        pauseInput.started += KeyCloseSettings;

        OpenGameplaySettings(); // want to make sure only one UI panel is up. a little reset as you will.
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

        if (pauseCanvas != null) { pauseCanvas.StartCoroutine(pauseCanvas.SettingsClosedDelay()); }

        settingsCanvasCollection.SetActive(false);
    }

    // TODO: replace with enum system instead.
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


    // Actaul settings

    public void ChangeResolution(ScreenResolution resolution)
    {
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode, Screen.currentResolution.refreshRateRatio);
    }

    public void ChangeRefreshRate(RefreshRate refreshRate)
    {
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, Screen.fullScreenMode, refreshRate);
    }

    public void ChangeFullScreenMode(FullScreenMode fullScreenMode)
    {
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, fullScreenMode, Screen.currentResolution.refreshRateRatio);
    }

    public void SetVsyncEnabled(bool enableVSync = false, bool ifEnableUseHalfVSync = false)
    {
        QualitySettings.vSyncCount = (enableVSync ? (ifEnableUseHalfVSync ? 2 : 1) : 0);
    }

    public void SetTargetFrameRate(int targetFrameRate = -1)
    {
        Application.targetFrameRate = targetFrameRate;
    }


}
