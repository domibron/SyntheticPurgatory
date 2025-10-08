using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//by    _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|
// © 2025 domibron

//  ▄▄▄▄  ▄▄▄▄▄   ▄▄▄▄   ▄▄▄▄  ▄      ▄▄▄▄▄▄▄▄▄▄▄▄▄ ▄▄▄▄▄▄
// ▄▀  ▀▄ █    █ █▀   ▀ ▄▀  ▀▄ █      █        █    █     
// █    █ █▄▄▄▄▀ ▀█▄▄▄  █    █ █      █▄▄▄▄▄   █    █▄▄▄▄▄
// █    █ █    █     ▀█ █    █ █      █        █    █     
//  █▄▄█  █▄▄▄▄▀ ▀▄▄▄█▀  █▄▄█  █▄▄▄▄▄ █▄▄▄▄▄   █    █▄▄▄▄▄


[Obsolete("Please use" + nameof(LevelCollection) + "instead!", true)]
public static class LevelCollections
{
    public static string[] Level1 = new string[] { "Level_1A", "Level_1B" };
    public static string[] Level2 = new string[] { "Level_2A", "Level_2B" };

    public static bool CheckSceneInCollection(string sceneName)
    {
        if (Level1.Contains(sceneName) || Level2.Contains(sceneName)) return true;
        else return false;
    }

    public static string[] GetCollectionNameFromScene(string sceneName)
    {
        if (Level1.Contains(sceneName)) return Level1;
        else if (Level2.Contains(sceneName)) return Level2;
        else return null;
    }

    public static int GetLevelIDFromSceneName(string sceneName)
    {
        if (Level1.Contains(sceneName)) return 1;
        else if (Level2.Contains(sceneName)) return 2;
        else return 0;
    }
}
