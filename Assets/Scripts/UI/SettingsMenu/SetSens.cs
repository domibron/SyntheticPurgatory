using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetSens : MonoBehaviour
{

    [SerializeField]
    bool isGamepad = false;

    [SerializeField]
    bool isHorizontal = true;

    [SerializeField]
    TMP_Text sliderText;

    private Slider slider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider = GetComponent<Slider>();

        slider.SetValueWithoutNotify((isHorizontal ? (isGamepad ? SettingsMenu.GetGamepadXSens() : SettingsMenu.GetMouseXSens())
         : (isGamepad ? SettingsMenu.GetGamepadYSens() : SettingsMenu.GetGamepadYSens())) * 10f);

        sliderText.text = (slider.value / 10f).ToString("F1");

        slider.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(float arg0)
    {
        if (isGamepad)
        {
            if (isHorizontal)
                SettingsMenu.SetGamepadXSens(slider.value / 10f);
            else
                SettingsMenu.SetGamepadYSens(slider.value / 10f);
        }
        else
        {
            if (isHorizontal)
                SettingsMenu.SetMouseXSens(slider.value / 10f);
            else
                SettingsMenu.SetMouseYSens(slider.value / 10f);
        }

        sliderText.text = (slider.value / 10f).ToString("F1");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
