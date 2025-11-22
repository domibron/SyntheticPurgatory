using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetVolume : MonoBehaviour
{

    [SerializeField]
    SettingsMenu settingsMenu;

    [SerializeField]
    string mixerVarName;

    private Slider slider;

    [SerializeField]
    TMP_Text sliderValueText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider = GetComponent<Slider>();

        slider.onValueChanged.AddListener(OnSliderValueChanged);

        slider.SetValueWithoutNotify(settingsMenu.GetVolumeValue(mixerVarName));
        // print(settingsMenu.GetVolumeValue(mixerVarName));

        sliderValueText.text = (slider.value * 100f).ToString("F0");
    }

    private void OnSliderValueChanged(float arg0)
    {
        settingsMenu.SetVolume(mixerVarName, slider.value);
        sliderValueText.text = (slider.value * 100f).ToString("F0");
        // print(settingsMenu.GetVolumeValue(mixerVarName));

    }

    // Update is called once per frame
    void Update()
    {

    }
}
