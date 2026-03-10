using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class PauseCanvas : MonoBehaviour
{
    private GameObject playerObject;

    private PlayerMovement playerMovement;
    private PlayerCombat playerCombat;
    [SerializeField]
    private CameraController playerCamera;

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

    private int unpausedPlayerMoveState;
    private bool unpausedPlayerCombatState;
    private bool unpausedCameraState;


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
        pauseInput = InputSystem.actions.FindAction("Pause");

        pauseInput.started += AlternateState;

        playerObject = PlayerRefFetcher.Instance.GetPlayerRef();
        playerMovement = playerObject.GetComponent<PlayerMovement>();
        playerCombat = playerObject.GetComponent<PlayerCombat>();

        if (playerCamera == null)
        {
            playerCamera = Camera.main.gameObject.GetComponent<CameraController>();
        }

        ResumeGame(); // closes the pause menu so the player can play the game.
    }

    private void Update()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main.gameObject.GetComponent<CameraController>();
        }

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
        unpausedPlayerMoveState = playerMovement.DisabledType;
        unpausedPlayerCombatState = playerCombat.IsDisabled;
        if (playerCamera != null)
            unpausedCameraState = playerCamera.IsDisabled;

        playerMovement.DisablePlayerMovement(1);
        playerCombat.DisablePlayerCombat(true);
        if (playerCamera != null)
            playerCamera.DisableCameraInput(true);

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

        Time.timeScale = 0;

        pauseCanvasCollection.SetActive(true);

    }

    public void ResumeGame()
    {
        playerMovement.DisablePlayerMovement(unpausedPlayerMoveState);
        playerCombat.DisablePlayerCombat(unpausedPlayerCombatState);
        if (playerCamera != null)
            playerCamera.DisableCameraInput(unpausedCameraState);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;


        Time.timeScale = 1;

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
        playerMovement.DisablePlayerMovement(unpausedPlayerMoveState);
        playerCombat.DisablePlayerCombat(unpausedPlayerCombatState);
        if (playerCamera != null)
            playerCamera.DisableCameraInput(unpausedCameraState);

        Time.timeScale = 1;

        LevelLoading.Instance.LoadMainMenu();
    }

    public void Quit()
    {
        Application.Quit();
    }

}
