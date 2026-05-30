using System;
using System.Collections.ObjectModel;
using UnityEngine;


/// <summary>
/// Scrap manager for the level, handles storing scrap into the temporary inventory, getting scrap prefabs and some events.
/// </summary>
public class ScrapLevelM : MonoBehaviour
{
    /// <summary>
    /// Scrap prefabs with worth for use of spawning in the level.
    /// </summary>
    public SO_ScrapWithWorth ScrapPrefabsWithWorth;

    /// <summary>
    /// Deposit / dummy scrap for use of displaying scrap that is not collectable.
    /// </summary>
    public SO_ScrapWithWorth DepositScrapWithWorth;

    /// <summary>
    /// Singleton for the scrap manager.
    /// </summary>
    public static ScrapLevelM Instance { get; private set; }

    /// <summary>
    /// The maximum scrap the player can carry in their inventory. Stats are assigned to this.
    /// </summary>
    private int maxInventoryScrap = 0;

    /// <summary>
    /// The current amount of scrap in the inventory.
    /// </summary>
    public int currentInventoryScrap = 0;

    /// <summary>
    /// The scrap that is deposited. This will be moved into the game manager at the end of the run.
    /// </summary>
    public int currentDepositedScrap = 0;

    /// <summary>
    /// When scrap is collected by the player.
    /// </summary>
    public event Action<int> OnCollectedScrap;

    /// <summary>
    /// When the player drops scrap.
    /// </summary>
    public event Action<int> OnRemovedScrap;

    /// <summary>
    /// When the player deposits the scrap into a depot.
    /// </summary>
    public event Action<int> OnDepositedScrap;

    /// <summary>
    /// Event for when the player is full and tries to pick up scrap.
    /// </summary>
    public event Action OnInventoryFull;



    /// <summary>
    /// Current timer to hold before invoking the <see cref="OnCollectedScrap"/> event.
    /// </summary>
    private float scrapCollectHoardInfoTimer = 0f;

    /// <summary>
    /// <see cref="OnCollectedScrap"/> invoke wait duration.
    /// </summary>
    private float scrapCollectHoardDuration = 0.5f;

    /// <summary>
    /// The final amount of scrap collected after collection has paused for the specified duration.
    /// </summary>
    private int finalCollectAmount = 0;



    /// <summary>
    /// The current timer to hold off invoking the <see cref="OnDepositedScrap"/> event.
    /// </summary>
    private float scrapDepoHoardInfoTimer = 0f;

    /// <summary>
    /// How long to wait before invoking the <see cref="OnDepositedScrap"/> event.
    /// </summary>
    private float scrapDepoHoardDuration = 0.5f;

    /// <summary>
    /// The final value of the deposited scrap after depositing has paused for the specified time.
    /// </summary>
    private int finalDepoAmount = 0;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"Detected multiple {nameof(ScrapLevelM)}, please make sure only one exsits at any given time.");
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
        if (RunManager.Instance != null)
        {
            MiscellaneousStats collectableStats = RunStatsManager.Instance.GetStats<MiscellaneousStats>(Stats.miscellaneous);

            if (collectableStats == null)
            {
                Debug.LogError("Collectable stats are null?!", this);
                // maxInventoryScrap = new CollectableStats().MaxInventoryScrap;
                collectableStats = new();
            }

            maxInventoryScrap = Mathf.FloorToInt(collectableStats.MaxInventoryScrapStat.GetCurrentValue());
        }

        scrapCollectHoardInfoTimer = scrapCollectHoardDuration;
        scrapDepoHoardInfoTimer = scrapDepoHoardDuration;
    }

    // Update is called once per frame
    void Update()
    {
        // Just the invoking delay.

        if (finalCollectAmount > 0 && scrapCollectHoardInfoTimer >= scrapCollectHoardDuration)
        {
            InvokeCollectedScrap(finalCollectAmount);
            finalCollectAmount = 0;
        }
        else if (scrapCollectHoardInfoTimer < scrapCollectHoardDuration)
        {
            scrapCollectHoardInfoTimer += Time.deltaTime;
        }

        if (finalDepoAmount > 0 && scrapDepoHoardInfoTimer >= scrapDepoHoardDuration)
        {
            InvokeDepositedScrap(finalDepoAmount);
            finalDepoAmount = 0;
        }
        else if (scrapDepoHoardInfoTimer < scrapDepoHoardDuration)
        {
            scrapDepoHoardInfoTimer += Time.deltaTime;
        }
    }

    // * This is static.
    /// <summary>
    /// Get the highest scrap with the provided value. If there are 3 scrap worths 1, 5 and 15, and you input 7, it will return the 5 worth.
    /// </summary>
    /// <param name="worthNeeded">The value you need to get scrap representation for.</param>
    /// <param name="scrapWithWorthSO">The table to look up at.</param>
    /// <returns>The scrap data with the prefab and worth.</returns>
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

    /// <summary>
    /// Check to see if there is room left in the inventory.
    /// </summary>
    /// <returns>True if there is space.</returns>
    public bool IsSpaceInScrapInv()
    {
        return currentInventoryScrap < maxInventoryScrap;
    }

    /// <summary>
    /// Get the maximum scrap carrying capacity.
    /// </summary>
    /// <returns>The inventory max.</returns>
    public int GetMaxScrapInventory()
    {
        return maxInventoryScrap;
    }

    /// <summary>
    /// Get the remaining space in the inventory before reaching max.
    /// </summary>
    /// <returns>The remaining space in the inventory.</returns>
    public int GetRemainingScrapInvSpace()
    {
        return maxInventoryScrap - currentInventoryScrap;
    }

    /// <summary>
    /// Collect the scrap amount and add it to the temporary inventory.
    /// </summary>
    /// <param name="amount">The amount to add to the inventory.</param>
    /// <returns>The remaining amount left over if there is overflow.</returns>
    public int CollectScrap(int amount)
    {
        if (currentInventoryScrap >= maxInventoryScrap) return amount;

        int remainder = (currentInventoryScrap + amount) - maxInventoryScrap;

        currentInventoryScrap += (amount - Mathf.Max(remainder, 0));

        // ! ~ What? The event?
        // InvokeCollectedScrap(amount);
        if (finalCollectAmount <= 0)
        {
            scrapCollectHoardInfoTimer = 0f;
        }

        finalCollectAmount += amount;


        return Mathf.Max(remainder, 0);
    }

    /// <summary>
    /// Remove the scrap from the inventory.
    /// </summary>
    /// <param name="amount">The amount of scrap to remove from the inventory.</param>
    public void RemoveScrapFromInv(int amount)
    {
        if (currentInventoryScrap <= 0) return;

        currentInventoryScrap -= amount;

        // Prevent negative, you cannot carry negative scrap.
        if (currentInventoryScrap < 0) currentInventoryScrap = 0;

        InvokeDroppedScrap(amount);
    }

    /// <summary>
    /// Deposit scrap into the deposit inventory container. Not the game manager container.
    /// </summary>
    /// <param name="amount">The amount to deposit.</param>
    public void DepositScrap(int amount)
    {
        if (currentInventoryScrap <= 0 || amount <= 0) return;

        currentInventoryScrap -= amount;
        currentDepositedScrap += amount;


        // InvokeDepositedScrap(amount);
        if (finalDepoAmount <= 0)
        {
            scrapCollectHoardInfoTimer = 0f;
        }

        finalDepoAmount += amount;
    }

    /// <summary>
    /// Get all the scrap this run, both deposited and inventory.
    /// </summary>
    /// <returns>The total amount of scrap that was collected.</returns>
    public int GetAllCollectedScrap()
    {
        int leftOvers = currentInventoryScrap;
        currentInventoryScrap = 0; // stop any depositing to prevent duplication.

        // GameManager.Instance.AddToDepositedScrap(currentDepositedScrap + leftOvers);
        return currentDepositedScrap + leftOvers;
    }

    /// <summary>
    /// Get the scrap currently in the inventory.
    /// </summary>
    /// <returns>The scrap in the inventory of the player.</returns>
    public int GetScrapInInventory()
    {
        return currentInventoryScrap;
    }

    /// <summary>
    /// Get all the scrap that was deposited this run.
    /// </summary>
    /// <returns>The amount of scrap deposited.</returns>
    public int GetDepositedScrap()
    {
        return currentDepositedScrap;
    }


    /// <summary>
    /// Spawn in a scrap object that the player can collect.
    /// </summary>
    /// <param name="worth">The value of scrap to spawn. (will get the largest scrap that can fit that value)</param>
    /// <param name="pos">The spawn location for the scrap.</param>
    /// <returns>The reference to the scrap object that was spawned.</returns>
    public GameObject SpawnScrap(int worth, Vector3 pos)
    {
        if (worth < 0) worth = Mathf.Abs(worth);
        else if (worth == 0) worth = 1;

        ScrapItemData scrapData = GetPrefabWithHighestWorth(worth, ScrapPrefabsWithWorth);

        GameObject scrapItem = Instantiate(scrapData.ScrapPrefab, pos, Quaternion.identity);
        scrapItem.GetComponent<ScrapCollectable>()?.Initialize(scrapData.ScrapWorth);

        return scrapItem;
    }

    /// <summary>
    /// Invokes the <see cref="OnCollectedScrap"/> event.
    /// </summary>
    /// <param name="amount">The amount that was collected.</param>
    void InvokeCollectedScrap(int amount)
    {
        OnCollectedScrap?.Invoke(amount);
    }

    /// <summary>
    /// Invokes the <see cref="OnRemovedScrap"/> event.
    /// </summary>
    /// <param name="amount">The amount that was dropped.</param>
    void InvokeDroppedScrap(int amount)
    {
        OnRemovedScrap?.Invoke(amount);
    }

    /// <summary>
    /// Invokes the <see cref="OnDepositedScrap"/> event.
    /// </summary>
    /// <param name="amount">The amount that was deposited.</param>
    void InvokeDepositedScrap(int amount)
    {
        OnDepositedScrap?.Invoke(amount);
    }


    /// <summary>
    /// Invokes the <see cref="OnInventoryFull"/> event.
    /// </summary>
    public void InvokeOnInventoryFull()
    {
        OnInventoryFull?.Invoke();
    }
}
