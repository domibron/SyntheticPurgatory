using TMPro;
using UnityEngine;

public class UpgradeItemUI : MonoBehaviour
{
    public TMP_Text NameBox;
    public TMP_Text ValueBox;
    public TMP_Text CostBox;

    private string upgradeKey;

    private UpgradeSystem upgradeSystem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetUp(string name, string value, string cost, string upgradeKey, UpgradeSystem upgradeSystem)
    {
        NameBox.text = name;
        ValueBox.text = "Value [" + value + "]";
        CostBox.text = "Cost {" + cost + "}";
        this.upgradeKey = upgradeKey;
        this.upgradeSystem = upgradeSystem;
    }

    public void OnClick()
    {
        // bool res = upgradeSystem.UpgradeItem(upgradeKey, out string newValue);

        // if (res)
        // {
        //     ValueBox.text = "[" + newValue + "]";
        // }
    }
}
