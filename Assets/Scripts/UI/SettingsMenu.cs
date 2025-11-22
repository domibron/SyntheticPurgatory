using System;
using UnityEngine;
using UnityEngine.Audio;
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
    InputAction closeInput;

    [SerializeField]
    private GameObject gameplayPage;
    [SerializeField]
    private GameObject videoPage;
    [SerializeField]
    private GameObject soundPage;
    [SerializeField]
    private GameObject controlsPage;

    [SerializeField]
    private GameObject defaultSelectedObject;

    [SerializeField]
    private AudioMixer mixer;

    [Serializable]
    public struct ScreenResolution
    {
        public int width;
        public int height;

        public ScreenResolution(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public ScreenResolution(Resolution resolution)
        {
            width = resolution.width;
            height = resolution.height;
        }

        public override string ToString()
        {
            return $"{width.ToString()}x{height.ToString()}";
        }
    }


    void Awake()
    {
        settingsCanvasCollection = transform.GetChild(0).gameObject;

        pauseInput = InputSystem.actions.FindAction("Pause");
        closeInput = InputSystem.actions.FindAction("Close");


        pauseInput.started += KeyOpenSettings;
        // closeInput.started += KeyCloseSettings;

        OpenGameplaySettings(); // want to make sure only one UI panel is up. a little reset as you will.
        CloseSettings();

    }

    void Start()
    {
    }


    private void KeyOpenSettings(InputAction.CallbackContext context)
    {
        if (settingsCanvasCollection.activeSelf) return;
        if (context.performed)
            OpenSettings();
    }

    private void KeyCloseSettings(InputAction.CallbackContext context)
    {
        if (!settingsCanvasCollection.activeSelf) return;
        if (context.performed)
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

        EventSystem.current.SetSelectedGameObject(defaultSelectedObject);
    }

    public void CloseSettings()
    {
        if (settingsCanvasCollection == null) { return; }
        if (!settingsCanvasCollection.gameObject.activeSelf) { return; }

        if (pauseCanvas != null) { pauseCanvas.StartCoroutine(pauseCanvas.SettingsClosedDelay()); }

        settingsCanvasCollection.SetActive(false);

        EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
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

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void SetVSyncEnabled(bool enableVSync = false, bool ifEnableUseHalfVSync = false)
    {
        QualitySettings.vSyncCount = (enableVSync ? (ifEnableUseHalfVSync ? 2 : 1) : 0);
    }

    public void SetTargetFrameRate(int targetFrameRate = -1)
    {
        Application.targetFrameRate = targetFrameRate;
    }

    public void SetVolume(string volumeVar, float value)
    {
        mixer.SetFloat(volumeVar, Mathf.Log10(value) * 20f); // convert linear to db.
    }

    public float GetVolumeValue(string volumeVar)
    {
        float returnedFloat = 0f;
        mixer.GetFloat(volumeVar, out returnedFloat);

        returnedFloat = Mathf.Pow(10f, returnedFloat / 20f);

        return returnedFloat;
    }

    #region Move to input manager or something.
    #endregion

    public static void SetMouseXSens(float value)
    {
        PlayerPrefs.SetFloat("mouseX", value);
    }

    public static void SetMouseYSens(float value)
    {
        PlayerPrefs.SetFloat("mouseY", value);
    }

    public static float GetMouseXSens()
    {
        return PlayerPrefs.GetFloat("mouseX", 10f);
    }

    public static float GetMouseYSens()
    {
        return PlayerPrefs.GetFloat("mouseY", 10f);
    }

    public static void SetMouseInvertY(bool invertY)
    {
        PlayerPrefs.SetInt("invertMouse", invertY ? 1 : 0);
    }

    public static bool GetMouseInvertY()
    {
        return PlayerPrefs.GetInt("invertMouse", 0) == 1;
    }

    public static void SetGamepadXSens(float value)
    {
        PlayerPrefs.SetFloat("gamepadX", value);
    }

    public static void SetGamepadYSens(float value)
    {
        PlayerPrefs.SetFloat("gamepadY", value);
    }

    public static float GetGamepadXSens()
    {
        return PlayerPrefs.GetFloat("gamepadX", 10f);

    }

    public static float GetGamepadYSens()
    {
        return PlayerPrefs.GetFloat("gamepadY", 10f);

    }

    public static void SetGamepadInvertY(bool invertY)
    {
        PlayerPrefs.SetInt("InvertGamepad", invertY ? 1 : 0);
    }

    public static bool GetGamepadInvertY()
    {
        return PlayerPrefs.GetInt("InvertGamepad", 0) == 1;
    }
}
