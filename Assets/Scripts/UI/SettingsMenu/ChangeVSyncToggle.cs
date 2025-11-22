using UnityEngine;
using UnityEngine.UI;

public class ChangeVSyncToggle : MonoBehaviour
{
    [SerializeField]
    SettingsMenu settingsMenu;

    Toggle toggle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        toggle = GetComponent<Toggle>();

        toggle.SetIsOnWithoutNotify(QualitySettings.vSyncCount > 0);


        toggle.onValueChanged.AddListener(InformSettingsAboutVSyncChange);
    }

    void InformSettingsAboutVSyncChange(bool value)
    {
        settingsMenu.SetVSyncEnabled(value);
    }
}
