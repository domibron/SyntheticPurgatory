using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public static class LevelCollection
{
    public enum LevelKey
    {
        MainMenu,
        HubWorld,
        DungeonWorld,
        BossWorld,
        PersistantScene,
    }

    private static Dictionary<string, string[]> AllLevels = new Dictionary<string, string[]>()
    {
        { "MainMenu", new string[] { "MainMenu" } },
        { "HubWorld", new string[] { "HubWorld" } },
        { "DungeonWorld", new string[] { "DungeonWorld" } },
        { "BossWorld", new string[] { "BossWorld" } },
        { "PersistantScene", new string[] { "PersistantScene" } },
    };


    public static bool CheckSceneInCollection(string sceneName)
    {
        foreach (var key in AllLevels.Keys)
        {
            if (AllLevels[key].Contains(sceneName)) return true;
        }

        return false;
    }

    public static bool DoesKeyExistsInCollection(string key)
    {
        return AllLevels[key].Contains(key);
    }

    public static string[] GetCollectionNameFromScene(string sceneName)
    {
        foreach (var key in AllLevels.Keys)
        {
            if (AllLevels[key].Contains(sceneName)) return AllLevels[key];
        }

        return null;
    }

    public static string[] GetCollectionFromKey(string key)
    {
        if (AllLevels.ContainsKey(key)) return AllLevels[key];
        else return null;
    }
}
