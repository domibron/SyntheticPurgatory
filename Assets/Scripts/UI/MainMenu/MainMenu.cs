using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    // public string HubWorldSceneName = "HubWorld";

    [SerializeField]
    GameObject startGameButton;

    void Start()
    {
        if (InputManager.Instance.GetCurrentInputDevice() == InputManager.InputDeviceType.Gamepad)
        {
            EventSystem.current.SetSelectedGameObject(startGameButton);
            Cursor.visible = false;
            print("controller detected");
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
            Cursor.visible = true;
            print("KaM detected");
        }
    }

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
        if (newDevice == InputManager.InputDeviceType.Gamepad)
        {
            EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
            Cursor.visible = false;
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
            Cursor.visible = true;
        }
    }

    public void StartNewGame()
    {
        if (LevelLoading.Instance == null) return;

        if (LevelLoading.Instance.IsLoading) return;

        LevelLoading.Instance.LoadScene(LevelCollection.LevelKey.SetupScreen.ToString());
    }

    public void StartTutorial()
    {
        if (LevelLoading.Instance == null) return;

        if (LevelLoading.Instance.IsLoading) return;

        LevelLoading.Instance.LoadScene(LevelCollection.LevelKey.Tutorial.ToString());
    }

    public void Quit()
    {
        Application.Quit();
    }
}
