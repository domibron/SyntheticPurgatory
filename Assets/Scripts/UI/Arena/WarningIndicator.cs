using UnityEngine;

public class WarningIndicator : MonoBehaviour
{
    [SerializeField]
    private GameObject warningBG;

    [SerializeField]
    private GameObject warningSign;

    [SerializeField]
    private GameObject exclamationMark;

    [SerializeField]
    private Vector3 minScale = Vector3.zero;

    [SerializeField]
    private Vector3 maxScale = Vector3.one;

    private bool isFlashing = false;
    private bool isAlert = false;

    private float flashTime = 0;

    [SerializeField]
    private float flashRate = 10f;

    private float alertTime = 0;

    [SerializeField]
    private float alertRate = 10f;

    [SerializeField]
    private SpriteRenderer bgSprite;

    private Color defaultBGColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // StartFlash();
        defaultBGColor = bgSprite.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFlashing)
        {
            flashTime = 0f;
        }
        else
        {
            flashTime += Time.deltaTime * flashRate;

            warningSign.transform.localScale = Vector3.Lerp(maxScale, minScale, (Mathf.Sin(flashTime) + 1f) / 2f);
        }

        if (!isAlert)
        {
            alertTime = 0f;
        }
        else
        {
            alertTime += Time.deltaTime * alertRate;

            exclamationMark.transform.localScale = Vector3.Lerp(maxScale, minScale, (Mathf.Sin(alertTime) + 1f) / 2f);
        }
    }

    public void StartFlash()
    {
        isFlashing = true;
        warningBG.SetActive(true);
        warningSign.SetActive(true);
        exclamationMark.SetActive(false);
    }

    public void StartAlert()
    {
        isAlert = true;
        warningBG.SetActive(true);
        warningSign.SetActive(false);
        exclamationMark.SetActive(true);
    }

    public void ShowBGOnly()
    {
        warningBG.SetActive(true);
        warningSign.SetActive(false);
        exclamationMark.SetActive(false);

    }

    public void EndMonitor()
    {
        isFlashing = false;
        isAlert = false;
        SetActiveState(false);
    }

    private void SetActiveState(bool state)
    {
        warningBG.SetActive(state);
        warningSign.SetActive(state);
        exclamationMark.SetActive(state);
    }

    public void SetBGColor(Color color)
    {
        bgSprite.color = color;
    }

    public void ResetBGColor()
    {
        bgSprite.color = defaultBGColor;
    }
}
