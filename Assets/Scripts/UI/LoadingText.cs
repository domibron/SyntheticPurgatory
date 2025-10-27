using TMPro;
using UnityEngine;

public class LoadingText : MonoBehaviour
{
    [SerializeField]
    private TMP_Text loadingText;

    private int count;

    [SerializeField]
    private int countMax = 3;

    [SerializeField]
    private int countMin = 0;

    [SerializeField]
    private float dotsTimer = .7f;

    private float currentTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (currentTimer <= 0)
        {
            currentTimer = dotsTimer;
            count++;
        }
        else
        {
            currentTimer -= Time.deltaTime;
        }

        if (count > countMax)
        {
            count = countMin;
        }

        loadingText.text = "Loading" + GetPeriods(count);
    }

    private string GetPeriods(int count)
    {
        string dots = "";

        for (int i = 0; i < count; i++)
        {
            dots += ".";
        }

        return dots;
    }
}
