using System;
using Discord;
using UnityEngine;
// using Discord;

public class DiscordManager : MonoBehaviour
{
    [SerializeField]
    private long clientId;

    Discord.Discord discord;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        try
        {
            discord = new Discord.Discord(clientId, (ulong)Discord.CreateFlags.NoRequireDiscord);
            ChangeActivity();
        }
        catch (ResultException ex)
        {
            return; // fuck off
        }
    }

    void OnDisable()
    {
        if (discord != null)
            discord.Dispose();
    }

    // This prevents all types of exit, requiring task manager to kill the application.
    // [RuntimeInitializeOnLoadMethod]
    // static void StopExit()
    // {
    //     Application.wantsToQuit += OnApplicationQuit;
    // }

    // private static bool OnApplicationQuit()
    // {
    //     return false;
    // }

    public void ChangeActivity()
    {
        if (discord == null) return;

        var activityManager = discord.GetActivityManager();

        var activity = new Discord.Activity
        {
#if UNITY_EDITOR
            Details = "Developing the game.",
#else
            Details = "Playing the game.",
#endif
            Timestamps = new Discord.ActivityTimestamps
            {
                Start = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds()
            }
        };

        activityManager.UpdateActivity(activity, (res) =>
        {
            Debug.Log("Activity updated!" + res);
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (discord == null) return;
        try
        {
            discord.RunCallbacks();
        }
        catch (ResultException ex)
        {
            return; // fuck off
        }
    }

}
