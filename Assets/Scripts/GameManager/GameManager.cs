using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    private int worldSeed = -1;
    private int levelSeed = -1;


    private int depositedScrap = 0;

    private float timePerLevel = 120f;

    private float currentTime = 1f;
    private bool inDungeon = false;

    private int currentLevel = 0;

    [SerializeField]
    private int maxLives = 3;

    private int currentLives = 1;

    // TODO: turn this either into structs or classes. maybe...
    [SerializeField]
    private int commonBaseUnlockCost = 200;

    [SerializeField]
    private int commonCostIncrease = 25;

    private int commonCurrentCost = 0;


    private int commonCards = 0;


    [SerializeField]
    private int rareBaseUnlockCost = 400;

    [SerializeField]
    private int rareCostIncrease = 50;

    private int rareCurrentCost = 0;


    private int rareCards = 0;


    [SerializeField]
    private int epicBaseUnlockCost = 600;

    [SerializeField]
    private int epicCostIncrease = 100;

    private int epicCurrentCost = 0;


    private int epicCards = 0;

    private int currentDifficulty = 0;

    private bool pause = false;

    private bool timerHidden = false;

    private bool lowTimeEventCalled = false;
    public event Action<float> OnLowTime;
    public event Action<float> OnWarnTime;

    private float lowTime = 60f;

    public StatsHolder statsHolder = new StatsHolder();


    void Awake()
    {
        // yeah, no, this is wrong. We need to destroy the other one.
        if (GameManager.Instance != null && GameManager.Instance != this)
        {
            // Destroy(this);
            Destroy(GameManager.Instance.gameObject); // Terminate the other game manager since it was brought over from a prev game.
            print("Other game manager has been removed!");
        }

        Instance = this;
        DontDestroyOnLoad(this);
        currentLives = maxLives;

        commonCurrentCost = commonBaseUnlockCost;
        rareCurrentCost = rareBaseUnlockCost;
        epicCurrentCost = epicBaseUnlockCost;
    }

    void Start()
    {
        timePerLevel = GameStatsManager.Instance.GetStats<MiscellaneousStats>(Stats.miscellaneous).MaxLevelTimeStat.GetCurrentValue();
        StartCoroutine(TrackGameTime());
    }

    void Update()
    {
        if (inDungeon && currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            if (currentTime < lowTime) InvokeLowTime(); // yes, spam the thing!!!
        }
        else if (inDungeon && currentTime <= 0 && !pause)
        {
            pause = true;
            PlayerRefFetcher.Instance?.GetPlayerRef()?.GetComponent<PlayerDeath>()?.KillPlayer();
        }
    }

    private void InvokeLowTime()
    {
        // if (lowTimeEventCalled) return;

        // lowTimeEventCalled = true;
        OnLowTime?.Invoke(currentTime);
    }

    public void InvokeRemindTime()
    {
        OnWarnTime?.Invoke(currentTime);
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
        // should be impossible but just in case.
        timePerLevel = GameStatsManager.Instance.GetStats<MiscellaneousStats>(Stats.miscellaneous).MaxLevelTimeStat.GetCurrentValue();

        currentTime = timePerLevel;
        inDungeon = true;
        pause = false;
    }

    public void ResetTimer()
    {
        inDungeon = false;
        timerHidden = false;
        lowTimeEventCalled = false;
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

            AddToDepositedScrap(ScrapManager.Instance.GetDepositedScrap());
        }
        else
        {
            AddToDepositedScrap(ScrapManager.Instance.GetAllDepositedScrap());
            statsHolder.totalScrap += ScrapManager.Instance.GetAllDepositedScrap();

            // so bad, but fuck it.
            commonCards += UpgradeCardManager.Instance.GetAllCardCountOfType(CardTier.Common);
            rareCards += UpgradeCardManager.Instance.GetAllCardCountOfType(CardTier.Rare);
            epicCards += UpgradeCardManager.Instance.GetAllCardCountOfType(CardTier.Epic);
        }
        ResetTimer();
        currentDifficulty++;
        LevelLoading.Instance.LoadScene(LevelCollection.LevelKey.HubWorld.ToString());
    }


    public void SetWorldSeed(int newSeed, bool setLevelSeed)
    {
        worldSeed = newSeed;

        if (setLevelSeed) { SetLevelSeed(newSeed); }

        Random.InitState(worldSeed);
    }

    public int GetWorldSeed()
    {
        return worldSeed;
    }

    public void SetLevelSeed(int newSeed)
    {
        currentLevel++; // Not the best place but works here
        print(newSeed);
        levelSeed = newSeed;
    }

    public int GetLevelSeed()
    {
        return levelSeed;
    }

    public int GenerateNextSeed()
    {
        if (GetWorldSeed() == -1)
        {
            int timeStampSeed = (int)new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();

            SetWorldSeed(timeStampSeed, true);
        }
        Random.InitState(GetLevelSeed());

        SetLevelSeed(Random.Range(int.MinValue, int.MaxValue)); // TF? "Random.Range(-999999999, 999999999)" was the original

        print("world seed: " + GetWorldSeed() + "     " + "randomlevel: " + GetLevelSeed());

        return GetLevelSeed();

    }


    public float GetCurrentTime()
    {
        return currentTime;
    }
    public void EndRun()
    {

    }

    public int GetCurrentDifficulty()
    {
        return currentDifficulty;
    }

    public int GetCardCount(CardTier cardTier)
    {
        switch (cardTier)
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

    public void RemoveFromStoredCards(CardTier cardTier, int amountToTake)
    {
        switch (cardTier)
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

    public void AddToStoredCards(CardTier cardTier, int amountToAdd)
    {
        switch (cardTier)
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

    public int GetCardCost(CardTier cardTier)
    {
        switch (cardTier)
        {
            default:
                return 0;
            case CardTier.Common:
                return commonCurrentCost;
            case CardTier.Rare:
                return rareCurrentCost;
            case CardTier.Epic:
                return epicCurrentCost;
        }
    }

    public void UnlockCard(CardTier cardTier)
    {
        switch (cardTier)
        {
            case CardTier.Common:
                RemoveFromDepositedScrap(commonCurrentCost);
                commonCurrentCost += commonCostIncrease;
                break;
            case CardTier.Rare:
                RemoveFromDepositedScrap(rareCurrentCost);
                rareCurrentCost += rareCostIncrease;
                break;
            case CardTier.Epic:
                RemoveFromDepositedScrap(epicCurrentCost);
                epicCurrentCost += epicCostIncrease;
                break;
        }

        RemoveFromStoredCards(cardTier, 1);
    }

    public int GetCurrentLives()
    {
        return currentLives;
    }

    public void SetMaxLives(int amount)
    {
        maxLives = amount;
        currentLives = amount;
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

    public int GetCurrentLevel()
    {
        return currentLevel;
    }


    //Stats
    public IEnumerator TrackGameTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1); // Less accurate but don't need to track milliseconds

            if (inDungeon)
            {
                statsHolder.runTime++;
            }
        }

    }
}
