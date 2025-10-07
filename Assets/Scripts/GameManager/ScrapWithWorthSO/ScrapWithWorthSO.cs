using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

[Serializable]
public class ScrapItemData
{
    public GameObject ScrapPrefab;
    public int ScrapWorth;
}

[CreateAssetMenu(fileName = "ScrapWithWorthData", menuName = "ScriptableObjects/Scrap/ScrapWithWorth")]
public class ScrapWithWorthSO : ScriptableObject
{
    [SerializeField]
    private ScrapItemData[] scrapItemsData;

    // this might not work. Im trying to return a copy of the array and not a direct refernce to prevent accidental modification.
    public ReadOnlyCollection<ScrapItemData> ScrapItemData { get => Array.AsReadOnly(scrapItemsData); }
}
