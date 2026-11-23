using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class PauseCanvas : MonoBehaviour
{
    private static PauseState m_pauseState;

    private static PauseState m_lastKnownPauseState;

    public static PauseState CurrentPauseState
    {
        get => m_pauseState;
        set
        {
            if (value == PauseState.Override)
            {
                m_lastKnownPauseState = m_pauseState;
            }
            else if (value == PauseState.Restore)
            {
                m_pauseState = m_lastKnownPauseState;
            }

            if (m_pauseState != PauseState.DenyOverrides)
            {
                m_pauseState = value;
            }
        }
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
        Override,

        /// <summary>
        /// Deny any overrides.
        /// </summary>
        DenyOverrides,

        /// <summary>
        /// Restore the last pause state.
        /// </summary>
        Restore,
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
        CurrentPauseState = PauseState.Resumed;

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

        TryPausingTime();

        pauseCanvasCollection.SetActive(true);

    }

    public void ResumeGame()
    {
        playerDisabling.CurrentDisabledState = unpausedPlayerState;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;


        TryResumingTime();

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


    public void TryPausingTime()
    {
        if (m_pauseState == PauseState.Resumed)
        {
            m_pauseState = PauseState.Paused;
            unpausedPausedState = Time.timeScale;
        }
        else
        {
            m_lastKnownPauseState = PauseState.Paused;
        }
    }

    public void TryResumingTime()
    {
        if (m_pauseState == PauseState.Paused)
        {
            m_pauseState = PauseState.Resumed;
            Time.timeScale = unpausedPausedState;
        }
        else
        {
            m_lastKnownPauseState = PauseState.Resumed;
        }
    }
}
