using System;
using UnityEngine;

[Serializable]
public class PopupIcon
{
    public string Name;
    public Sprite Icon;
}

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    [SerializeField]
    private GameObject popupTextPrefab;

    [SerializeField]
    private PopupIcon[] popupIcons;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("There cannot be more than one popup manager! Overriding!");
            Instance = this;
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnPopupText("Some stinky message");
        }
    }

    public void DisplayText(string text, string iconName)
    {
        SpawnPopupText(text, GetSpriteFromIconName(iconName)); // yeah yeah, double functions. I will ponder later.
    }

    private Sprite GetSpriteFromIconName(string name)
    {
        foreach (var icon in popupIcons)
        {
            if (icon.Name == name)
            {
                return icon.Icon;
            }
        }

        return null;
    }

    public void DisplayText(string text, Sprite sprite = null)
    {
        SpawnPopupText(text, sprite); // yeah yeah, double functions. I will ponder later.
    }

    private void SpawnPopupText(string text, Sprite sprite = null)
    {
        PopupInfoText popupInfoText = Instantiate(popupTextPrefab, transform).GetComponent<PopupInfoText>();

        popupInfoText.Initialize(text, sprite);

    }
}
