using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    private int depositedScrap = 0;

    public float TimePerLevel = 120f;

    private float currentTime = 1f;
    private bool inDungeon = false;

    [SerializeField]
    private int maxLives = 3;

    private int currentLives = 1;

    [SerializeField]
    private int commonUpgradeAmount = 2;
    [SerializeField]
    private int commonDowngradeAmount = 1;

    private int commonCards = 0;

    [SerializeField]
    private int rareUpgradeAmount = 4;
    [SerializeField]
    private int rareDowngradeAmount = 1;

    private int rareCards = 0;

    [SerializeField]
    private int epicUpgradeAmount = 5;
    [SerializeField]
    private int epicDowngradeAmount = 0;

    private int epicCards = 0;

    private int currentDifficulty = 0;

    private bool pause = false;

    private bool timerHidden = false;

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
        else if (inDungeon && currentTime <= 0 && !pause)
        {
            pause = true;
            PlayerRefFetcher.Instance?.GetPlayerRef()?.GetComponent<PlayerDeath>()?.KillPlayer();
        }
    }

    public (int, int) GetUPandDOWNAmounts(CardTier cardTeir)
    {
        switch (cardTeir)
        {
            case CardTier.Common:
                return (commonUpgradeAmount, commonDowngradeAmount);
            case CardTier.Rare:
                return (rareUpgradeAmount, rareDowngradeAmount);
            case CardTier.Epic:
                return (epicUpgradeAmount, epicDowngradeAmount);
            default:
                return (1, 0);
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
        pause = false;
    }

    public void ResetTimer()
    {
        inDungeon = false;
        timerHidden = false;
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
            commonCards += UpgradeCardManager.Instance.GetAllCardCountOfType(CardTier.Common);
            rareCards += UpgradeCardManager.Instance.GetAllCardCountOfType(CardTier.Rare);
            epicCards += UpgradeCardManager.Instance.GetAllCardCountOfType(CardTier.Epic);
        }
        ResetTimer();
        currentDifficulty++;
        LevelLoading.Instance.LoadScene(LevelCollection.LevelKey.HubWorld.ToString());
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }
    public void EndRun()
    {

    }

    public int GetCurrentDifficlty()
    {
        return currentDifficulty;
    }

    public int GetCardCount(CardTier cardTeir)
    {
        switch (cardTeir)
        {
            case CardTier.Common:
                return commonCards;
            case CardTier.Rare:
                return rareCards;
            case CardTier.Epic:
                return epicCards;
            default:
                return 0;
        }
    }

    public void RemoveFromStoredCards(CardTier cardTeir, int amountToTake)
    {
        switch (cardTeir)
        {
            case CardTier.Common:
                commonCards -= amountToTake;
                break;
            case CardTier.Rare:
                rareCards -= amountToTake;
                break;
            case CardTier.Epic:
                epicCards -= amountToTake;
                break;
        }
    }

    public void AddToStoredCards(CardTier cardTeir, int amountToAdd)
    {
        switch (cardTeir)
        {
            case CardTier.Common:
                commonCards += amountToAdd;
                break;
            case CardTier.Rare:
                rareCards += amountToAdd;
                break;
            case CardTier.Epic:
                epicCards += amountToAdd;
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

    public void HideTimer()
    {
        timerHidden = true;
    }

    public bool IsTimerHidden()
    {
        return timerHidden;
    }
}
