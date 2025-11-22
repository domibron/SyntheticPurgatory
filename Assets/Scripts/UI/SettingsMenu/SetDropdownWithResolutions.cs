using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SetDropdownWithResolutions : MonoBehaviour
{
    [SerializeField]
    SettingsMenu settingsMenu;

    List<SettingsMenu.ScreenResolution> allRealRes = new List<SettingsMenu.ScreenResolution>();

    TMP_Dropdown dropdown;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();

        dropdown.options.Clear();

        Resolution[] allRes = Screen.resolutions;

        foreach (Resolution resolution in allRes)
        {
            if (!allRealRes.Contains(new SettingsMenu.ScreenResolution(resolution)))
            {
                allRealRes.Add(new SettingsMenu.ScreenResolution(resolution));
            }
        }

        Resolution curRes = Screen.currentResolution;
        int selectedRes = 0;

        for (int i = 0; i < allRealRes.Count; i++)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(allRealRes[i].ToString()));
            if (allRealRes[i].width == curRes.width && allRealRes[i].height == curRes.height)
            {
                selectedRes = i;
            }
        }

        dropdown.SetValueWithoutNotify(selectedRes);

        dropdown.onValueChanged.AddListener(InformSettingsOfDesiredChange);
    }

    public void InformSettingsOfDesiredChange(int option)
    {
        // print(dropdown.value + allRealRes[dropdown.value].ToString() + "");
        settingsMenu.ChangeResolution(allRealRes[dropdown.value]);
    }
}
