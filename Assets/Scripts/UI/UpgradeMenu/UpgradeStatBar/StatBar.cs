using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatBar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    TMP_Text title;

    [SerializeField]
    RectTransform defualtBar;
    [SerializeField]
    RectTransform upgradeBar;
    [SerializeField]
    RectTransform aboutBar;
    [SerializeField]
    RectTransform chipBar;

    [SerializeField]
    RectTransform parentContainer;

    [SerializeField]
    RectTransform addButton;
    [SerializeField]
    RectTransform removeButton;


    string statText = "STAT";

    float total = 0;
    bool displayWholeNumbersOnly = false;

    float currentAmount = 0;
    float upgradedAmount = 0;
    float upgradeAboutAmount = 0;
    float chipAmount = 0;

    float parentWidth = 0;
    float positionOffset = 0;
    float getLeftBound { get { return positionOffset - parentWidth; } }
    float getRightBound { get { return positionOffset; } }

    bool isBeingHovered = false;

    const float slideLerpSpeed = 0.1f;
    float currentLerpValue = 0;
    const float lingerTime = 0.2f;
    float currentLingerTime = 0;

    float addButtonWidth = 0;
    float removeButtonWidth = 0;

    const float hoverTimeBeforeToolTip = 1f;
    float hoverTime = 0;

    void Start()
    {
        // parentWidth = GetComponent<RectTransform>().sizeDelta.x + parentContainer.sizeDelta.x; // * This will always be fucked, UI is updated last in update.
        parentWidth = 0;
        currentLerpValue = 0f;

        SetUpStat(20, 50, 20, 10);
    }

    void Update()
    {
        total = currentAmount + upgradedAmount + upgradeAboutAmount + chipAmount;
        parentWidth = GetComponent<RectTransform>().sizeDelta.x + parentContainer.sizeDelta.x;
        positionOffset = parentWidth / 2f;



        addButtonWidth = addButton.sizeDelta.x;
        removeButtonWidth = removeButton.sizeDelta.x;

        if (isBeingHovered) currentLingerTime = lingerTime;
        else if (!isBeingHovered) currentLingerTime -= Time.deltaTime;


        if (currentLingerTime > 0 && currentLerpValue <= 1) currentLerpValue += Time.deltaTime * (1 / slideLerpSpeed);
        else if (currentLingerTime <= 0 && currentLerpValue > 0) currentLerpValue -= Time.deltaTime * (1 / slideLerpSpeed);

        if (isBeingHovered)
        {
            if (hoverTime > 0) hoverTime -= Time.deltaTime;

            if (ToolTipTextDisplay.Instance != null && hoverTime <= 0)
            {
                ToolTipTextDisplay.Instance.SetDisplayText("STAT STAT STAT");
            }
        }
        else
        {
            hoverTime = hoverTimeBeforeToolTip;
        }

        addButton.localPosition = new Vector3(Mathf.Lerp(getRightBound + addButtonWidth, getRightBound, currentLerpValue), addButton.localPosition.y, addButton.localPosition.z);
        removeButton.localPosition = new Vector3(Mathf.Lerp(getLeftBound - removeButtonWidth, getLeftBound, currentLerpValue), removeButton.localPosition.y, removeButton.localPosition.z);


        UpdateStatBar();
    }

    public void UpdateStatBar()
    {
        float currentPercentage = 0f;

        defualtBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (currentAmount / total) * parentWidth);
        currentPercentage = currentAmount / total;

        upgradeBar.localPosition = new Vector3(GetOffsetAmount(currentPercentage), upgradeBar.localPosition.y, upgradeBar.localPosition.z);
        upgradeBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (upgradedAmount / total) * parentWidth);
        currentPercentage += upgradedAmount / total;

        aboutBar.localPosition = new Vector3(GetOffsetAmount(currentPercentage), aboutBar.localPosition.y, aboutBar.localPosition.z);
        aboutBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (upgradeAboutAmount / total) * parentWidth);
        currentPercentage += upgradeAboutAmount / total;

        chipBar.localPosition = new Vector3(GetOffsetAmount(currentPercentage), chipBar.localPosition.y, chipBar.localPosition.z);
        chipBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (chipAmount / total) * parentWidth);
        currentPercentage += chipAmount / total;
    }

    public void SetUpStat(float defualt, float upgraded, float about, float chip)
    {
        currentAmount = defualt;
        upgradedAmount = upgraded;
        upgradeAboutAmount = about;
        chipAmount = chip;
    }

    public void SetUpgradeAbout(float aboutAmount)
    {
        upgradeAboutAmount = aboutAmount;
    }

    public void SetUpgrade(float amount)
    {
        upgradedAmount = amount;
    }

    public void SetChipAmount(float amount)
    {
        chipAmount = amount;
    }

    private float GetOffsetAmount(float percentage)
    {
        return Mathf.Lerp(getLeftBound, getRightBound, percentage);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isBeingHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isBeingHovered = false;
    }
}
