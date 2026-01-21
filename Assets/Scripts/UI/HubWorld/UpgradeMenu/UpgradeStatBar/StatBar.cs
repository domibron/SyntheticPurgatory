using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StatBar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    TMP_Text title;

    [SerializeField]
    RectTransform currentBar;
    [SerializeField]
    RectTransform upgradeBar;
    [SerializeField]
    RectTransform chipBar;
    [SerializeField]
    RectTransform chipUpgradeBar;

    [SerializeField]
    RectTransform parentContainer;

    [SerializeField]
    RectTransform addButton;
    [SerializeField]
    RectTransform removeButton;

    RectTransform currentRect;

    [SerializeField]
    TMP_Text text;

    [SerializeField]
    StatType statBarType = StatType.MaxHealth;

    [SerializeField]
    AudioClip hover;
    [SerializeField]
    AudioClip add;
    [SerializeField]
    AudioClip remove;

    AudioSource uiAudioSource;

    float total = 0;
    bool displayWholeNumbersOnly = false;

    float currentStatAmount = 0;
    float upgradedStatAmount = 0;
    float currentChip = 0;
    float upgradedChip = 0;

    float parentWidth = 0;
    float parentHeight = 0;
    float positionOffset = 0;
    float getLeftBound { get { return positionOffset - parentWidth; } }
    float getRightBound { get { return positionOffset; } }

    bool isBeingHovered = false;
    enum HoveringOver
    {
        StatBar,
        RemoveButton,
        AddButton,
    }
    HoveringOver hoveringOver = HoveringOver.StatBar;
    bool isStationary = false;
    Vector2 lastPos = Vector2.zero;

    const float slideLerpSpeed = 0.1f;
    float currentLerpValue = 0;
    const float lingerTime = 0.2f;
    float currentLingerTime = 0;

    bool addButtonEnabled = true;
    float addButtonWidth = 0;

    bool removeButtonEnabled = true;
    float removeButtonWidth = 0;

    const float hoverTimeBeforeToolTip = 1f;
    float hoverTime = 0;

    // these are most likely stored references and not copies!
    UpgradablePlayerStat currentStat;
    UpgradablePlayerStat upgradedButNotAppliedStat;



    void OnDestroy()
    {
        if (UpgradeMenuManager.Instance == null) return;
        UpgradeMenuManager.Instance.OnStatsUpdated -= UpdateStoredStatInfo;
    }

    void Start()
    {
        if (UpgradeMenuManager.Instance == null) throw new NullReferenceException($"Cannot find {nameof(UpgradeMenuManager)}!");
        UpgradeMenuManager.Instance.OnStatsUpdated += UpdateStoredStatInfo;
        // UpdateStoredStatInfo();

        uiAudioSource = UIAudioSource.Instance.GetAudioSource();

        // parentWidth = GetComponent<RectTransform>().sizeDelta.x + parentContainer.sizeDelta.x; // * This will always be fucked, UI is updated last in update.
        parentWidth = 0;
        currentLerpValue = 0f;

        currentRect = GetComponent<RectTransform>();


        SetUpStat(0, 0, 0, 0);

        UpdateStoredStatInfo();
    }

    void Update()
    {
        total = currentStatAmount + upgradedStatAmount + currentChip + upgradedChip;
        parentWidth = currentRect.sizeDelta.x + parentContainer.sizeDelta.x;
        parentHeight = currentRect.sizeDelta.y + parentContainer.sizeDelta.y;
        positionOffset = parentWidth / 2f;



        addButtonWidth = addButton.sizeDelta.x;
        removeButtonWidth = removeButton.sizeDelta.x;



        if (isBeingHovered && Pointer.current.position.value == lastPos)
        {
            isStationary = true;
        }
        else
        {
            isStationary = false;
        }

        lastPos = Pointer.current.position.value;



        if (isBeingHovered)
        {
            currentLingerTime = lingerTime;

            Vector3 pos = currentRect.position;

            if (lastPos.x <= pos.x + getLeftBound + removeButtonWidth && lastPos.x >= pos.x + getLeftBound &&
                lastPos.y <= pos.y + (parentHeight / 2f) && lastPos.y >= pos.y - (parentHeight / 2f) && removeButtonEnabled)
                hoveringOver = HoveringOver.RemoveButton;
            else if (lastPos.x <= pos.x + getRightBound && lastPos.x >= pos.x + getRightBound - addButtonWidth &&
                lastPos.y <= pos.y + (parentHeight / 2f) && lastPos.y >= pos.y - (parentHeight / 2f) && addButtonEnabled)
                hoveringOver = HoveringOver.AddButton;
            else
                hoveringOver = HoveringOver.StatBar;
        }
        else if (!isBeingHovered && currentLingerTime > 0) currentLingerTime -= Time.deltaTime;


        if (currentLingerTime > 0 && currentLerpValue <= 1) currentLerpValue += Time.deltaTime * (1 / slideLerpSpeed);
        else if (currentLingerTime <= 0 && currentLerpValue > 0) currentLerpValue -= Time.deltaTime * (1 / slideLerpSpeed);

        if (isStationary)
        {
            if (hoverTime > 0) hoverTime -= Time.deltaTime;

            if (ToolTipTextDisplay.Instance != null && hoverTime <= 0)
            {
                switch (hoveringOver)
                {
                    case HoveringOver.StatBar:
                        ToolTipTextDisplay.Instance.SetDisplayText("STAT STAT STAT");
                        break;
                    case HoveringOver.RemoveButton:
                        ToolTipTextDisplay.Instance.SetDisplayText("REMOVE STAT");
                        break;
                    case HoveringOver.AddButton:
                        ToolTipTextDisplay.Instance.SetDisplayText("ADD STAT");
                        break;
                }
            }
        }
        else
        {
            hoverTime = hoverTimeBeforeToolTip;
        }

        if (addButtonEnabled)
            addButton.localPosition = new Vector3(Mathf.Lerp(getRightBound + addButtonWidth, getRightBound, currentLerpValue), addButton.localPosition.y, addButton.localPosition.z);
        else
            addButton.localPosition = new Vector3(Mathf.Lerp(getRightBound + addButtonWidth, getRightBound, 0), addButton.localPosition.y, addButton.localPosition.z);


        if (removeButtonEnabled)
            removeButton.localPosition = new Vector3(Mathf.Lerp(getLeftBound - removeButtonWidth, getLeftBound, currentLerpValue), removeButton.localPosition.y, removeButton.localPosition.z);
        else
            removeButton.localPosition = new Vector3(Mathf.Lerp(getLeftBound - removeButtonWidth, getLeftBound, 0), removeButton.localPosition.y, removeButton.localPosition.z);


        UpdateStatBar();
    }

    private void UpdateStoredStatInfo()
    {
        if (UpgradeMenuManager.Instance == null)
        {
            throw new NullReferenceException($"Urm, {nameof(UpgradeMenuManager)} is null but you called update stats.");
        }

        (currentStat, upgradedButNotAppliedStat) = UpgradeMenuManager.Instance.GetCurrentStat(statBarType);

        if (currentStat == null || upgradedButNotAppliedStat == null)
        {
            throw new NullReferenceException($"Fetched stats are NULL what the hell, did you add them in {nameof(UpgradeMenuManager)}?");
        }

        if (currentStat.IsIncreasingStat)
        {
            // currentStat.GetCurrentValue(); // gets the total.
            currentStatAmount = currentStat.CurrentValue;
            float diff = upgradedButNotAppliedStat.CurrentValue - currentStat.CurrentValue;
            upgradedStatAmount = diff;

            // chip
            currentChip = currentStat.ChipIncreaseAmount;
            float chipDiff = upgradedButNotAppliedStat.ChipIncreaseAmount - currentStat.ChipIncreaseAmount;
            upgradedChip = chipDiff;
        }
        else
        {
            float offset = currentStat.CurrentValue - upgradedButNotAppliedStat.CurrentValue;
            currentStatAmount = currentStat.CurrentValue - offset;

            upgradedStatAmount = offset;

            // Chip
            float chipOffset = currentStat.ChipIncreaseAmount - upgradedButNotAppliedStat.ChipIncreaseAmount;
            currentChip = currentStat.ChipIncreaseAmount - chipOffset;

            upgradedChip = chipOffset;
        }

        if (upgradedButNotAppliedStat.UpgradedAmount <= currentStat.UpgradedAmount)
        {
            removeButtonEnabled = false;
        }
        else
        {
            removeButtonEnabled = true;
        }


        if (UpgradeMenuManager.Instance.GetRemainingScrap() - upgradedButNotAppliedStat.UpgradeCost() < 0)
        {
            addButtonEnabled = false;
        }
        else
        {
            addButtonEnabled = true;
        }

        text.text = upgradedButNotAppliedStat.GetName() + " - " + upgradedButNotAppliedStat.GetValueWithPreAndSuf();
    }

    public void UpdateStatBar()
    {
        total = currentStatAmount + upgradedStatAmount + currentChip + upgradedChip;

        float currentPercentage = 0f;

        currentBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (currentStatAmount / total) * parentWidth);
        currentPercentage = currentStatAmount / total;

        upgradeBar.localPosition = new Vector3(GetOffsetAmount(currentPercentage), upgradeBar.localPosition.y, upgradeBar.localPosition.z);
        upgradeBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (upgradedStatAmount / total) * parentWidth);
        currentPercentage += upgradedStatAmount / total;

        chipBar.localPosition = new Vector3(GetOffsetAmount(currentPercentage), chipBar.localPosition.y, chipBar.localPosition.z);
        chipBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (currentChip / total) * parentWidth);
        currentPercentage += currentChip / total;

        chipUpgradeBar.localPosition = new Vector3(GetOffsetAmount(currentPercentage), chipUpgradeBar.localPosition.y, chipUpgradeBar.localPosition.z);
        chipUpgradeBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (upgradedChip / total) * parentWidth);
        currentPercentage += upgradedChip / total;
    }

    public void SetUpStat(float current, float upgraded, float chip, float chipUpgraded)
    {
        currentStatAmount = current;
        upgradedStatAmount = upgraded;
        currentChip = chip;
        upgradedChip = chipUpgraded;
    }

    public void SetChipAmount(float amount)
    {
        currentChip = amount;
    }



    private float GetOffsetAmount(float percentage)
    {
        float x = Mathf.Lerp(getLeftBound, getRightBound, percentage);

        if (float.IsNaN(x)) return 0f;
        else return x;
    }



    public void OnPointerEnter(PointerEventData eventData)
    {
        isBeingHovered = true;

        if (addButtonEnabled || removeButtonEnabled)
            uiAudioSource.PlayOneShot(hover);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isBeingHovered = false;
    }


    public void OnAddOneToStat()
    {
        if (UpgradeMenuManager.Instance == null) throw new NullReferenceException($"Cannot find the {nameof(UpgradeMenuManager)}!");

        if (UpgradeMenuManager.Instance.GetRemainingScrap() - upgradedButNotAppliedStat.UpgradeCost() < 0) return; // Prevents and purchases that puts the player into negative.

        UpgradeMenuManager.Instance.AddUpgradeOnce(statBarType);

        uiAudioSource.PlayOneShot(add);
    }

    public void OnRemoveOneToStat()
    {
        if (UpgradeMenuManager.Instance == null) throw new NullReferenceException($"Cannot find the {nameof(UpgradeMenuManager)}!");

        if (upgradedButNotAppliedStat.UpgradedAmount <= currentStat.UpgradedAmount) return; // Stops accidental downgrades that eat into current stats.

        UpgradeMenuManager.Instance.RemoveUpgradeOnce(statBarType);

        uiAudioSource.PlayOneShot(remove);
    }
}
