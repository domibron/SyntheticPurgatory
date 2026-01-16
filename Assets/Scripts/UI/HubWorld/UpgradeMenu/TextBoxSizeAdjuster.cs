using TMPro;
using UnityEngine;

public class TextBoxSizeAdjuster : MonoBehaviour
{
    // Abandon, found layout element, and found content size fitter should go on elements such as text and not parents.

    [SerializeField]
    float minWidthSize = 0;
    [SerializeField]
    float minHeightSize = 0;

    [SerializeField]
    float maxWidthSize = 200;
    [SerializeField]
    float maxHeightSize = 100;

    [SerializeField]
    TMP_Text textBox;

    RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        textBox.autoSizeTextContainer = true;
    }
}
