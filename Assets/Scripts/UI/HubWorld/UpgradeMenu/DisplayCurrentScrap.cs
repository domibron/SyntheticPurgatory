using TMPro;
using UnityEngine;

public class DisplayCurrentScrap : MonoBehaviour
{
    TMP_Text text;


    RunManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = RunManager.Instance;
        text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        int currentScrap = gameManager.GetCurrentScrapCount();
        if (currentScrap > 0)
            text.text = currentScrap.ToString("N0") + "sc";
        else
            text.text = "<b><color=red>" + currentScrap + "</color><b>";
    }
}
