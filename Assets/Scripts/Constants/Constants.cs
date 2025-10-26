using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//by    _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|
// © 2025 domibron



public class Constants
{
    #region Enemy stuff
    /// <summary>
    /// The tag for enemies.
    /// </summary>
    public const string EnemyTag = "Enemy";

    /// <summary>
    /// The name of the enemy layer.
    /// </summary>
    public const string EnemyLayer = "Enemy";

    #endregion

    #region  Player stuff

    /// <summary>
    /// The tag for the player.
    /// </summary>
    public const string PlayerTag = "Player";

    /// <summary>
    /// The name of the player layer.
    /// </summary>
    public const string PlayerLayer = "Player";


    #endregion

    public const string ScrapTag = "Scrap";

    public const string ScrapLayer = "Scrap";

    public const string DepoScrapTag = "DepoScrap";

    public const string DepoSrapLayer = "DepoScrap";

    /// <summary>
    /// The name of the item layer.
    /// </summary>
    // public const string ItemLayer = "Item";

    public const string DefaultLayer = "Default";

    // public const string CheckpointTag = "Checkpoint";

    public const string WallTag = "Wall";

    public const string FloorTag = "Floor";

    public const string DecorationTag = "Decor";
}
