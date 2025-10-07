using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;



public class ScrapManager : MonoBehaviour
{
    public ScrapWithWorthSO ScrapPrefabsWithWorth;

    public static ScrapManager Instance { get => instance; }
    static ScrapManager instance;

    public float MaxCollectionRange = 5f; // TODO create a stats class that can be publicly access and read from and not editable during gameplay.
    public float CollectItemRange = 1f;

    public float flyAccel = 15f;
    public float flyMaxSpeed = 30f;
    public float flyDistanceBoost = 10f;

    [SerializeField]
    int maxInventoryScrap = 10;
    int currentInventoryScrap = 0;

    public event Action<int> collectScrap;
    public event Action<int> droppedScrap;


    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning($"Detected multiple {nameof(ScrapManager)}, please make sure only one exsits at any given time.");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public ScrapItemData GetPrefabWithHighestWorth(int worthNeeded)
    {
        if (worthNeeded <= 0) return null;

        ReadOnlyCollection<ScrapItemData> scrapItems = ScrapPrefabsWithWorth.ScrapItemData;

        ScrapItemData scrapItem = new ScrapItemData();

        for (int i = 0; i < scrapItems.Count; i++)
        {
            if (i == 0) // TODO better checking that the first item can work to spawn.
            {
                scrapItem.ScrapWorth = scrapItems[i].ScrapWorth;
                scrapItem.ScrapPrefab = scrapItems[i].ScrapPrefab;
                continue;
            }

            if (scrapItems[i].ScrapWorth <= worthNeeded && scrapItems[i].ScrapWorth > scrapItem.ScrapWorth)
            {
                scrapItem.ScrapWorth = scrapItems[i].ScrapWorth;
                scrapItem.ScrapPrefab = scrapItems[i].ScrapPrefab;
            }
        }

        return scrapItem;
    }

    public bool HaveInventorySpace()
    {
        return currentInventoryScrap < maxInventoryScrap;
    }

    public int HowMuchCanInventoryHold()
    {
        return maxInventoryScrap - currentInventoryScrap;
    }

    public int CollectScrap(int amount)
    {
        if (currentInventoryScrap >= maxInventoryScrap) return amount;

        int remainder = (currentInventoryScrap + amount) - maxInventoryScrap;

        currentInventoryScrap += (amount - Mathf.Max(remainder, 0));
        InvokeCollectedScrap(amount);

        return Mathf.Max(remainder, 0);
    }

    public void DropScrap(int amount)
    {
        if (currentInventoryScrap <= 0) return;

        currentInventoryScrap -= amount;

        InvokeDroppedScrap(amount);
    }

    public int HowMuchSpaceLeft()
    {
        return maxInventoryScrap - currentInventoryScrap;
    }

    public void DepositScrap()
    {
        if (currentInventoryScrap <= 0) return;

        // code goes here.
    }

    public int GetScrapInInventory()
    {
        return currentInventoryScrap;
    }

    void InvokeCollectedScrap(int amount)
    {
        collectScrap?.Invoke(amount);
    }

    void InvokeDroppedScrap(int amount)
    {
        droppedScrap?.Invoke(amount);
    }
}
