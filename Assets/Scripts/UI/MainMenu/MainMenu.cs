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
        EventSystem.current.SetSelectedGameObject(startGameButton);
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
            Cursor.visible = true;
        }
    }

    public void StartNewGame()
    {
        if (LevelLoading.Instance == null) return;

        if (LevelLoading.Instance.IsLoading) return;

        LevelLoading.Instance.LoadScene(LevelCollection.LevelKey.SetupScreen.ToString());
    }

    public void Quit()
    {
        Application.Quit();
    }
}
