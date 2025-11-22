using System;
using UnityEngine;
using UnityEngine.UI;

public class EnableFullScreen : MonoBehaviour
{
    [SerializeField]
    SettingsMenu settingsMenu;

    Toggle toggle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        toggle = GetComponent<Toggle>();

        toggle.SetIsOnWithoutNotify(Screen.fullScreen);

        toggle.onValueChanged.AddListener(OnToggle);
    }

    private void OnToggle(bool arg0)
    {
        settingsMenu.SetFullScreen(arg0);
    }
}
