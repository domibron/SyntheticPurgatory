using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeFPSCap : MonoBehaviour
{
    [SerializeField]
    SettingsMenu settingsMenu;

    Slider slider;

    [SerializeField]
    TMP_Text fpsText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider = GetComponent<Slider>();

        slider.SetValueWithoutNotify(Application.targetFrameRate);
        fpsText.text = (Application.targetFrameRate == -1 ? "NONE" : Application.targetFrameRate.ToString());

        slider.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(float arg0)
    {
        fpsText.text = (slider.value == 29 ? "NONE" : ((int)slider.value).ToString());
        settingsMenu.SetTargetFrameRate(slider.value == 29 ? -1 : (int)slider.value);
    }
}
