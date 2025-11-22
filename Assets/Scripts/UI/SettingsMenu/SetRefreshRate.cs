using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SetRefreshRate : MonoBehaviour
{
    [SerializeField]
    private SettingsMenu settingsMenu;

    TMP_Dropdown dropdown;

    List<RefreshRate> refreshRates = new List<RefreshRate>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();

        // refreshRates = Screen.resolutions.Select(res => res.refreshRateRatio).ToList();

        foreach (Resolution res in Screen.resolutions) // TODO: figure out why there is only one refresh rate option.
        {
            // print("hz " + res.refreshRateRatio.numerator + " " + res.refreshRateRatio.denominator + " " + res.refreshRateRatio.value);
            if (!refreshRates.Contains(res.refreshRateRatio))
            {
                refreshRates.Add(res.refreshRateRatio);
            }
            // print(res.ToString());
        }

        dropdown.options.Clear();

        int currentSelected = 0;

        for (int i = 0; i < refreshRates.Count; i++)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(refreshRates[i].value.ToString("F2")));
            if (refreshRates[i].value == Screen.currentResolution.refreshRateRatio.value)
            {
                currentSelected = i;
            }
        }

        dropdown.SetValueWithoutNotify(currentSelected);

        dropdown.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(int arg0)
    {
        settingsMenu.ChangeRefreshRate(refreshRates[dropdown.value]);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
