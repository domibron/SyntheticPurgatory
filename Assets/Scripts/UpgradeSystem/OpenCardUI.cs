using TMPro;
using UnityEngine;

public class OpenCardUI : MonoBehaviour
{
    [SerializeField]
    private UpgradeSystem upgradeSystem;

    [SerializeField]
    private CardTeir cardTeir;

    [SerializeField]
    private TMP_Text cardCost;

    [SerializeField]
    private TMP_Text cardAmount;

    public void OnClick()
    {
        upgradeSystem.OpenCard(cardTeir);
    }

    void Update()
    {
        cardCost.text = upgradeSystem.GetCardOpenCost(cardTeir).ToString() + " Scrap";
        cardAmount.text = GameManager.Instance.GetCardCount(cardTeir).ToString();
    }

    public void OnScrapCard()
    {
        upgradeSystem.ScrapCard(cardTeir);
    }
}
