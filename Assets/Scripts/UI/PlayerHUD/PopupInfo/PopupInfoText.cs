using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupInfoText : MonoBehaviour
{
    [SerializeField]
    private float slideInTime = 1f;

    [SerializeField]
    private float slideOutTime = 1f;

    [SerializeField]
    private float defaultDuration = 3f;

    private float currentDurationTime = 0f;

    [SerializeField]
    private Transform objectToSlide;

    private float slideInTimer = 0f;

    private Vector3 offsetPos = Vector3.zero;
    private Vector3 targetPos = Vector3.zero;

    private bool isSlidingIn = true;

    [SerializeField]
    private TMP_Text text;

    [SerializeField]
    private Image image;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


        if (isSlidingIn)
        {
            slideInTimer += Time.deltaTime * (1f / slideInTime);
        }
        else
        {
            slideInTimer -= Time.deltaTime * (1f / slideOutTime);

            if (slideInTimer <= 0) Destroy(this.gameObject);
        }

        slideInTimer = Mathf.Clamp01(slideInTimer);

        Vector3 pos = objectToSlide.localPosition;


        pos.x = Mathf.Lerp(offsetPos.x, targetPos.x, slideInTimer);

        objectToSlide.localPosition = pos;

        currentDurationTime -= Time.deltaTime;

        if (currentDurationTime <= 0)
        {
            isSlidingIn = false;
        }

    }

    public void Initialize(string textInfo, Sprite icon, float additionalOffset = 0f, float duration = -1f)
    {
        if (duration <= 1f)
        {
            currentDurationTime = Mathf.Max(defaultDuration, 1f);
        }
        else
        {
            currentDurationTime = duration;
        }

        if (icon == null)
        {
            text.rectTransform.offsetMin = new Vector2(text.rectTransform.offsetMin.y, text.rectTransform.offsetMin.y);
            // text.rectTransform.offsetMin = new Vector2(text.rectTransform.anchoredPosition.y, text.rectTransform.anchoredPosition.y);
            // text.rectTransform.sizeDelta = new Vector2(text.rectTransform.sizeDelta.y, text.rectTransform.sizeDelta.y);
            image.gameObject.SetActive(false);
        }
        else
        {
            image.sprite = icon;
        }

        float offset = objectToSlide.GetComponent<RectTransform>().sizeDelta.x;

        targetPos = objectToSlide.localPosition;

        offsetPos = targetPos + (Vector3.left * offset) + (Vector3.left * additionalOffset);

        slideInTimer = 0f;
        isSlidingIn = true;

        objectToSlide.localPosition = offsetPos;

        text.text = textInfo;
    }
}
