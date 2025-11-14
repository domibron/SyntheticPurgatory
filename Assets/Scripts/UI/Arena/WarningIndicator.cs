using UnityEngine;

public class WarningIndicator : MonoBehaviour
{
    [SerializeField]
    private GameObject warningBG;

    [SerializeField]
    private GameObject warningSign;

    [SerializeField]
    private Vector3 minScale = Vector3.zero;

    [SerializeField]
    private Vector3 maxScale = Vector3.one;

    private bool isFlashing = false;

    private float flashTime = 0;

    [SerializeField]
    private float flashRate = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // StartFlash();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFlashing)
        {
            flashTime = 0f;
            return;
        }

        flashTime += Time.deltaTime * flashRate;

        warningSign.transform.localScale = Vector3.Lerp(maxScale, minScale, (Mathf.Sin(flashTime) + 1f) / 2f);
    }

    public void StartFlash()
    {
        isFlashing = true;
        SetActiveState(true);
    }

    public void ShowBGOnly()
    {
        warningBG.SetActive(true);
        warningSign.SetActive(false);
    }

    public void EndFlash()
    {
        isFlashing = false;
        SetActiveState(false);
    }

    private void SetActiveState(bool state)
    {
        warningBG.SetActive(state);
        warningSign.SetActive(state);
    }
}
