// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;

// This is the v2 upgrade system, this is now depricated.

// public class SelectCardType : MonoBehaviour
// {
//     [SerializeField]
//     private UpgradeSystem upgradeSystem;


//     // Held Card Values
//     private int activeCard = 0;
//     [SerializeField]
//     private OpenCardUI SelectedCardData;


//     [SerializeField]
//     private OpenCardUI cardOneData;
//     [SerializeField]
//     private OpenCardUI cardTwoData;
//     [SerializeField]
//     private OpenCardUI cardThreeData;

//     [SerializeField]
//     private GameObject cardOneBorder;
//     [SerializeField]
//     private GameObject cardTwoBorder;
//     [SerializeField]
//     private GameObject cardThreeBorder;

//     [SerializeField]
//     private Button openButton;
//     [SerializeField]
//     private Button scrapButton;

//     private void Start()
//     {
//         SetButtonStates(false);
//     }

//     public void SetCardOne(bool state)
//     {
//         if (activeCard == 1) { cardOneBorder.SetActive(false); ; return; }
//         cardOneBorder.SetActive(state);

//     }
//     public void SetCardTwo(bool state)
//     {
//         if (activeCard == 2) { cardTwoBorder.SetActive(false); return; }
//         cardTwoBorder.SetActive(state);

//     }
//     public void SetCardThree(bool state)
//     {
//         if (activeCard == 3) { cardThreeBorder.SetActive(false); return; }
//         cardThreeBorder.SetActive(state);

//     }

//     public void UpdateCardInfo(int index)
//     {
//         if (activeCard == index) { index = 0; SelectedCardData = null; }
//         activeCard = index;

//         switch (activeCard)
//         {
//             case 1:
//                 SetButtonStates(true);
//                 SelectedCardData = cardOneData;
//                 break;
//             case 2:
//                 SetButtonStates(true);
//                 SelectedCardData = cardTwoData;
//                 break;
//             case 3:
//                 SetButtonStates(true);
//                 SelectedCardData = cardThreeData;
//                 break;
//             default:
//                 SetButtonStates(false);
//                 SetCardOne(false);
//                 SetCardTwo(false);
//                 SetCardThree(false);
//                 break;

//         }

//     }

//     public void SetButtonStates(bool state)
//     {
//         openButton.interactable = state;
//         scrapButton.interactable = state;
//     }

//     public void OnOpenClick()
//     {
//         CardTier curCard = SelectedCardData.GetCardTier();
//         upgradeSystem.OpenCard(curCard);
//         if (GameManager.Instance.GetCardCount(curCard) < 1)
//         {
//             SetButtonStates(false);
//             UpdateCardInfo(0);
//         }
//     }


//     public void OnScrapCard()
//     {
//         CardTier curCard = SelectedCardData.GetCardTier();
//         upgradeSystem.ScrapCard(curCard);
//         if (GameManager.Instance.GetCardCount(curCard) < 1)
//         {
//             SetButtonStates(false);
//             UpdateCardInfo(0);
//         }

//     }

// }
