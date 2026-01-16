using UnityEngine;

public class StatPurchaseBar : MonoBehaviour
{
    private RectTransform barRootTransform;

    float height;
    Vector3 originalPos;

    bool isVisible = false;

    float localMoveLerp = 0;
    const float slideLerpSpeed = 0.1f;

    UpgradeMenuManager upgradeMenuManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        barRootTransform = GetComponent<RectTransform>();
        originalPos = barRootTransform.localPosition;
        upgradeMenuManager = UpgradeMenuManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        height = barRootTransform.sizeDelta.y;

        isVisible = upgradeMenuManager.GetCurrentCost() > 0;

        if (isVisible && localMoveLerp < 1)
        {
            localMoveLerp += Time.deltaTime * (1f / slideLerpSpeed);
        }
        else if (!isVisible && localMoveLerp > 0)
        {
            localMoveLerp -= Time.deltaTime * (1f / slideLerpSpeed);
        }

        barRootTransform.localPosition = Vector3.Lerp(originalPos - (Vector3.up * height), originalPos, localMoveLerp);

    }

    public void PurchaseStats()
    {
        upgradeMenuManager.GetConfirmApplyStats();
    }

    public void ResetPurchase()
    {
        upgradeMenuManager.GetConfirmRevertStats();
    }
}
