using System;
using UnityEngine;


public enum CardTeir
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

    public void CollectUpgradeCard(CardTeir cardTeir)
    {

        switch (cardTeir)
        {
            case CardTeir.Common:
                currentT1Cards++;
                break;
            case CardTeir.Rare:
                currentT2Cards++;
                break;
            case CardTeir.Epic:
                currentT3Cards++;
                break;
        }
    }

    public int GetAllCardCountOfType(CardTeir cardTeir)
    {
        switch (cardTeir)
        {
            case CardTeir.Common:
                return currentT1Cards;
            case CardTeir.Rare:
                return currentT2Cards;
            case CardTeir.Epic:
                return currentT3Cards;
            default:
                return 0;
        }
    }

    public GameObject GetUpgradeCardPrefab(CardTeir cardTeir)
    {
        switch (cardTeir)
        {
            case CardTeir.Common:
                return commonCardPrefab;
            case CardTeir.Rare:
                return rareCardPrefab;
            case CardTeir.Epic:
                return epicCardPrefab;
            default:
                return null;
        }
    }
}

