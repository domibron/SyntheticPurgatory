using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//by    _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|
// © 2025 domibron


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
