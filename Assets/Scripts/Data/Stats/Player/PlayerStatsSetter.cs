using UnityEngine;

/// <summary>
/// Reads the player stats from the stat manager or creates dummy class if there is no manager and sets it to the player.
/// </summary>
public class PlayerStatsSetter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerStats playerStats = null;
        if (RunStatsM.Instance != null)
        {
            playerStats = RunStatsM.Instance.GetStats<PlayerStats>(Stats.player);
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
}
