using UnityEngine;
using TMPro;
public class PlayerSeedInput : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField seedInputField;
    [SerializeField]
    private GameObject disabledUnlockText;

    public void UpdateWorldSeed()
    {
        if (seedInputField.text == "-")
        {
            seedInputField.text = string.Empty;
        }

        if (seedInputField.text == string.Empty)
        {
            RunManager.Instance.SetRunSeed(-1, true);
            disabledUnlockText.SetActive(false);
        }
        else
        {

            if (int.Parse(seedInputField.text) < 0)
            {
                seedInputField.text = Mathf.Abs(int.Parse(seedInputField.text)).ToString();
            }

            RunManager.Instance.SetRunSeed(int.Parse(seedInputField.text), true);
            disabledUnlockText.SetActive(true);
        }

    }


    public void ClearSeed()
    {
        seedInputField.text = string.Empty;
        //UpdateWorldSeed();
    }
}
