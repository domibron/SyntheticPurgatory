// Hello there o/
// Names here should be self explanatory.

/// <summary>
/// Tags, Layers and anything that is reused in passing in functions.
/// </summary>
public class Constants
{
    // ****************************************
    // *                ENEMY                 *
    // ****************************************

    //                   TAGS
    public const string BossTag = "Boss";

    public const string EnemyTag = "Enemy";

    //                  LAYERS
    public const string EnemyLayer = "Enemy";


    // ****************************************
    // *                PLAYER                *
    // ****************************************

    //                   TAGS
    public const string PlayerTag = "Player";

    //                  LAYERS
    public const string PlayerLayer = "Player";


    // ****************************************
    // *                SCRAP                 *
    // ****************************************

    //                   TAGS
    public const string DepoScrapTag = "DepoScrap";

    public const string ScrapTag = "Scrap";

    //                  LAYERS
    public const string DepoSrapLayer = "DepoScrap";

    public const string ScrapLayer = "Scrap";


    // ****************************************
    // *             CULLING / LOD            *
    // ****************************************
    // These tags are in relation to the culling and LOD systems in Synthetic Purgatory.

    //                   TAGS
    public const string HighDetailTag = "HighDetail";
    public const string MediumDetailTag = "MediumDetail";
    public const string LowDetailTag = "LowDetail";

    // ****************************************
    // *                MISC                  *
    // ****************************************

    //                   TAGS
    public const string CollapsibleFloorTag = "CollapsibleFloor";

    public const string CollectableItemTag = "CollectableItem";

    public const string NavLineTag = "NavLine";

    //                  LAYERS
    public const string DefaultLayer = "Default";
}
