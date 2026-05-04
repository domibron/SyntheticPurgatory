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
            playerStats = new PlayerStats();
        }

        SetAllStats(playerStats);
    }

    private void SetAllStats(PlayerStats stats)
    {
        if (stats != null) GetComponent<Health>().SetMaxHealth(stats.MaxHealthStat.GetCurrentValue());
        if (stats != null) GetComponent<Regeneration>().SetUpRegeneration(stats.RegenerationSpeed / stats.RegenerationAmountStat.GetCurrentValue(), 3f);
        GetComponent<PlayerCombat>().UpdateVariablesWithStats(stats);
        GetComponent<PlayerMovement>().UpdateVariablesWithStats(stats);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
