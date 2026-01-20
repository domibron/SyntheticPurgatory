using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChipInvHoverSelectDetect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField]
    Image frontImage;

    [SerializeField]
    Transform backImageTransform;


    private int id;

    private bool hovered = false; // currently used for debugging.

    ChipBoardMenu chipBoardMenu;

    private bool setup = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetUp(int id, ChipBoardMenu chipBoardMenu, ChipSO chipData)
    {
        if (setup) return;

        this.id = id;
        this.chipBoardMenu = chipBoardMenu;

        const float bevel = 20f;// 20 px.

        GameObject boardItem = Instantiate(chipData.GetBoardChipObject(), backImageTransform);
        Vector2Int chipSize = chipData.GetSize();
        RectTransform rectBack = backImageTransform.GetComponent<RectTransform>();
        // boardItem.transform.localPosition = new Vector3(-(chipSize.x * chipBoardMenu.UnitSize) / 2f, (chipSize.y * chipBoardMenu.UnitSize) / 2f, 0);
        boardItem.transform.localPosition = new Vector3(-(rectBack.sizeDelta.x - bevel) / 2f, (rectBack.sizeDelta.x - bevel) / 2f, 0);
        float newScale = (Mathf.Min(rectBack.sizeDelta.x - bevel, rectBack.sizeDelta.y - bevel) / chipBoardMenu.UnitSize) / Mathf.Max(chipSize.x, chipSize.y);
        boardItem.transform.localScale = new Vector3(newScale, newScale, newScale);


        setup = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!setup) return;

        hovered = true;
        chipBoardMenu.HoveredInventoryItem(id);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!setup) return;

        hovered = false;
        chipBoardMenu.RemoveHoverInventoryItem(id);
    }
}
