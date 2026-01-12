using UnityEngine;
using UnityEngine.UI;

public class ForceVerticalLayoutToUpdate : MonoBehaviour
{
    RectTransform rectTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        // to fix the stupid layout group only updating once whilst playing causing UI elements that resize to be clipping.
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }
}
