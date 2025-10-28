using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get => instance; }
    private static GameManager instance;

    private int depositedScrap = 0;

    public float TimePerLevel = 120f;

    private float currentTime = 1f;
    private bool inDungeon = false;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
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
        }
        else
        {
            ScrapManager.Instance.AddDepositedScrapToGameManager();
        }
        ResetTimer();
        LevelLoading.Instance.LoadScene(LevelCollection.LevelKey.HubWorld.ToString());
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }
}
