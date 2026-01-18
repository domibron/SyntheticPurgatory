using UnityEngine;
using UnityEngine.EventSystems;

public class ChipBoardMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public float unitSize = 200f;
    public int width = 4;
    public int height = 3;

    private Vector3 pos;
    private RectTransform rectTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        pos = rectTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}
