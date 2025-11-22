using System;
using UnityEngine;
using UnityEngine.UI;

public class SetInvert : MonoBehaviour
{
    [SerializeField]
    bool isGamepad = false;

    private Toggle toggle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        toggle = GetComponent<Toggle>();

        toggle.SetIsOnWithoutNotify((isGamepad ? SettingsMenu.GetGamepadInvertY() : SettingsMenu.GetMouseInvertY()));

        toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool arg0)
    {
        if (isGamepad)
            SettingsMenu.SetGamepadInvertY(arg0);
        else
            SettingsMenu.SetMouseInvertY(arg0);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
