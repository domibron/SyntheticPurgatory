using TMPro;
using UnityEngine;

public class DisplayCost : MonoBehaviour
{
    private TMP_Text text;
    private UpgradeMenuManager umm;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TMP_Text>();
        umm = UpgradeMenuManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        text.text = umm.GetCurrentCost().ToString();
    }
}
