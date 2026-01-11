using TMPro;
using UnityEngine;

public class StatBar : MonoBehaviour
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

    string statText = "STAT";

    float total = 0;
    bool displayWholeNumbersOnly = false;

    float currentAmount = 0;
    float upgradedAmount = 0;
    float upgradeAboutAmount = 0;
    float chipAmount = 0;

    float parentWidth = 0;
    float positionOffset = 0;

    void Start()
    {
        parentWidth = GetComponent<RectTransform>().sizeDelta.x + parentContainer.sizeDelta.x;

        SetUpStat(20, 50, 20, 10);
    }

    void Update()
    {
        total = currentAmount + upgradedAmount + upgradeAboutAmount + chipAmount;
        parentWidth = GetComponent<RectTransform>().sizeDelta.x + parentContainer.sizeDelta.x;
        positionOffset = parentWidth / 2f;

        print(GetComponent<RectTransform>().sizeDelta.x + parentContainer.sizeDelta.x);

        UpdateUI();
    }

    public void UpdateUI()
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
        return Mathf.Lerp(positionOffset - parentWidth, positionOffset, percentage);
    }

}
