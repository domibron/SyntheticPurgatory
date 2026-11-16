using UnityEngine;

public class IncreaseGameWinSteamStat : MonoBehaviour
{
    void Start()
    {
        if (GetComponent<Health>())
            GetComponent<Health>().OnDeath += OnDeath;
    }

    private void OnDeath()
    {
        if (!SteamStatsAndAchievements.Initialized) return;

        SteamStatsAndAchievements.Instance.AddToStat(SteamStatsAndAchievements.Stat.STAT_ENEMIES_KILLED, 1);
    }
}
