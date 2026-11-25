using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class PauseCanvas : MonoBehaviour
{
    private static PauseState m_pauseState;

    private static PauseState m_lastKnownPauseState;

    private static PauseState m_lastKnownDebugPauseSate;

    public static bool IsPaused
    {
        get
        {
            return (m_pauseState == PauseState.DebugConsolePaused || m_pauseState == PauseState.OverridePaused || m_pauseState == PauseState.Paused);
        }
    }

    public static bool IsResumed
    {
        get
        {
            return (m_pauseState == PauseState.OverrideResume || m_pauseState == PauseState.Resumed);
        }
    }

    public static bool IsForcedPaused
    {
        get
        {
            return (m_pauseState == PauseState.DebugConsolePaused || m_pauseState == PauseState.OverridePaused);
        }
    }

    public static bool IsForcedResume
    {
        get
        {
            return (m_pauseState == PauseState.OverrideResume);
        }
    }

    public static PauseState CurrentPauseState
    {
        get => m_pauseState;
    }

    public enum PauseState : int
    {
        /// <summary>
        /// The game is playing. Used by pause menu.
        /// </summary>
        Resumed,

        /// <summary>
        /// The game is paused. Used by pause menu.
        /// </summary>
        Paused,

        /// <summary>
        /// Something else is controlling the game's pause state.
        /// </summary>
        OverridePaused,

        /// <summary>
        /// Something else is controlling the game's pause state.
        /// </summary>
        OverrideResume,

        /// <summary>
        /// Debug console is pausing the game.
        /// </summary>
        DebugConsolePaused,

        /// <summary>
        /// Deny any overrides.
        /// </summary>
        DenyOverrides,

        /// <summary>
        /// Restore the last pause state.
        /// </summary>
        RestoreFromOverride,

        /// <summary>
        /// Restore the last pause state.
        /// </summary>
        RestoreFromDebugPause,
    }


    private GameObject playerObject;

    // private PlayerMovement playerMovement;
    // private PlayerCombat playerCombat;

    private PlayerDisabling playerDisabling;

    /// <summary>
    /// Object on the canvas that contains all the pause canvas GUI
    /// </summary>
    [SerializeField]
    private GameObject pauseCanvasCollection;
    /// <summary>
    /// Object on the canvas that contains all the death canvas GUI
    /// </summary>
    [SerializeField]
    private GameObject deathCanvasCollection;
    /// <summary>
    /// Object on the canvas that contains all the death canvas GUI
    /// </summary>
    [SerializeField]
    private GameObject endStateCanvasCollection;

    /// <summary>
    /// Object on the canvas that contains all the death canvas GUI
    /// </summary>
    [SerializeField]
    private GameObject settingsCanvasCollection;

    InputAction pauseInput;
    bool settingsCloseBuffer = false;

    private PlayerDisabling.DisabledType unpausedPlayerState;

    private float unpausedPausedState = 1f;

    void OnEnable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.onDeviceChanged += OnDeviceChanged;
    }

    void OnDestroy()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.onDeviceChanged -= OnDeviceChanged;

        m_pauseState = PauseState.Resumed;
        Time.timeScale = 1;
    }

    private void OnDeviceChanged(InputManager.InputDeviceType newDevice, InputManager.InputDeviceType oldDevice)
    {
        if (!pauseCanvasCollection.activeSelf) return;


        if (newDevice == InputManager.InputDeviceType.Gamepad)
        {
            EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
            Cursor.visible = false;
        }
        else
        {
            Cursor.visible = true;
        }
    }

    void Start()
    {
        SetPauseState(PauseState.Resumed);

        pauseInput = InputSystem.actions.FindAction("Pause");

        pauseInput.started += AlternateState;

        playerObject = PlayerRefFetcher.Instance.gameObject;
        // playerMovement = playerObject.GetComponent<PlayerMovement>();
        // playerCombat = playerObject.GetComponent<PlayerCombat>();
        playerDisabling = playerObject.GetComponent<PlayerDisabling>();

        ResumeGame(); // closes the pause menu so the player can play the game.
    }

    private void Update()
    {

        // if (pauseCanvasCollection.activeSelf)
        // {
        //     if (InputManager.Instance != null)
        //     {
        //         if (InputManager.Instance.GetCurrentInputDevice() == InputManager.InputDeviceType.Keyboard)
        //         {
        //             Cursor.visible = true;
        //         }
        //         else
        //         {
        //             Cursor.visible = false;
        //         }
        //     }
        // }
    }

    private void AlternateState(InputAction.CallbackContext context)
    {
        if (pauseCanvasCollection == null) { return; }
        if (settingsCloseBuffer) { return; }

        ActivateCanvas(!pauseCanvasCollection.gameObject.activeSelf);
    }


    /// <summary>
    /// Activate and enable visibility of the Pause canvas
    /// </summary>
    /// <param name="state">Whether to turn on or off the pause canvas</param>
    public void ActivateCanvas(bool state)
    {
        if (settingsCloseBuffer) { return; } // Avoid switching if settings were just closed

        if (settingsCanvasCollection.gameObject.activeSelf) // Do not allow switching if settings are open
        {
            return;
        }

        if (deathCanvasCollection.gameObject.activeSelf || endStateCanvasCollection.gameObject.activeSelf) // Don't allow player to open pause menu when on death/end screen
        {
            if (!state) // Allow ability to close pause screen if player somehow dies when pause screen opens
            {
                pauseCanvasCollection.SetActive(false);
            }
            return;
        }

        if (state)
        {
            OpenPauseMenu();
        }
        else
        {
            ResumeGame();
        }
    }

    public void OpenPauseMenu()
    {
        // if (IsForcedPaused || IsForcedResume) return;

        unpausedPlayerState = playerDisabling.CurrentDisabledState;

        playerDisabling.CurrentDisabledState = PlayerDisabling.DisabledType.All;

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

        SetPauseState(PauseState.Paused);

        pauseCanvasCollection.SetActive(true);
    }

    public void ResumeGame()
    {
        // if (IsForcedPaused || IsForcedResume) return;

        playerDisabling.CurrentDisabledState = unpausedPlayerState;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;


        SetPauseState(PauseState.Resumed);

        pauseCanvasCollection.SetActive(false);
    }

    public IEnumerator SettingsClosedDelay()
    {
        settingsCloseBuffer = true;

        yield return new WaitForSecondsRealtime(0.1f);

        settingsCloseBuffer = false;
    }



    /// <summary>
    /// Return back to hub
    /// </summary>
    public void ReturnToMainMenu()
    {
        //? Isn't the player destroyed? So this is pointless?
        playerDisabling.CurrentDisabledState = unpausedPlayerState;

        // Deny any overrides.
        m_pauseState = PauseState.DenyOverrides;

        // make sure time is resumed.
        Time.timeScale = 1;

        LevelLoading.Instance.LoadMainMenu();
    }

    public void Quit()
    {
        Application.Quit();
    }


    public static void UpdateTimeScaleBasedOnPauseState()
    {

    }

    public static void SetPauseState(PauseState pauseState)
    {
        print($"IN {pauseState} CUR {m_pauseState}");

        if (m_pauseState == PauseState.DenyOverrides)
        {
            Debug.LogWarning("Pause system is denying overrides due to requirements to keep time flowing.");
            return;
        }

        if (m_pauseState == pauseState)
        {
            return;
        }

        if (m_pauseState == PauseState.DebugConsolePaused)
        {
            if (pauseState == PauseState.DebugConsolePaused)
            {
                return;
            }
            else if (pauseState == PauseState.RestoreFromDebugPause)
            {
                m_pauseState = PauseState.RestoreFromDebugPause;
                SetPauseState(m_lastKnownDebugPauseSate);
                return;
            }
            else if (pauseState == PauseState.Resumed)
            {
                if (m_lastKnownDebugPauseSate == PauseState.OverridePaused)
                {
                    m_lastKnownPauseState = PauseState.Resumed;
                }
                else
                {
                    m_lastKnownDebugPauseSate = pauseState;
                }
            }
            else if (pauseState == PauseState.Paused)
            {
                if (m_pauseState == PauseState.OverrideResume)
                {
                    m_lastKnownPauseState = PauseState.Paused;
                }
                else
                {
                    m_lastKnownDebugPauseSate = pauseState;
                }
            }
            else if (pauseState == PauseState.OverridePaused)
            {
                m_lastKnownDebugPauseSate = pauseState;
            }
            else if (pauseState == PauseState.OverrideResume)
            {
                m_lastKnownDebugPauseSate = pauseState;
            }
            else if (pauseState == PauseState.RestoreFromOverride)
            {
                m_lastKnownDebugPauseSate = pauseState;
            }

            Debug.LogWarning("Debug console is denying overrides.");

            return;
        }

        switch (pauseState)
        {
            case PauseState.Resumed:
                if (m_pauseState == PauseState.OverridePaused)
                {
                    m_lastKnownPauseState = PauseState.Resumed;
                }
                else
                {
                    m_pauseState = pauseState;
                    Time.timeScale = 1;
                }
                return;

            case PauseState.Paused:
                if (m_pauseState == PauseState.OverrideResume)
                {
                    m_lastKnownPauseState = PauseState.Paused;
                }
                else
                {
                    m_pauseState = pauseState;
                    Time.timeScale = 0;
                }
                return;

            case PauseState.OverridePaused:
                m_lastKnownPauseState = m_pauseState;
                m_pauseState = pauseState;
                Time.timeScale = 1;
                return;

            case PauseState.OverrideResume:
                m_lastKnownPauseState = m_pauseState;
                m_pauseState = pauseState;
                Time.timeScale = 0;
                return;

            case PauseState.RestoreFromOverride:
                m_pauseState = PauseState.RestoreFromOverride;
                SetPauseState(m_lastKnownPauseState);
                return;// DO NOT SET VALUES.

            case PauseState.RestoreFromDebugPause:
                m_pauseState = PauseState.RestoreFromDebugPause;
                SetPauseState(m_lastKnownDebugPauseSate);
                return;// DO NOT SET VALUES.

            case PauseState.DebugConsolePaused:
                m_lastKnownDebugPauseSate = m_pauseState;
                m_pauseState = pauseState;
                Time.timeScale = 0;
                return;

            case PauseState.DenyOverrides:
                m_pauseState = pauseState;
                Time.timeScale = 1;
                return;
        }
    }


}
