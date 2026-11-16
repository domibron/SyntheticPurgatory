using UnityEngine;
using Steamworks;
using System;

public class SteamStatsAndAchievements : MonoBehaviour
{
    public static SteamStatsAndAchievements Instance { get; private set; }

    public static bool Initialized { get; private set; }


    public enum Achievement : int
    {
        ACH_KILL_10_ENEMIES,
        ACH_WIN_ONE_GAME,
        ACH_BETA,
        ACH_KILL_100000_ENEMIES,
    }

    public enum Stat : int
    {
        STAT_ENEMIES_KILLED,
        STAT_GAMES_WON,
    }

    public enum StatType : byte
    {
        Float,
        Int,
        Average,
    }

    private Achievement_t[] m_Achievements = new Achievement_t[0];

    private Stat_t[] m_Stats = new Stat_t[0];

    public Action StatsRefreshed;


#if UNITY_2019_3_OR_NEWER
    // In case of disabled Domain Reload, reset static members before entering Play Mode.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void InitOnPlayMode()
    {
        Initialized = false;
        Instance = null;
    }
#endif

    #region  MonoBehaviours

    void OnEnable()
    {
        if (!SteamManager.Initialized)
        {
            return;
        }

        // // Cache the GameID for use in the Callbacks
        // m_GameID = new CGameID(SteamUtils.GetAppID());

        // // m_UserStatsReceived = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
        // // m_UserStatsStored = Callback<UserStatsStored_t>.Create(OnUserStatsStored);

        // // TODO: These need to be reset to get the stats upon assembly reload in the Editor.
        // m_bRequestedStats = false;
        // m_bStatsValid = false;
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }

        Instance = this;

        /// Here are where the achievements are assigned to the collection.
        /// Once you created the enum for that achievement and it has the same API name as the enum.
        /// You define it here. Dont worry about the names and descriptions as they will be fetched from Steam.
        /// As for the function / lambda, this is the unlock check. () => Enemies killed > 10 for example.
        /// It must return a true or false value and true will unlock that achievement for the player.
        /// DO NOT use the lambda for any achievements that unlock from progress stat on the steam works page.
        /// Use the respected functions to unlock achievements.

        m_Achievements = new Achievement_t[]
        {
            new Achievement_t(Achievement.ACH_WIN_ONE_GAME),
            new Achievement_t(Achievement.ACH_KILL_10_ENEMIES),
            new Achievement_t(Achievement.ACH_BETA, checkCriteria: () => true),
            new Achievement_t(Achievement.ACH_KILL_100000_ENEMIES),
        };


        /// Here are the stats that you can get and update.
        /// When adding a new stat make sure the API name on the steamworks and the enum are a match.
        /// You will also need to give it a name too. This is NOT automatically fetched.
        /// make sure to define the stat type since Steam will error if you try to set or get a miss0-matched type.
        /// Use the respected functions to get and update the stats.

        m_Stats = new Stat_t[]
        {
            new Stat_t(Stat.STAT_ENEMIES_KILLED, "Enemies Killed"),
            new Stat_t(Stat.STAT_GAMES_WON, "Games Won"),
        };

        Initialized = true;
    }



    void Update()
    {
        if (!SteamManager.Initialized)
        {
            return;
        }

        RefreshAchievements();
        RefreshStats();

        foreach (Achievement_t achievement in m_Achievements)
        {
            if (achievement.m_bAchieved || achievement.m_CheckCriteria is null) continue;

            if (achievement.m_CheckCriteria())
            {
                UnlockAchievement(achievement);
            }
        }
    }

    #endregion

    #region Achievement Funcs

    public void UnlockAchievement(Achievement achievement)
    {
        if (!SteamManager.Initialized) return;

        UnlockAchievement(GetAchievement(achievement));
    }

    public bool IsAchievementUnlocked(Achievement achievement)
    {
        if (!SteamManager.Initialized) return false;

        Achievement_t _ = GetAchievement(achievement);

        if (_ != null)
        {
            return _.m_bAchieved;
        }

        return false;
    }

    public bool SetUnlockCriteria(Achievement achievement, Func<bool> check)
    {
        if (!SteamManager.Initialized) return false;

        Achievement_t _ = GetAchievement(achievement);

        if (_ != null)
        {
            _.m_CheckCriteria = check;
            return true;
        }

        return false;
    }

    public bool RemoveUnlockCriteria(Achievement achievement)
    {
        if (!SteamManager.Initialized) return false;

        Achievement_t _ = GetAchievement(achievement);

        if (_ != null)
        {
            _.m_CheckCriteria = null;
            return true;
        }

        return false;
    }

    public Achievement_t GetAchievement(Achievement achievement)
    {
        if (!SteamManager.Initialized) return null;

        foreach (Achievement_t ach in m_Achievements)
        {
            if (ach.m_eAchievementID != achievement) continue;

            return ach;
        }

        return null;
    }

    private void UnlockAchievement(Achievement_t achievement)
    {
        if (!SteamManager.Initialized) return;

        if (achievement.m_bAchieved) return;

        achievement.m_bAchieved = true;

        SteamUserStats.SetAchievement(achievement.m_eAchievementID.ToString());
    }

    private void RefreshAchievements()
    {
        if (!SteamManager.Initialized) return;

        foreach (Achievement_t ach in m_Achievements)
        {
            bool ret = SteamUserStats.GetAchievement(ach.m_eAchievementID.ToString(), out ach.m_bAchieved);
            if (ret)
            {
                ach.m_strName = SteamUserStats.GetAchievementDisplayAttribute(ach.m_eAchievementID.ToString(), "name");
                ach.m_strDescription = SteamUserStats.GetAchievementDisplayAttribute(ach.m_eAchievementID.ToString(), "desc");
            }
            else
            {
                Debug.LogWarning("SteamUserStats.GetAchievement failed for Achievement " + ach.m_eAchievementID + "\nIs it registered in the Steam Partner site?");
            }
        }
    }

    #endregion



    #region Stat Funcs

    /// <summary>
    /// Sets the start with the new value. DO NOT use this to reset stats.
    /// </summary>
    /// <param name="targetStat">The stat to set the value of.</param>
    /// <param name="values">The new value of the stat. Only use one for Int and Float, use two for average.</param>
    public void SetStatValue(Stat targetStat, params object[] values)
    {
        if (!SteamManager.Initialized) return;

        if (values.Length <= 0) return;

        foreach (Stat_t stat in m_Stats)
        {
            if (stat.m_eStatID != targetStat) continue;

            switch (stat.m_eStatType)
            {
                case StatType.Int:
                    if (values[0] is not int)
                    {
                        Debug.LogErrorFormat("Cannot combine {0} with non {0} value!", "int");
                        break;
                    }

                    if (SteamUserStats.SetStat(stat.m_eStatID.ToString(), (int)values[0]))
                    {
                        RefreshStats();
                    }
                    else
                    {
                        Debug.LogWarning("SteamUserStats.SetStat failed for stat " + stat.m_eStatID + "\nIs it registered in the Steam Partner site?");
                    }
                    break;

                case StatType.Float:
                    if (values[0] is not float)
                    {
                        Debug.LogErrorFormat("Cannot combine {0} with non {0} value!", "float");
                        break;
                    }

                    if (SteamUserStats.SetStat(stat.m_eStatID.ToString(), (float)values[0]))
                    {
                        RefreshStats();
                    }
                    else
                    {
                        Debug.LogWarning("SteamUserStats.SetStat failed for stat " + stat.m_eStatID + "\nIs it registered in the Steam Partner site?");
                    }
                    break;

                case StatType.Average:
                    if (values.Length < 2)
                    {
                        Debug.LogError("Cannot set the average with less than 2 values!");
                        break;
                    }

                    if (values[0] is not float)
                    {
                        Debug.LogErrorFormat("Cannot combine param[0] {0} with non {0} value!", "float");
                        break;
                    }

                    if (values[1] is not double)
                    {
                        Debug.LogErrorFormat("Cannot combine param[1] {0} with non {0} value!", "double");
                        break;
                    }

                    if (SteamUserStats.UpdateAvgRateStat(stat.m_eStatID.ToString(), (float)values[0], (double)values[1]))
                    {
                        RefreshStats();
                    }
                    else
                    {
                        Debug.LogWarning("SteamUserStats.SetStat failed for stat " + stat.m_eStatID + "\nIs it registered in the Steam Partner site?");
                    }
                    break;
            }
        }
    }

    public Stat_t GetStat(Stat targetStat)
    {
        if (!SteamManager.Initialized) return null;

        foreach (Stat_t stat in m_Stats)
        {
            if (stat.m_eStatID != targetStat) continue;

            return stat;
        }

        return null;
    }


    public object GetStatValue(Stat targetStat, out bool result)
    {
        result = false;
        if (!SteamManager.Initialized) return 0;

        foreach (Stat_t stat in m_Stats)
        {
            if (stat.m_eStatID != targetStat) continue;

            switch (stat.m_eStatType)
            {
                case StatType.Int:
                    result = true;
                    return stat.m_intValue;

                case StatType.Float:
                    result = true;
                    return stat.m_floatValue;

                case StatType.Average:
                    result = true;
                    return stat.m_floatValue;
            }
        }

        return 0;
    }

    public void AddToStat(Stat targetStat, object amount)
    {
        if (!SteamManager.Initialized) return;

        foreach (Stat_t stat in m_Stats)
        {
            if (stat.m_eStatID != targetStat) continue;

            switch (stat.m_eStatType)
            {
                case StatType.Int:
                    if (amount is int)
                        SetStatValue(targetStat, stat.m_intValue + (int)amount);
                    break;

                case StatType.Float:
                    if (amount is float)
                        SetStatValue(targetStat, stat.m_intValue + (float)amount);
                    break;

                case StatType.Average:
                    Debug.LogError("This does not support average stat types!");
                    break;
            }


        }
    }

    private void RefreshStats()
    {
        if (!SteamManager.Initialized) return;

        foreach (Stat_t stat in m_Stats)
        {
            switch (stat.m_eStatType)
            {
                case StatType.Int:
                    if (SteamUserStats.GetStat(stat.m_eStatID.ToString(), out int iVal))
                    {
                        stat.m_intValue = iVal;
                    }
                    else
                    {
                        Debug.LogWarning("SteamUserStats.GetStat failed for stat " + stat.m_eStatID + "\nIs it registered in the Steam Partner site?");
                    }
                    break;

                case StatType.Float:
                    if (SteamUserStats.GetStat(stat.m_eStatID.ToString(), out float fVal))
                    {
                        stat.m_floatValue = fVal;
                    }
                    else
                    {
                        Debug.LogWarning("SteamUserStats.GetStat failed for stat " + stat.m_eStatID + "\nIs it registered in the Steam Partner site?");
                    }
                    break;

                case StatType.Average:
                    if (SteamUserStats.GetStat(stat.m_eStatID.ToString(), out float avgVal))
                    {
                        stat.m_floatValue = avgVal;
                    }
                    else
                    {
                        Debug.LogWarning("SteamUserStats.GetStat failed for stat " + stat.m_eStatID + "\nIs it registered in the Steam Partner site?");
                    }
                    break;
            }
        }

        StatsRefreshed?.Invoke();
    }

    #endregion





    public void ResetAllStats(bool AchievementsToo)
    {
        if (!SteamManager.Initialized)
        {
            return;
        }

        SteamUserStats.ResetAllStats(AchievementsToo);
        RefreshStats();
        RefreshAchievements();
    }



    public void OnGUI()
    {
        if (!SteamManager.Initialized)
        {
            GUILayout.Label("Steamworks not Initialized");
            return;
        }


        GUILayout.Label("m_totalGamesWon: " + GetStatValue(Stat.STAT_GAMES_WON, out bool _));
        GUILayout.Label("m_nTotalEnemiesKilled: " + GetStatValue(Stat.STAT_ENEMIES_KILLED, out bool _));

        GUILayout.BeginArea(new Rect(Screen.width - 300, 0, 300, 800));
        foreach (Achievement_t ach in m_Achievements)
        {
            GUILayout.Label(ach.m_eAchievementID.ToString());
            GUILayout.Label(ach.m_strName + " - " + ach.m_strDescription);
            GUILayout.Label("Achieved: " + ach.m_bAchieved);
            GUILayout.Space(20);
        }

        // FOR TESTING PURPOSES ONLY!
        if (GUILayout.Button("RESET STATS AND ACHIEVEMENTS"))
        {
            SteamUserStats.ResetAllStats(true);
            RefreshStats();
        }
        GUILayout.EndArea();
    }


    #region Data classes

    public class Achievement_t
    {
        public Achievement m_eAchievementID;
        public string m_strName;
        public string m_strDescription;
        public bool m_bAchieved;
        public Func<bool> m_CheckCriteria;


        /// <summary>
        /// Creates an Achievement. You must also mirror the data provided here in https://partner.steamgames.com/apps/achievements/yourappid
        /// </summary>
        /// <param name="achievementID">The "API Name Progress Stat" used to uniquely identify the achievement.</param>
        /// <param name="name">The "Display Name" that will be shown to players in game and on the Steam Community.</param>
        /// <param name="desc">The "Description" that will be shown to players in game and on the Steam Community.</param>
        public Achievement_t(Achievement achievementID, string name = "", string desc = "", Func<bool> checkCriteria = null)
        {
            m_eAchievementID = achievementID;
            m_strName = name;
            m_strDescription = desc;
            m_CheckCriteria = checkCriteria;
            m_bAchieved = false;
        }
    }

    public class Stat_t
    {
        public Stat m_eStatID;
        public StatType m_eStatType;
        public string m_strName;

        public int m_intValue;
        public float m_floatValue;

        public Stat_t(Stat statID, string name, StatType type = StatType.Int)
        {
            m_eStatID = statID;
            m_strName = name;
            m_eStatType = type;
        }
    }

    #endregion
}
