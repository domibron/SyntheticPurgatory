using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Data class for modules with their cost, increase cost and current amount.
/// </summary>
[Serializable]
public class ModuleData
{
    [SerializeField]
    private int Cost = 1;

    [SerializeField]
    private int CostIncreaseAmount = 1;

    // Data is reset every time, we can override the base cost, no?
    // [ReadOnly]
    // private int CurrentCost = 1;

    [ReadOnly]
    private int amount = 0;

    public int GetCost()
    {
        return Cost;
    }

    public void IncreaseCost()
    {
        Cost += CostIncreaseAmount;
    }

    public int GetCostAndIncrease()
    {
        int r = GetCost();
        IncreaseCost();
        return r;
    }

    public int GetAmount()
    {
        return amount;
    }

    public void AddAmount(int amountIncrease = 1)
    {
        amount += amountIncrease;
    }

    public void RemoveAmount(int amountDecrease = 1)
    {
        amount -= amountDecrease;
        if (amount < 0) amount = 0; // stop negative.
    }
}

/// <summary>
/// Manager for the entire run. Stores the lives, time limit and other things related to running the game.
/// </summary>
public class RunManager : MonoBehaviour
{
    /// <summary>
    /// Singleton for the run manager.
    /// </summary>
    public static RunManager Instance { get; private set; }

    /// <summary>
    /// The world seed.
    /// </summary>
    private int runSeed = -1;

    /// <summary>
    /// The seed for the level.
    /// </summary>
    private int levelSeed = -1;


    /// <summary>
    /// All scrap currency the player has collected.
    /// </summary>
    private int depositedScrap = 0;

    // /// <summary>
    // /// The time limit per level. This is set by stats. Not any more.
    // /// </summary>
    // [Obsolete("Stats assigned at the correct time.", true)]
    // private float timePerLevel = 120f;

    /// <summary>
    /// The current time in the level.
    /// </summary>
    private float currentTime = 1f;

    /// <summary>
    /// Keeping track if the player is in the dungeon level.
    /// </summary>
    private bool inDungeon = false;

    /// <summary>
    /// The current level count. Starts at 1.
    /// </summary>
    private int currentLevel = 0;

    /// <summary>
    /// The max lives the player has. This is set by another script.
    /// </summary>
    [SerializeField]
    private int maxLives = 3;

    /// <summary>
    /// The current lives count for the player. 0 is game over.
    /// </summary>
    private int currentLives = 1;

    /// <summary>
    /// All cost and count data for the common modules.
    /// </summary>
    [SerializeField]
    private ModuleData commonModuleData;

    /// <summary>
    /// All cost and count data for the rare modules.
    /// </summary>
    [SerializeField]
    private ModuleData rareModuleData;

    /// <summary>
    /// All cost and count data for the epic modules.
    /// </summary>
    [SerializeField]
    private ModuleData epicModuleData;


    /// <summary>
    /// The current difficulty of the run, this may increase quickly or slowly depending on difficulty selected.
    /// </summary>
    private int currentDifficulty = 0;

    /// <summary>
    /// Is the game paused.
    /// </summary>
    private bool isPaused = false;

    /// <summary>
    /// Is the timer hidden.
    /// </summary>
    private bool isTimerHidden = false;

    // private bool lowTimeEventCalled = false;

    // These both events are not used correctly. Why are there events here? what?
    // Shouldn't time be a asset in the dungeon that gets the stats data when loaded?
    // Why is it bundled here? This is for persistent data. 

    // Screw these.
    // TODO: sort this out. Its annoying to add a remove timer asset in levels. Why isn't it a opt in imp.
    public event Action<float> OnLowTime;
    public event Action<float> OnWarnTime;

    // Its being abused. Not clear at all.
    private float lowTime = 60f;

    // TODO: maybe move this?

    /// <summary>
    /// Stats for the run. For achievements and other things.
    /// </summary>
    public StatsHolder statsHolder = new StatsHolder();


    void Awake()
    {
        // Terminate the other game manager since it needs to be replaced with this one for a fresh run.
        if (Instance != null && Instance != this)
        {
            // Destroy(this);
            Destroy(Instance.gameObject); // Terminate the other game manager since it was brought over from a prev game.
            print("Other game manager has been removed!");
        }

        Instance = this;
        DontDestroyOnLoad(this);
        currentLives = maxLives;
    }

    void Start()
    {
        //timePerLevel = GameStatsManager.Instance.GetStats<MiscellaneousStats>(Stats.miscellaneous).MaxLevelTimeStat.GetCurrentValue();
        StartCoroutine(TrackGameTime());
    }

    void Update()
    {

        // Timer warning and kill player.
        if (inDungeon && currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            if (currentTime < lowTime) InvokeLowTime(); // yes, spam the thing!!!
        }
        else if (inDungeon && currentTime <= 0 && !isPaused)
        {
            isPaused = true;
            PlayerRefFetcher.Instance?.GetPlayerRef()?.GetComponent<PlayerDeath>()?.OnPlayerDeath();
        }
    }

    /// <summary>
    /// Invokes the <see cref="OnLowTime"/> event.
    /// </summary>
    private void InvokeLowTime()
    {
        // if (lowTimeEventCalled) return;

        // lowTimeEventCalled = true;
        OnLowTime?.Invoke(currentTime);
    }

    /// <summary>
    /// Invokes the <see cref="OnWarnTime"/> event. 
    /// </summary>
    public void InvokeRemindTime()
    {
        OnWarnTime?.Invoke(currentTime);
    }

    /// <summary>
    /// Add scrap to the player's bank.
    /// </summary>
    /// <param name="amount"></param>
    public void AddToDepositedScrap(int amount)
    {
        depositedScrap += amount;
    }

    /// <summary>
    /// Remove from the player's bank.
    /// </summary>
    /// <param name="amount"></param>
    public void RemoveFromDepositedScrap(int amount)
    {
        depositedScrap -= amount;
    }

    /// <summary>
    /// Get the current amount in the player's bank.
    /// </summary>
    /// <returns></returns>
    public int GetCurrentScrapCount()
    {
        return depositedScrap;
    }

    // TODO: Create the other timer method. This sucks.
    /// <summary>
    /// Starts the timer. Also sets if in dungeon.
    /// </summary>
    public void StartTimer()
    {
        // should be impossible but just in case.
        // timePerLevel = GameStatsManager.Instance.GetStats<MiscellaneousStats>(Stats.miscellaneous).MaxLevelTimeStat.GetCurrentValue();

        currentTime = RunStatsM.Instance.GetStats<MiscellaneousStats>(Stats.miscellaneous).MaxLevelTimeStat.GetCurrentValue();
        inDungeon = true;
        isPaused = false;
    }

    /// <summary>
    /// Resets the timer.
    /// </summary>
    public void ResetTimer()
    {
        inDungeon = false;
        isTimerHidden = false;
        // lowTimeEventCalled = false;
    }

    /// <summary>
    /// Returns to the hub world if the player still has lives, otherwise it ends the run.
    /// </summary>
    /// <param name="playerDied"></param>
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
                // WHAT?
                // I really hate it here. What is happening.
                // The tech debt is a real thing. Mystery functions that do nothing.
                // History of what was a solution to a removed problem.
            }


        }
        else
        {
            // so bad, but fuck it.
            commonModuleData.AddAmount(ModuleLevelM.Instance.GetAllModuleCountOfType(ModuleTier.Common));
            rareModuleData.AddAmount(ModuleLevelM.Instance.GetAllModuleCountOfType(ModuleTier.Rare));
            epicModuleData.AddAmount(ModuleLevelM.Instance.GetAllModuleCountOfType(ModuleTier.Epic));
        }

        AddToDepositedScrap(ScrapLevelM.Instance.GetDepositedScrap());
        statsHolder.totalScrap += ScrapLevelM.Instance.GetAllCollectedScrap();

        ResetTimer();
        currentDifficulty++;
        LevelLoading.Instance.LoadScene(LevelCollection.LevelKey.HubWorld.ToString());
    }


    /// <summary>
    /// Set the seed for the entire run.
    /// </summary>
    /// <param name="newSeed">The new run seed.</param>
    /// <param name="setLevelSeed">Whether to generate a random level seed too.</param>
    public void SetRunSeed(int newSeed, bool setLevelSeed)
    {
        runSeed = newSeed;

        if (setLevelSeed) { SetLevelSeed(newSeed); }

        Random.InitState(runSeed);
    }

    /// <summary>
    /// Get the run seed.
    /// </summary>
    /// <returns>The seed for the run.</returns>
    public int GetRunSeed()
    {
        return runSeed;
    }

    /// <summary>
    /// Sets the level seed, used for generation mainly.
    /// </summary>
    /// <param name="newSeed">The seed for the level.</param>
    public void SetLevelSeed(int newSeed)
    {
        // TODO: move to better spot. This called like 2-3 times when loading into the first level.
        currentLevel++;

        print(newSeed);
        levelSeed = newSeed;
    }

    /// <summary>
    /// Get the level seed.
    /// </summary>
    /// <returns>The seed for the level.</returns>
    public int GetLevelSeed()
    {
        return levelSeed;
    }

    /// <summary>
    /// Generate a new seed.
    /// </summary>
    /// <returns>The new seed.</returns>
    public int GenerateNextSeed()
    {
        if (GetRunSeed() == -1)
        {
            int timeStampSeed = (int)new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();

            SetRunSeed(timeStampSeed, true);
        }

        Random.InitState(GetLevelSeed());

        SetLevelSeed(Random.Range(int.MinValue, int.MaxValue)); // TF? "Random.Range(-999999999, 999999999)" was the original ~ yeah, who the hell would do that? Any way.

        print("world seed: " + GetRunSeed() + "     " + "randomlevel: " + GetLevelSeed());

        return GetLevelSeed();

    }

    /// <summary>
    /// Get the current time in level.
    /// </summary>
    /// <returns>The amount of time in seconds.</returns>
    public float GetCurrentTime()
    {
        return currentTime;
    }

    /// <summary>
    /// Get the current difficulty.
    /// </summary>
    /// <returns>The current difficulty. 0 is easy.</returns>
    public int GetCurrentDifficulty()
    {
        return currentDifficulty;
    }

    /// <summary>
    /// Get the current count of the module tier.
    /// </summary>
    /// <param name="cardTier">The tier to check for.</param>
    /// <returns>The amount of that tier of modules.</returns>
    public int GetModuleCount(ModuleTier cardTier)
    {
        switch (cardTier)
        {
            case ModuleTier.Common:
                return commonModuleData.GetAmount();
            case ModuleTier.Rare:
                return rareModuleData.GetAmount();
            case ModuleTier.Epic:
                return epicModuleData.GetAmount();
            default:
                return 0;
        }
    }

    /// <summary>
    /// Remove from the currently stored modules with the given tier.
    /// </summary>
    /// <param name="cardTier">The tier to remove from.</param>
    /// <param name="amountToTake">The amount to remove.</param>
    public void RemoveFromStoredModules(ModuleTier cardTier, int amountToTake)
    {
        switch (cardTier)
        {
            case ModuleTier.Common:
                commonModuleData.RemoveAmount(amountToTake);
                break;
            case ModuleTier.Rare:
                rareModuleData.RemoveAmount(amountToTake);
                break;
            case ModuleTier.Epic:
                epicModuleData.RemoveAmount(amountToTake);
                break;
        }
    }

    /// <summary>
    /// Add to the currently stored modules with the given tier.
    /// </summary>
    /// <param name="cardTier">The tier to add to.</param>
    /// <param name="amountToAdd">The amount to add.</param>
    public void AddToStoredModules(ModuleTier cardTier, int amountToAdd)
    {
        switch (cardTier)
        {
            case ModuleTier.Common:
                commonModuleData.AddAmount(amountToAdd);
                break;
            case ModuleTier.Rare:
                rareModuleData.AddAmount(amountToAdd);
                break;
            case ModuleTier.Epic:
                epicModuleData.AddAmount(amountToAdd);
                break;
        }
    }

    /// <summary>
    /// Get the cost to open one module from that tier.
    /// </summary>
    /// <param name="cardTier">The tier to get the cost for.</param>
    /// <returns>The cost to open that tier of module.</returns>
    public int GetModuleCost(ModuleTier cardTier)
    {
        switch (cardTier)
        {
            default:
                return 0;
            case ModuleTier.Common:
                return commonModuleData.GetCost();
            case ModuleTier.Rare:
                return rareModuleData.GetCost();
            case ModuleTier.Epic:
                return epicModuleData.GetCost();
        }
    }

    /// <summary>
    /// Removes 1 from the stored module with the given tier and takes the cost for opening it from the player's banked scrap.
    /// </summary>
    /// <param name="cardTier">The tier of module to open.</param>
    public void OpenModule(ModuleTier cardTier)
    {
        switch (cardTier)
        {
            case ModuleTier.Common:
                RemoveFromDepositedScrap(commonModuleData.GetCostAndIncrease());
                break;
            case ModuleTier.Rare:
                RemoveFromDepositedScrap(rareModuleData.GetCostAndIncrease());
                break;
            case ModuleTier.Epic:
                RemoveFromDepositedScrap(epicModuleData.GetCostAndIncrease());
                break;
        }

        RemoveFromStoredModules(cardTier, 1);
    }


    /// <summary>
    /// Get the current amount of lives.
    /// </summary>
    /// <returns>The amount of lives left.</returns>
    public int GetCurrentLives()
    {
        return currentLives;
    }

    /// <summary>
    /// Set the maximum of lives.
    /// </summary>
    /// <param name="amount">The amount to set the max to.</param>
    public void SetMaxLives(int amount)
    {
        maxLives = amount;
        currentLives = amount;
    }

    /// <summary>
    /// Get the maximum amount of lives the player has this run.
    /// </summary>
    /// <returns></returns>
    public int GetMaxLives()
    {
        return maxLives;
    }

    // TODO: remove this.
    /// <summary>
    /// Hide the timer.
    /// </summary>
    public void HideTimer()
    {
        isTimerHidden = true;
    }

    /// <summary>
    /// Check to see if the timer is hidden.
    /// </summary>
    /// <returns></returns>
    public bool IsTimerHidden()
    {
        return isTimerHidden;
    }

    /// <summary>
    /// Get the current level count.
    /// </summary>
    /// <returns>The current amount of times the player entered the dungeon.</returns>
    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    // TODO: fix this mess.
    //Stats
    public IEnumerator TrackGameTime()
    {
        while (true)
        {
            // USE TIME STAMPS YOU MELLON!
            yield return new WaitForSeconds(1); // Less accurate but don't need to track milliseconds

            if (inDungeon)
            {
                statsHolder.runTime++;
            }
        }

    }
}
