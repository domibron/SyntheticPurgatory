using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get => instance; }
    private static GameManager instance;

    private int depositedScrap = 0;

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

        LevelLoading.Instance.LoadScene(LevelCollection.LevelKey.HubWorld.ToString());
    }

}
