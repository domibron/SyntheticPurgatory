using System;
using UnityEngine;

public class InventoryUIPopupHooks : MonoBehaviour
{
    private float invFullSpamProtect = 0f;

    private float invFullSpamWait = 5f;

    private float lowTimeSpamProtect = 0f;

    private float lowTimeSpamWait = 10f;

    private float duration = 3f;

    private ScrapLevelM scrapManager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (RunManager.Instance == null)
        {
            Debug.LogWarning("Cannot find game manager!");
            return;
        }

        scrapManager = ScrapLevelM.Instance;

        scrapManager.OnDepositedScrap += OnDepositedScrap;
        scrapManager.OnCollectedScrap += OnCollectedScrap;
        scrapManager.OnInventoryFull += OnInventoryFull;


        ModuleLevelM.Instance.OnModuleCollected += OnModuleCollected;
        ModuleLevelM.Instance.OnModuleDeposited += OnModuleDeposited;

        RunManager.Instance.OnLowTime += OnLowTime;
        RunManager.Instance.OnWarnTime += OnWarnTime;
    }

    void Update()
    {
        if (invFullSpamProtect >= 0) invFullSpamProtect -= Time.deltaTime;
        if (lowTimeSpamProtect >= 0) lowTimeSpamProtect -= Time.deltaTime;
    }

    private void OnLowTime(float obj)
    {
        if (lowTimeSpamProtect > 0) return;

        lowTimeSpamProtect = lowTimeSpamWait;

        PopupManager.Instance?.DisplayText($"You have {obj.ToString("F0")} seconds left!", "warn", duration);
        ToolTipManager.Instance?.DisplayTooltip($"<color=red>{obj.ToString("F0")} seconds left!</color>", duration, 10);
    }

    private void OnWarnTime(float obj)
    {
        if (lowTimeSpamProtect > 0) return;

        lowTimeSpamProtect = lowTimeSpamWait;

        int time = Mathf.FloorToInt(obj) + 1;


        PopupManager.Instance?.DisplayText($"You have {(time / 60).ToString() + ":" + ((time % 60f) < 10 ? "0" : "") + (time % 60f).ToString("F0")} left!", "warn", duration);
        ToolTipManager.Instance?.DisplayTooltip($"<color=yellow>{(time / 60).ToString() + ":" + ((time % 60f) < 10 ? "0" : "") + (time % 60f).ToString("F0")} left!</color>", duration, 10);
    }

    private void OnModuleDeposited(ModuleTier tier, int arg2)
    {

    }

    private void OnModuleCollected(ModuleTier tier, int arg2)
    {

    }

    private void OnInventoryFull()
    {
        if (invFullSpamProtect > 0) return;

        invFullSpamProtect = invFullSpamWait;

        PopupManager.Instance?.DisplayText($"Inventory is full! <b>({scrapManager.CurrentInventoryScrap}/{scrapManager.GetMaxScrapInventory()})</b>", "warn", duration);
        ToolTipManager.Instance?.DisplayTooltip($"<color=red>Inventory is full!</color>", duration, 5);
    }

    private void OnCollectedScrap(int obj)
    {
        PopupManager.Instance?.DisplayText($"+{obj.ToString()} scrap collected", "scrap");
    }

    private void OnDepositedScrap(int obj)
    {
        PopupManager.Instance?.DisplayText($"↓{obj.ToString()} scrap deposited", "depo");
    }


}
