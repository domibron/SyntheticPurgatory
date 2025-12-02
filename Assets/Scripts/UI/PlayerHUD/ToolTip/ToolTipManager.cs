using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class ToolTipQueItem
{
    public string TextToDisplay;
    public float TimeLeftToDisplay;
    public int Priority;

    public ToolTipQueItem(string text, float duration, int priority = 0)
    {
        TextToDisplay = text;
        TimeLeftToDisplay = duration;
        Priority = priority;
    }
}

public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager Instance { get; private set; }

    [SerializeField]
    TMP_Text tooltipText;

    [SerializeField]
    CanvasGroup canvasGroup;


    List<ToolTipQueItem> toolTipsToDisplay = new List<ToolTipQueItem>();

    [SerializeField]
    private float fadeTime = 0.3f;

    private float currentFadeTime = 0f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("There cannot be more than one tool tip manager! Overriding!");
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
        currentFadeTime = 0f;
        canvasGroup.alpha = currentFadeTime;
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     DisplayTooltip("Example text", 1f);
        // }

        if (toolTipsToDisplay.Count > 0)
        {
            tooltipText.text = toolTipsToDisplay[0].TextToDisplay;


            List<ToolTipQueItem> itemsToRemove = new List<ToolTipQueItem>();
            foreach (ToolTipQueItem item in toolTipsToDisplay)
            {
                item.TimeLeftToDisplay -= Time.deltaTime;

                if (item.TimeLeftToDisplay <= 0)
                {
                    itemsToRemove.Add(item);
                }
            }

            foreach (ToolTipQueItem item in itemsToRemove)
            {
                toolTipsToDisplay.Remove(item);
            }

            if (toolTipsToDisplay.Count > 0 && itemsToRemove.Count > 0) // reorganize when stuff is removed.
            {
                OrganizeQue();
            }
        }

        if (toolTipsToDisplay.Count > 0)
        {
            currentFadeTime += Time.deltaTime * (1f / fadeTime);
        }
        else
        {
            currentFadeTime -= Time.deltaTime * (1f / fadeTime);
        }

        currentFadeTime = Mathf.Clamp01(currentFadeTime);

        canvasGroup.alpha = currentFadeTime;
    }



    public bool DisplayTooltip(ToolTipQueItem toolTipQueItem)
    {
        if (toolTipsToDisplay.Count > 10 || ContainsText(toolTipQueItem.TextToDisplay)) return false;

        toolTipsToDisplay.Add(toolTipQueItem);
        OrganizeQue();
        return true;
    }

    public bool DisplayTooltip(string text, float duration, int priority = 0)
    {
        if (toolTipsToDisplay.Count > 10 || ContainsText(text)) return false;

        ToolTipQueItem toolTipQueItem = new ToolTipQueItem(text, duration, priority);

        toolTipsToDisplay.Add(toolTipQueItem);
        OrganizeQue();
        return true;
    }

    private bool ContainsText(string textToCheck)
    {
        foreach (ToolTipQueItem item in toolTipsToDisplay)
        {
            if (item.TextToDisplay == textToCheck) return true;
        }

        return false;
    }

    private void OrganizeQue()
    {
        toolTipsToDisplay.Sort(ComparePriorities);
    }

    private static int ComparePriorities(ToolTipQueItem x, ToolTipQueItem y)
    {
        if (x == null)
        {
            if (y == null)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
        else
        {
            if (y == null)
            {
                return 1;
            }
            else
            {
                if (x.Priority == y.Priority) return 0;
                else if (x.Priority > y.Priority) return -1;
                else return 1;
            }
        }
    }
}
