using System;
using UnityEngine;

public class DiscordManager : MonoBehaviour
{
    Discord.Discord discord;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        discord = new Discord.Discord(1437893216409223309, (ulong)Discord.CreateFlags.NoRequireDiscord);
        ChangeActivity();
    }

    void OnDisable()
    {
        discord.Dispose();
    }

    public void ChangeActivity()
    {
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
            Debug.Log("Activity updated!");
        });
    }

    // Update is called once per frame
    void Update()
    {
        discord.RunCallbacks();
    }
}
