using System;
using UnityEngine;


public enum CardTier
{
    Common,
    Rare,
    Epic,
}

public class UpgradeCardManager : MonoBehaviour
{
    public static UpgradeCardManager Instance { get; private set; }

    // could replace with dictionary with enum + int, and have it auto init, this would allow for more expantion without too much recode.
    private int currentT1Cards = 0;
    private int currentT2Cards = 0;
    private int currentT3Cards = 0;

    [SerializeField]
    private GameObject commonCardPrefab;
    [SerializeField]
    private GameObject rareCardPrefab;
    [SerializeField]
    private GameObject epicCardPrefab;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            Debug.LogError($"Two or more {nameof(UpgradeCardManager)} exists, this one was removed! Make sure only one exists at all times.", this);
            return;
        }

        Instance = this;
    }

    public void CollectUpgradeCard(CardTier cardTeir)
    {

        switch (cardTeir)
        {
            case CardTier.Common:
                currentT1Cards++;
                break;
            case CardTier.Rare:
                currentT2Cards++;
                break;
            case CardTier.Epic:
                currentT3Cards++;
                break;
        }
    }

    public int GetAllCardCountOfType(CardTier cardTeir)
    {
        switch (cardTeir)
        {
            case CardTier.Common:
                return currentT1Cards;
            case CardTier.Rare:
                return currentT2Cards;
            case CardTier.Epic:
                return currentT3Cards;
            default:
                return 0;
        }
    }

    public GameObject GetUpgradeCardPrefab(CardTier cardTeir)
    {
        switch (cardTeir)
        {
            case CardTier.Common:
                return commonCardPrefab;
            case CardTier.Rare:
                return rareCardPrefab;
            case CardTier.Epic:
                return epicCardPrefab;
            default:
                return null;
        }
    }
}

