using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;



public class ScrapManager : MonoBehaviour
{
    public SO_ScrapWithWorth ScrapPrefabsWithWorth;
    public SO_ScrapWithWorth DepositScrapWithWorth;

    public static ScrapManager Instance { get; private set; }

    // public CollectableStatsSO collectableStats;

    // public float MaxCollectionRange = 5f; // TODO: create a stats class that can be publicly access and read from and not editable during gameplay.
    // public float CollectItemRange = 1f;

    // public float FlyAccel = 15f;
    // public float FlyMaxSpeed = 30f;
    // public float FlyDistanceBoost = 10f;

    // public float DepositRate = 0.5f;

    private int maxInventoryScrap = 0;

    // [SerializeField]
    // int maxInventoryScrap = 10;
    public int currentInventoryScrap = 0;

    public int currentDepositedScrap = 0;

    public event Action<int> collectedScrap;
    public event Action<int> droppedScrap;
    public event Action<int> depositedScrap;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"Detected multiple {nameof(ScrapManager)}, please make sure only one exsits at any given time.");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GameManager.Instance != null)
        {
            MiscellaneousStats collectableStats = GameStatsManager.Instance.GetStats<MiscellaneousStats>(Stats.miscellaneous);

            if (collectableStats == null)
            {
                Debug.LogError("Collectable stats are null?!", this);
                // maxInventoryScrap = new CollectableStats().MaxInventoryScrap;
                collectableStats = new();
            }

            maxInventoryScrap = collectableStats.MaxInventoryScrap;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // TODO: remove.
        {
            GameManager.Instance.ReturnToHubWorld();
        }
    }

    // * This is static.
    public static ScrapItemData GetPrefabWithHighestWorth(int worthNeeded, SO_ScrapWithWorth scrapWithWorthSO)
    {
        if (worthNeeded <= 0) return null;

        ReadOnlyCollection<ScrapItemData> scrapItems = scrapWithWorthSO.ScrapItemData;

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

    public void DepositScrap(int amount)
    {
        if (currentInventoryScrap <= 0 || amount <= 0) return;

        currentInventoryScrap -= amount;
        currentDepositedScrap += amount;


        InvokeDepositedScrap(amount);
        // code goes here.
    }

    public int GetAllDepositedScrap()
    {
        int leftOvers = currentInventoryScrap;
        currentInventoryScrap = 0;

        // GameManager.Instance.AddToDepositedScrap(currentDepositedScrap + leftOvers);
        return currentDepositedScrap + leftOvers;
    }

    public int GetScrapInInventory()
    {
        return currentInventoryScrap;
    }

    void InvokeCollectedScrap(int amount)
    {
        collectedScrap?.Invoke(amount);
    }

    public GameObject SpawnScrap(int worth, Vector3 pos)
    {
        if (worth < 0) worth = Mathf.Abs(worth);
        else if (worth == 0) worth = 1;

        ScrapItemData scrapData = GetPrefabWithHighestWorth(worth, ScrapPrefabsWithWorth);

        GameObject scrapItem = Instantiate(scrapData.ScrapPrefab, pos, Quaternion.identity);
        scrapItem.GetComponent<ScrapCollectable>()?.Initialize(scrapData.ScrapWorth);

        return scrapItem;
    }

    void InvokeDroppedScrap(int amount)
    {
        droppedScrap?.Invoke(amount);
    }

    void InvokeDepositedScrap(int amount)
    {
        depositedScrap?.Invoke(amount);
    }
}
