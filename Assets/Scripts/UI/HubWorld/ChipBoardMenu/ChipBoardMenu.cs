using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChipBoardMenu : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public float UnitSize = 200f;
    public int Width = 4;
    public int Height = 3;

    private Vector3 pos;
    private RectTransform rectTransform;

    private Vector2Int cursorGridPos;

    // readonly Vector2Int NULL_CURSOR_POS = new Vector2Int(-1, -1);

    private ChipBoardHover chipBoardHover; // we had hover ref this and vice versa. a but cringe.

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        pos = rectTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        cursorGridPos = chipBoardHover.GetCursorGridPos();
    }

    void UpdateGrid()
    {

    }

    void UpdateInventory()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {

        // eventData.button == PointerEventData.InputButton.Right
        throw new System.NotImplementedException();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}
