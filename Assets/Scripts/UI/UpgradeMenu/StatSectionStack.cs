using System;
using UnityEngine;

[ExecuteInEditMode]
public class StatSectionStack : MonoBehaviour
{
    [Serializable]
    struct Padding
    {
        public float Left;
        public float Right;
        public float Top;
        public float Bottom;
    }

    enum Alignment
    {
        TopLeft,
        TopRight,
    }

    [SerializeField]
    Padding padding;

    [SerializeField]
    float spacing;

    // [SerializeField]
    // Alignment alignment;

    RectTransform rectTransform;

    // Update is called once per frame
    void Update()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null) return;


        if (transform.childCount <= 0) return;

        float width = rectTransform.sizeDelta.x;
        Vector3 targetPos = rectTransform.localPosition;

        targetPos.x = rectTransform.sizeDelta.x / 2f;
        targetPos.y = rectTransform.rect.height / 2f;

        targetPos.x += padding.Left;
        targetPos.y -= padding.Top;

        float targetWidth = width - (padding.Left + padding.Right);

        for (int i = transform.childCount - 1; i >= 0; i--) // !! This isnt working right at the moment I have abandond it for now.
        {
            RectTransform childRect = transform.GetChild(0).GetComponent<RectTransform>();

            // float xOffset = targetPos.x + (childRect.sizeDelta.x / 2f);
            // float yOffset = targetPos.y + (childRect.sizeDelta.y / 2f);


            childRect.localPosition = targetPos - new Vector3(0, childRect.rect.height / 2f, 0);
            childRect.sizeDelta.Set(targetWidth, childRect.sizeDelta.y);

            targetPos.y -= childRect.sizeDelta.y + spacing;
        }
    }


}
