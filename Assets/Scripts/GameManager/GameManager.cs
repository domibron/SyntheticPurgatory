using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    private int depositedScrap = 0;

    public float TimePerLevel = 120f;

    private float currentTime = 1f;
    private bool inDungeon = false;
    private int commonCards = 0;
    private int rareCards = 0;
    private int epicCards = 0;

    [SerializeField]
    private int maxLives = 3;

    private int currentLives = 1;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
            currentLives = maxLives;
        }
    }

    void Update()
    {
        if (inDungeon && currentTime > 0)
        {
            currentTime -= Time.deltaTime;
        }
    }

    public void AddToDepositedScrap(int amount)
    {
        depositedScrap += amount;
    }

    public void RemoveFromDepositedScrap(int amount)
    {
        depositedScrap -= amount;
    }

    public int GetCurrentScrapCount()
    {
        return depositedScrap;
    }

    public void StartTimer()
    {
        currentTime = TimePerLevel;
        inDungeon = true;
    }

    public void ResetTimer()
    {
        inDungeon = false;
    }

    public void ReturnToHubWorld(bool playerDied = false)
    {
        // is player dead?
        // minus lives
        // else
        // deposit scrap
        if (playerDied)
        {
            // remove from lives.
            currentLives--; // you fucked up, get destroyed.
            if (currentLives <= 0)
            {
                // End run
                EndRun();
            }
        }
        else
        {
            AddToDepositedScrap(ScrapManager.Instance.GetAllDepositedScrap());

            // so bad, but fuck it.
            commonCards += UpgradeCardManager.Instance.GetAllCardCountOfType(CardTeir.Common);
            rareCards += UpgradeCardManager.Instance.GetAllCardCountOfType(CardTeir.Rare);
            epicCards += UpgradeCardManager.Instance.GetAllCardCountOfType(CardTeir.Epic);
        }
        ResetTimer();
        LevelLoading.Instance.LoadScene(LevelCollection.LevelKey.HubWorld.ToString());
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }
    public void EndRun()
    {

    }

    public int GetCardCount(CardTeir cardTeir)
    {
        switch (cardTeir)
        {
            case CardTeir.Common:
                return commonCards;
            case CardTeir.Rare:
                return rareCards;
            case CardTeir.Epic:
                return epicCards;
            default:
                return 0;
        }
    }

    public void RemoveFrom(CardTeir cardTeir, int amountToTake)
    {
        switch (cardTeir)
        {
            case CardTeir.Common:
                commonCards -= amountToTake;
                break;
            case CardTeir.Rare:
                rareCards -= amountToTake;
                break;
            case CardTeir.Epic:
                epicCards -= amountToTake;
                break;
        }
    }

    public int GetCurrentLives()
    {
        return currentLives;
    }

    public int GetMaxLives()
    {
        return maxLives;
    }

}
