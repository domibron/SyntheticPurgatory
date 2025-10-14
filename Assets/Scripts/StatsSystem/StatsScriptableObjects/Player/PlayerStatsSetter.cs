using UnityEngine;

public class PlayerStatsSetter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerStats playerStats = null;
        if (GameStatsManager.Instance != null)
        {
            playerStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);
        }
        else
        {
            Debug.LogError("No game stats manager detected. Defaulting player stats.");
        }

        SetAllStats(playerStats);
    }

    private void SetAllStats(PlayerStats stats)
    {
        GetComponent<Health>().SetMaxHealth(stats.MaxHealth);
        GetComponent<PlayerCombat>().UpdateVariablesWithStats(stats);
        GetComponent<PlayerMovement>().UpdateVariablesWithStats(stats);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
