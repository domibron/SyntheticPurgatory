using UnityEngine;

public class WinGame : MonoBehaviour
{
    public void WinDaGame()
    {
        if (SteamStatsAndAchievements.Initialized)
        {
            SteamStatsAndAchievements.Instance.AddToStat(SteamStatsAndAchievements.Stat.STAT_GAMES_WON, 1);
        }

        RunManager.Instance.statsHolder.outcome = true;
    }
}
