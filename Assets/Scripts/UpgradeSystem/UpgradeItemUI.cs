using TMPro;
using UnityEngine;

public class UpgradeItemUI : MonoBehaviour
{
    [SerializeField]
    private UpgradeSystem upgradeSystem;

    [SerializeField]
    private UpgradeType upgradeType = UpgradeType.Ranged;

    [SerializeField]
    private TMP_Text InfoText;

    public void SetText(string text)
    {
        InfoText.text = text;
    }

    public UpgradeType GetUpgradeType()
    {
        return upgradeType;
    }

    public void OnClick()
    {
        upgradeSystem.UpgradeSelected(upgradeType);
    }
}
