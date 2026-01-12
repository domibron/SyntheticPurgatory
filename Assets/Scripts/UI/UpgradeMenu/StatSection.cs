using UnityEngine;

[ExecuteInEditMode]
public class StatSection : MonoBehaviour
{
    // Usful util to set the size like a content fitter for automatic alignment. Dev QOL.

    [SerializeField]
    RectTransform titleSection;

    [SerializeField]
    RectTransform statListSection;

    RectTransform rectTransform;

    // Update is called once per frame
    void Update()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null) return;

        if (titleSection == null || statListSection == null) return;

        rectTransform.sizeDelta = new(rectTransform.sizeDelta.x, titleSection.sizeDelta.y + statListSection.sizeDelta.y);
    }
}
