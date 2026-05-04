using System;
using UnityEngine;

public class InventoryUIPopupHooks : MonoBehaviour
{

    private PopupManager popupManager;
    private ToolTipManager toolTipManager;

    private float invFullSpamProtect = 0f;

    private float invFullSpamWait = 5f;

    private float lowTimeSpamProtect = 0f;

    private float lowTimeSpamWait = 10f;

    private float duration = 3f;

    private ScrapManager scrapManager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        popupManager = PopupManager.Instance;
        toolTipManager = ToolTipManager.Instance;

        if (GameManager.Instance == null)
        {
            Debug.LogWarning("Cannot find game manager!");
            return;
        }

        scrapManager = ScrapManager.Instance;

        scrapManager.OnDepositedScrap += OnDepositedScrap;
        scrapManager.OnCollectedScrap += OnCollectedScrap;
        scrapManager.OnInventoryFull += OnInventoryFull;


        ModuleManager.Instance.OnModuleCollected += OnModuleCollected;
        ModuleManager.Instance.OnModuleDeposited += OnModuleDeposited;

        GameManager.Instance.OnLowTime += OnLowTime;
        GameManager.Instance.OnWarnTime += OnWarnTime;
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

        popupManager.DisplayText($"You have {obj.ToString("F0")} seconds left!", "warn", duration);
        toolTipManager.DisplayTooltip($"<color=red>{obj.ToString("F0")} seconds left!</color>", duration, 10);
    }

    private void OnWarnTime(float obj)
    {
        if (lowTimeSpamProtect > 0) return;

        lowTimeSpamProtect = lowTimeSpamWait;

        int time = Mathf.FloorToInt(obj) + 1;


        popupManager.DisplayText($"You have {(time / 60).ToString() + ":" + ((time % 60f) < 10 ? "0" : "") + (time % 60f).ToString("F0")} left!", "warn", duration);
        toolTipManager.DisplayTooltip($"<color=yellow>{(time / 60).ToString() + ":" + ((time % 60f) < 10 ? "0" : "") + (time % 60f).ToString("F0")} left!</color>", duration, 10);
    }

    private void OnModuleDeposited(ModuleTier tier, int arg2)
    {
        throw new NotImplementedException();
    }

    private void OnModuleCollected(ModuleTier tier, int arg2)
    {
        throw new NotImplementedException();
    }

    private void OnInventoryFull()
    {
        if (invFullSpamProtect > 0) return;

        invFullSpamProtect = invFullSpamWait;

        popupManager.DisplayText($"Inventory is full! <b>({scrapManager.GetScrapInInventory()}/{scrapManager.GetMaxScrapInventory()})</b>", "warn", duration);
        toolTipManager.DisplayTooltip($"<color=red>Inventory is full!</color>", duration, 5);
    }

    private void OnCollectedScrap(int obj)
    {
        popupManager.DisplayText($"+{obj.ToString()} scrap collected", "scrap");
    }

    private void OnDepositedScrap(int obj)
    {
        popupManager.DisplayText($"↓{obj.ToString()} scrap deposited", "depo");
    }


}
