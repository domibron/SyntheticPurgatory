using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HubWorldUI : MonoBehaviour
{
    // public string DungeonWorldSceneName = "DungeonWorld";
    // public string BossWorldSceneName = "BossWorld";
    // public string MainMenuSceneName = "MainMenu";

    // public TMP_Text ScrapText;

    // public GameObject MainUI;
    // public GameObject UpgradeUI;

    enum WaitingConfirmationFor
    {
        None,
        Boss,
        MainMenu,
        Quit,
    }

    WaitingConfirmationFor waitingConfirmationFor = WaitingConfirmationFor.None;

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

    void Start()
    {
        ConfirmationBox.Instance.OnConfirmation += OnConfirmation;
    }

    void Update()
    {
        if (InputManager.Instance.GetCurrentInputDevice() == InputManager.InputDeviceType.Keyboard)
        {
            Cursor.visible = false; // using a custom cursor.
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.None;
        }

    }

    private void OnConfirmation(bool confirmedAction)
    {
        if (waitingConfirmationFor == WaitingConfirmationFor.None) return;

        if (!confirmedAction) return;

        switch (waitingConfirmationFor)
        {
            case WaitingConfirmationFor.Boss:
                LoadBossLevel();
                break;
            case WaitingConfirmationFor.MainMenu:
                LoadMainMenu();
                break;
            case WaitingConfirmationFor.Quit:
                Quit();
                break;
        }
    }

    private void OnDeviceChanged(InputManager.InputDeviceType newDevice, InputManager.InputDeviceType oldDevice)
    {
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

    public void StartNextRun()
    {
        if (LevelLoading.Instance != null)
            LevelLoading.Instance.LoadScene(LevelCollection.LevelKey.DungeonWorld.ToString());
    }

    public void LoadMainMenu()
    {
        if (RunManager.Instance != null)
        {
            // remove this so no issues occur.
            Destroy(RunManager.Instance.gameObject);
        }

        if (LevelLoading.Instance != null)
            LevelLoading.Instance.LoadScene(LevelCollection.LevelKey.MainMenu.ToString());
    }
    public void LoadBossLevel()
    {
        if (LevelLoading.Instance != null)
            LevelLoading.Instance.LoadScene(LevelCollection.LevelKey.BossWorld.ToString());
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void GetConfirmationLoadMainMenu()
    {
        waitingConfirmationFor = WaitingConfirmationFor.MainMenu;
        if (!ConfirmationBox.Instance.TryOpenConfirmationBox("Main Menu", "Are you sure you want to lose all progress and quit to the main menu?"))
        {
            waitingConfirmationFor = WaitingConfirmationFor.None;
        }
    }

    public void GetConfirmationLoadBossLevel()
    {
        waitingConfirmationFor = WaitingConfirmationFor.Boss;
        if (!ConfirmationBox.Instance.TryOpenConfirmationBox("Challenge Boss", "Are you sure you want to go against the boss? Like now? Are you really sure?"))
        {
            waitingConfirmationFor = WaitingConfirmationFor.None;
        }
    }

    public void GetConfirmationQuit()
    {
        waitingConfirmationFor = WaitingConfirmationFor.Quit;
        if (!ConfirmationBox.Instance.TryOpenConfirmationBox("Quit", "Are you sure you want to lose all progress and exit out of the game?"))
        {
            waitingConfirmationFor = WaitingConfirmationFor.None;
        }
    }
}
