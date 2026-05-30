using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpenCardMenu : MonoBehaviour
{
    [SerializeField]
    ChipBoardMenu chipBoardMenu;

    [SerializeField]
    TMP_Text commonCounter;

    [SerializeField]
    TMP_Text commonCost;

    [SerializeField]
    Button commonButton;

    [SerializeField]
    TMP_Text rareCounter;

    [SerializeField]
    TMP_Text rareCost;

    [SerializeField]
    Button rareButton;

    [SerializeField]
    TMP_Text epicCounter;

    [SerializeField]
    TMP_Text epicCost;

    [SerializeField]
    Button epicButton;

    [Space, SerializeField]
    TMP_Text unlockTitle;

    [SerializeField]
    TMP_Text unlockDescription;

    [SerializeField]
    Transform chipDisplaySection;

    [SerializeField]
    float bevel = 20f;

    [SerializeField]
    GameObject newChipDisplayUI;

    [SerializeField]
    GameObject showPlayerCanUnlockCard;

    private GameObject displayedChip; // so we can destroy it later.


    private RunManager gameManager;

    private ChipRunM chipManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (RunManager.Instance == null)
        {
            Debug.LogError("GameManager Instance is null!");
            return;
        }

        gameManager = RunManager.Instance;

        if (ChipRunM.Instance == null)
        {
            Debug.LogError("ChipManager Instance is null!");
            return;
        }

        chipManager = ChipRunM.Instance;

        CloseDisplayNewChipUI();
    }

    // Update is called once per frame
    void Update()
    {
        commonCounter.text = gameManager.GetModuleCount(ModuleTier.Common).ToString("N0");
        rareCounter.text = gameManager.GetModuleCount(ModuleTier.Rare).ToString("N0");
        epicCounter.text = gameManager.GetModuleCount(ModuleTier.Epic).ToString("N0");

        commonCost.text = gameManager.GetModuleCost(ModuleTier.Common).ToString("N0") + "sc";
        rareCost.text = gameManager.GetModuleCost(ModuleTier.Rare).ToString("N0") + "sc";
        epicCost.text = gameManager.GetModuleCost(ModuleTier.Epic).ToString("N0") + "sc";

        if (gameManager.GetModuleCount(ModuleTier.Common) <= 0 || gameManager.GetCurrentScrapCount() < gameManager.GetModuleCost(ModuleTier.Common)) commonButton.interactable = false;
        else commonButton.interactable = true;

        if (gameManager.GetModuleCount(ModuleTier.Rare) <= 0 || gameManager.GetCurrentScrapCount() < gameManager.GetModuleCost(ModuleTier.Rare)) rareButton.interactable = false;
        else rareButton.interactable = true;

        if (gameManager.GetModuleCount(ModuleTier.Epic) <= 0 || gameManager.GetCurrentScrapCount() < gameManager.GetModuleCost(ModuleTier.Epic)) epicButton.interactable = false;
        else epicButton.interactable = true;

        if (commonButton.interactable || rareButton.interactable || epicButton.interactable) showPlayerCanUnlockCard.SetActive(true);
        else showPlayerCanUnlockCard.SetActive(false);
    }

    public void OpenCommon()
    {
        if (!commonButton.interactable) return;

        OpenAndDisplayNewChip(ModuleTier.Common);

        gameManager.OpenModule(ModuleTier.Common);
    }

    public void OpenRare()
    {
        if (!rareButton.interactable) return;

        OpenAndDisplayNewChip(ModuleTier.Rare);

        gameManager.OpenModule(ModuleTier.Rare);
    }

    public void OpenEpic()
    {

        if (!epicButton.interactable) return;

        OpenAndDisplayNewChip(ModuleTier.Epic);

        gameManager.OpenModule(ModuleTier.Epic);
    }

    private void OpenAndDisplayNewChip(ModuleTier moduleTier)
    {
        // GameObject boardItem = Instantiate(chipData.GetBoardChipObject(), backImageTransform);

        ChipSO chipData = chipManager.OpenModule(moduleTier);

        unlockTitle.text = "Unlocked " + chipData.GetNameOfChip();
        unlockDescription.text = $"<b>Tier:</b>\n{moduleTier}\n<b>Description:</b>\n{chipData.GetDescriptionOfChip()}";

        if (displayedChip != null) Destroy(displayedChip);

        displayedChip = ChipUtil.CreateAndReturnBoardItem(chipData);
        displayedChip.transform.SetParent(chipDisplaySection);

        Vector2Int chipSize = chipData.GetSize();
        RectTransform rectBack = chipDisplaySection.GetComponent<RectTransform>();
        // boardItem.transform.localPosition = new Vector3(-(chipSize.x * chipBoardMenu.UnitSize) / 2f, (chipSize.y * chipBoardMenu.UnitSize) / 2f, 0);
        displayedChip.transform.localPosition = new Vector3(-(rectBack.sizeDelta.x - bevel) / 2f, (rectBack.sizeDelta.x - bevel) / 2f, 0);
        float newScale = (Mathf.Min(rectBack.sizeDelta.x - bevel, rectBack.sizeDelta.y - bevel) / ChipBoardMenu.UnitSize) / Mathf.Max(chipSize.x, chipSize.y);
        displayedChip.transform.localScale = new Vector3(newScale, newScale, newScale);

        newChipDisplayUI.SetActive(true);

        chipBoardMenu.CreateNewInventoryChip(chipManager.GetTotalChipCount() - 1); // im lazy.
    }

    public void CloseDisplayNewChipUI()
    {
        newChipDisplayUI.SetActive(false);
        if (displayedChip != null) Destroy(displayedChip);
    }

}
