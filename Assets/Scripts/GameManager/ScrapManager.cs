using System;
using UnityEngine;



public class ScrapManager : MonoBehaviour
{
    public ScrapWithWorthSO ScrapPrefabsWithWorth;

    public static ScrapManager Instance { get => instance; }
    static ScrapManager instance;

    public const float MaxCollectionRange = 5f; // TODO create a stats class that can be publicly access and read from and not editable during gameplay.
    public const float CollectItemRange = 1f;

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
