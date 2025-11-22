using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetFullScreen : MonoBehaviour
{
    [SerializeField]
    SettingsMenu settingsMenu;

    TMP_Dropdown fullscreenMode;

    List<FullScreenMode> fullScreenModes = new List<FullScreenMode>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fullscreenMode = GetComponent<TMP_Dropdown>();

        // toggle.SetIsOnWithoutNotify(Screen.fullScreen);

        // Screen.fullScreen = true;
        fullscreenMode.options.Clear();

        foreach (FullScreenMode mode in Enum.GetValues(typeof(FullScreenMode)))
        {
            fullScreenModes.Add(mode);
            fullscreenMode.options.Add(new TMP_Dropdown.OptionData(mode.ToString()));
        }

        for (int i = 0; i < fullScreenModes.Count; i++)
        {
            if (Screen.fullScreenMode == fullScreenModes[i])
            {
                fullscreenMode.SetValueWithoutNotify(i);
            }
        }

        fullscreenMode.onValueChanged.AddListener(SetFullScreenMode);
    }

    public void SetFullScreenMode(int option)
    {
        settingsMenu.ChangeFullScreenMode(fullScreenModes[fullscreenMode.value]);
    }
}
