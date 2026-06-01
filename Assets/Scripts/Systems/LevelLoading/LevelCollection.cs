using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Collection of all the levels in the game. This also includes multiple scenes to make a level.
/// </summary>
public static class LevelCollection
{
    /*
    READ ME!
    Before adding a new scene to the list please know this first.
    
    The enum "LevelKey" is case sensitive since it uses the enum as a string.
    
    Make sure to add another item in the dictionary, use the others as an example. 
    Make sure that the key is the same as the enum case matters!
    For the values, you need to make sure they are named the same as in the build index. Yes case matters again.
    
    Add your scene to the build index, DO NOT make it first, that is reserved for the boot strap / persistent scene.
    */

    /// <summary>
    /// Enum names of levels that can be loaded.
    /// </summary>
    public enum LevelKey
    {
        MainMenu,
        Tutorial,
        TutorialHub,
        SetupScreen,
        HubWorld,
        DungeonWorld,
        BossWorld,
        PersistentScene,
    }

    /// <summary>
    /// A collection of all the levels that can be loaded.
    /// </summary>
    private static Dictionary<string, string[]> AllLevels = new Dictionary<string, string[]>()
    {
        { "MainMenu", new string[] { "MainMenu" } },
        { "Tutorial", new string[] { "Tutorial" } },
        { "TutorialHub", new string[] { "TutorialHub" } },
        { "SetupScreen", new string[] { "SetupScreen" } },
        { "HubWorld", new string[] { "HubWorld" } },
        { "DungeonWorld", new string[] { "DungeonWorld" } },
        { "BossWorld", new string[] { "BossWorld" } },
        { "PersistentScene", new string[] { "PersistentScene" } },
    };

    /// <summary>
    /// Check to see if the scene with the name is in the <see cref="AllLevels"/> collection.
    /// </summary>
    /// <param name="sceneName">The name of the scene to look for.</param>
    /// <returns>True if the scene name was found in at least one collection.</returns>
    public static bool CheckSceneInCollection(string sceneName)
    {
        foreach (var key in AllLevels.Keys)
        {
            if (AllLevels[key].Contains(sceneName)) return true;
        }

        return false;
    }

    /// <summary>
    /// Does the key exist in the <see cref="AllLevels"/> collection.
    /// </summary>
    /// <param name="key">The key to check for.</param>
    /// <returns>True if the key exists in the collection.</returns>
    public static bool DoesKeyExistsInCollection(string key)
    {
        return AllLevels[key].Contains(key);
    }

    /// <summary>
    /// Get the name of the fist collection with the scene name.
    /// </summary>
    /// <param name="sceneName">The scene name to look for.</param>
    /// <returns>The name of the first collection that scene is in.</returns>
    public static string[] GetCollectionNameFromScene(string sceneName)
    {
        foreach (var key in AllLevels.Keys)
        {
            if (AllLevels[key].Contains(sceneName)) return AllLevels[key];
        }

        return null;
    }

    /// <summary>
    /// Get a collection of levels from with the key.
    /// </summary>
    /// <param name="key">The key to get the data for.</param>
    /// <returns>A collection of scene names from that key, or NULL if the key does not exist.</returns>
    public static string[] GetCollectionFromKey(string key)
    {
        if (AllLevels.ContainsKey(key)) return AllLevels[key];
        else return null;
    }

    /// <summary>
    /// Check if the scene name match any collection key.
    /// </summary>
    /// <param name="sceneName">The name of the scene to check against.</param>
    /// <param name="key">The key to check the contents for the scene.</param>
    /// <returns>True if the scene name is in that collection.</returns>
    public static bool DoesSceneMatchStoredKey(string sceneName, string key)
    {
        if (!AllLevels.ContainsKey(key)) return false;

        return AllLevels[key].Contains(sceneName);
    }
}
