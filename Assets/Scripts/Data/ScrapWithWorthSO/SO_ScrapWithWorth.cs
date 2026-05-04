using System;
using System.Collections.ObjectModel;
using UnityEngine;

/// <summary>
/// Scrap prefab with associated value. Used for quickly getting the prefab with the scrap value.
/// </summary>
[Serializable]
public class ScrapItemData
{
    /// <summary>
    /// The scrap prefab.
    /// </summary>
    public GameObject ScrapPrefab;

    /// <summary>
    /// Value of the scrap prefab.
    /// </summary>
    public int ScrapWorth;
}

/// <summary>
/// Data class that stores the scrap with associated values to them. Useful for spawning in scrap.
/// </summary>
[CreateAssetMenu(fileName = "ScrapWithWorthData", menuName = "ScriptableObjects/Scrap/ScrapWithWorth")]
public class SO_ScrapWithWorth : ScriptableObject
{
    /// <summary>
    /// The list of scrap and associated values
    /// </summary>
    [SerializeField]
    private ScrapItemData[] scrapItemsData;

    /// <summary>
    /// Gets the read only array of the scrap with value array.
    /// </summary>
    public ReadOnlyCollection<ScrapItemData> ScrapItemData { get => Array.AsReadOnly(scrapItemsData); }
}
