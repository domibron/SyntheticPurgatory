// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;

// This is the v2 upgrade system, this is now depricated.

// public class OpenCardUI : MonoBehaviour
// {
//     [SerializeField]
//     private UpgradeSystem upgradeSystem;

//     [SerializeField]
//     private Button button;

//     [SerializeField]
//     private CardTier cardTier;

//     [SerializeField]
//     private TMP_Text cardCost;

//     [SerializeField]
//     private TMP_Text cardAmount;


//     void Update()
//     {
//         cardCost.text = "Cost: " + upgradeSystem.GetCardOpenCost(cardTier).ToString() + "sc";
//         cardAmount.text = GameManager.Instance.GetCardCount(cardTier).ToString();

//         if (GameManager.Instance.GetCardCount(cardTier) <= 0)
//         {
//             button.interactable = false;
//         }
//     }

//     public CardTier GetCardTier()
//     {
//         return cardTier;
//     }

// }
