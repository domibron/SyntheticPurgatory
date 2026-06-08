using System;
using TMPro;
using UnityEngine;


public class VersionTag : MonoBehaviour
{
    public TMP_Text uiText;
    public string AdditionalText = "ALPHA";
    public string PrefixVersionText = "";
    public string SuffixVersionText = "";

    void OnValidate()
    {
        if (GetComponent<TMP_Text>() != null)
        {
            uiText = GetComponent<TMP_Text>();
        }

        if (uiText != null)
        {
            uiText.text = GetText();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        uiText.text = GetText();
    }

    string GetText()
    {
        return $"{PrefixVersionText}V{Application.version}{SuffixVersionText}" + (String.IsNullOrWhiteSpace(AdditionalText) ? "" : "\n" + AdditionalText);
    }
}
