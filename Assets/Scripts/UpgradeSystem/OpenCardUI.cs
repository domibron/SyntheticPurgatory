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

    public void OnClick()
    {
        upgradeSystem.OpenCard(cardTeir);
    }

    void Update()
    {
        cardCost.text = upgradeSystem.GetCardOpenCost(cardTeir).ToString() + " Scrap";
    }
}
