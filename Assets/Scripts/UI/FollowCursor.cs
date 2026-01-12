using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FollowCursor : MonoBehaviour
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
        // Cursor.SetCursor();

        rectTransform.position = Pointer.current.position.value;
    }
}
