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
    [SerializeField]
    TMP_Text tooltipText;

    [SerializeField]
    CanvasGroup canvasGroup;


    List<ToolTipQueItem> toolTipsToDisplay = new List<ToolTipQueItem>();



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (toolTipsToDisplay.Count > 0)
        {
            tooltipText.text = toolTipsToDisplay[0].TextToDisplay;
            canvasGroup.alpha = 1f;

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
        else
        {
            canvasGroup.alpha = 0f;
        }
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
                else if (x.Priority > y.Priority) return 1;
                else return -1;
            }
        }
    }
}
