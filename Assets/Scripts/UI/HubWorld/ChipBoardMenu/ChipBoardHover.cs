using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ChipBoardHover : MonoBehaviour
{
    [SerializeField]
    ChipBoardMenu chipBoardMenu;

    bool isBeingHovered = false;

    Vector2 lowerBounds;
    Vector2 upperBounds;
    Vector2 halfSize;
    Vector2 mousePos;
    Vector2Int mousePosOnGrid;


    void Start()
    {
        halfSize = new Vector2((chipBoardMenu.Width / 2f) * ChipBoardMenu.UnitSize, (chipBoardMenu.Height / 2f) * ChipBoardMenu.UnitSize);

        lowerBounds = new Vector2(transform.position.x - halfSize.x, transform.position.y - halfSize.y);
        upperBounds = new Vector2(transform.position.x + halfSize.x, transform.position.y + halfSize.y);
    }

    void Update()
    {
        mousePos = Pointer.current.position.ReadValue();


        if (mousePos.x >= lowerBounds.x && mousePos.x <= upperBounds.x
        && mousePos.y >= lowerBounds.y && mousePos.y <= upperBounds.y)
        {
            isBeingHovered = true;
        }
        else
        {
            isBeingHovered = false;
        }


        if (isBeingHovered)
        {
            int localizedPosX = Mathf.FloorToInt((mousePos.x - lowerBounds.x) / ChipBoardMenu.UnitSize);
            int localizedPosY = Mathf.FloorToInt((upperBounds.y - mousePos.y) / ChipBoardMenu.UnitSize); // inverted because grid is top to bottom not bottom to top.
            mousePosOnGrid = new Vector2Int(localizedPosX, localizedPosY);
        }
        else
            mousePosOnGrid = new Vector2Int(-1, -1);

        // update the chipboard manager.
        // chipBoardMenu.SetCursorGridPos(mousePosOnGrid);
    }

    public Vector2Int GetCursorGridPos()
    {
        return mousePosOnGrid;
    }

    public bool IsBeingHovered()
    {
        return isBeingHovered;
    }
}
