using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToolTipTextDisplay : MonoBehaviour
{
    public static ToolTipTextDisplay Instance { get; private set; }

    [SerializeField]
    GameObject toolTipObject;

    [SerializeField]
    TMP_Text displayText;

    [SerializeField]
    string prefixText = "<b>Info:</b>\n";

    bool showToolTip = false;
    string currentText;

    float lastUpdateTime = 0f;

    const float waitTime = 0.1f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        else
        {
            Instance = this;
        }

        toolTipObject.SetActive(false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (showToolTip && lastUpdateTime <= 0) showToolTip = false;
        else if (showToolTip && lastUpdateTime > 0) lastUpdateTime -= Time.deltaTime;
        else if (!showToolTip && lastUpdateTime > 0) showToolTip = true;

        if (toolTipObject.activeSelf != showToolTip) toolTipObject.SetActive(showToolTip);

        displayText.text = prefixText + currentText;
    }

    public void SetDisplayText(string text)
    {
        lastUpdateTime = waitTime;
        currentText = text;
        if (!showToolTip) showToolTip = true;
    }

    public void HideToolTip()
    {
        lastUpdateTime = 0;
        if (showToolTip) showToolTip = false;
    }
}
