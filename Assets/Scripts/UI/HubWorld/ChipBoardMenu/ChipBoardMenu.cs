using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChipBoardMenu : MonoBehaviour//, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public float UnitSize = 200f;
    public int Width = 4;
    public int Height = 3;

    float halfWidthSize { get => (float)Width / 2f; }
    float halfHeightSize { get => (float)Height / 2f; }

    [SerializeField]
    Transform placeChipsSection;

    [SerializeField]
    Transform inventorySection;

    [SerializeField]
    GameObject InventoryChipItem;

    private ChipManager chipManager;

    private Vector3 pos;
    private RectTransform rectTransform;

    private Vector2Int cursorGridPos;

    // readonly Vector2Int NULL_CURSOR_POS = new Vector2Int(-1, -1);

    [SerializeField]
    private ChipBoardHover chipBoardHover; // we had hover ref this and vice versa. a but cringe.

    private int hoveredInventoryChipID = -1;

    private int currentSelectedChipID = -1;
    private bool selectedFromInventory = false;

    Dictionary<int, GameObject> inventoryChips = new Dictionary<int, GameObject>();
    Dictionary<int, GameObject> placedChips = new Dictionary<int, GameObject>();




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        pos = rectTransform.localPosition;
        chipManager = ChipManager.Instance;

        Init();
    }

    // Update is called once per frame
    void Update()
    {
        cursorGridPos = chipBoardHover.GetCursorGridPos();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            int chipSelectRes = chipManager.GetChipIdFromGridPos(cursorGridPos); // banking on this returning -1 for null.
            if (chipSelectRes != -1)
            {
                currentSelectedChipID = chipSelectRes;
                selectedFromInventory = false;
            }
            else if (hoveredInventoryChipID != -1)
            {
                currentSelectedChipID = hoveredInventoryChipID;
                selectedFromInventory = true;
            }
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (currentSelectedChipID != -1)
            {
                if (cursorGridPos == new Vector2Int(-1, -1))
                {
                    if (!selectedFromInventory)
                        MoveToInventoryFromBoard(currentSelectedChipID);
                }
                else
                {
                    if (selectedFromInventory)
                    {
                        MoveToBoardFromInventory(currentSelectedChipID, cursorGridPos);
                    }
                    else
                    {
                        MoveToNewLocationOnBoard(currentSelectedChipID, cursorGridPos);
                    }
                }
            }

            currentSelectedChipID = -1;
            selectedFromInventory = false;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            int chipSelectRes = chipManager.GetChipIdFromGridPos(cursorGridPos);
            if (chipSelectRes != -1)
            {
                MoveToInventoryFromBoard(chipSelectRes);
            }
        }

    }

    void Init()
    {
        List<int> invChips = chipManager.GetAllInventoryChips();
        Dictionary<int, Vector2Int> placedChips = chipManager.GetAllPlacedChips();

        foreach (int chip in invChips)
        {
            AddToInventory(chip);
        }

        foreach (int chip in placedChips.Keys)
        {
            CreateAndAddToBoard(chip, placedChips[chip], false); // we are working of a existing list. So we dont need to update it.
        }
    }

    private void AddToInventory(int id)
    {
        if (inventoryChips.ContainsKey(id))
        {
            Debug.LogError("Key already exists!");
            return;
        }

        chipManager.AddChipToInventory(id);

        ChipSO chipData = chipManager.GetChipDataFromID(id);

        GameObject invItem = Instantiate(InventoryChipItem, inventorySection);

        invItem.GetComponent<ChipInvHoverSelectDetect>().SetUp(id, this, chipData);

        inventoryChips.Add(id, invItem);
    }

    private bool CreateAndAddToBoard(int id, Vector2Int pos, bool updateChipManager = true)
    {
        if (placedChips.ContainsKey(id))
        {
            Debug.LogError("Key already exists!");
            return false;
        }

        if (updateChipManager)
        {
            if (!chipManager.AddChipToBoard(id, pos))
            {
                return false;
            }
        }

        ChipSO chipData = chipManager.GetChipDataFromID(id);

        GameObject chipBoardObject = Instantiate(chipData.GetBoardChipObject(), placeChipsSection);

        chipBoardObject.transform.localPosition = ConvertedGridPosToRealPos(pos);

        placedChips.Add(id, chipBoardObject);


        return true;
    }

    private void MoveToInventoryFromBoard(int id)
    {
        RemoveChipFromBoard(id);
        AddToInventory(id);

        UpdateStatsToo();
    }

    private void MoveToBoardFromInventory(int id, Vector2Int pos)
    {
        if (!CreateAndAddToBoard(id, pos)) return; // we failed, abort.
        RemoveChipFromInventory(id);

        UpdateStatsToo();
    }

    private void MoveToNewLocationOnBoard(int id, Vector2Int pos)
    {
        if (!placedChips.ContainsKey(id))
        {
            Debug.LogError("NO MATCHING ID ON BOARD!");
            return;
        }

        if (!chipManager.AddChipToBoard(id, pos)) return; // failed, so exit out.
        placedChips[id].transform.localPosition = ConvertedGridPosToRealPos(pos);

        print(chipManager.BoardToString());

    }

    private void RemoveChipFromInventory(int id)
    {
        Destroy(inventoryChips[id]); // IKY! could fuck up any iteration on placedChips.
        inventoryChips.Remove(id);

        chipManager.RemoveChipToInventory(id);
    }

    private void RemoveChipFromBoard(int id)
    {
        Destroy(placedChips[id]); // IKY! could fuck up any iteration on placedChips.
        placedChips.Remove(id);

        chipManager.RemoveChipFromBoard(id);

        print(chipManager.BoardToString());
    }

    // public void OnPointerClick(PointerEventData eventData)
    // {

    //     // eventData.button == PointerEventData.InputButton.Right
    //     // throw new System.NotImplementedException();
    // }

    // public void OnPointerDown(PointerEventData eventData)
    // {
    //     if (eventData.button == PointerEventData.InputButton.Left)
    //     {
    //         int chipSelectRes = chipManager.GetChipIdFromGridPos(cursorGridPos); // banking on this returning -1 for null.
    //         if (chipSelectRes != -1)
    //         {
    //             currentSelectedChipID = chipSelectRes;
    //             selectedFromInventory = false;
    //         }
    //         else if (hoveredInventoryChipID != -1)
    //         {
    //             currentSelectedChipID = hoveredInventoryChipID;
    //             selectedFromInventory = true;
    //         }
    //     }
    //     else if (eventData.button == PointerEventData.InputButton.Right)
    //     {
    //         int chipSelectRes = chipManager.GetChipIdFromGridPos(cursorGridPos);
    //         if (chipSelectRes != -1)
    //         {
    //             MoveToInventoryFromBoard(chipSelectRes);
    //         }
    //     }
    // }

    // public void OnPointerUp(PointerEventData eventData)
    // {
    //     if (eventData.button == PointerEventData.InputButton.Left)
    //     {
    //         if (currentSelectedChipID != -1)
    //         {
    //             if (cursorGridPos == new Vector2Int(-1, -1))
    //             {
    //                 MoveToInventoryFromBoard(currentSelectedChipID);
    //             }
    //             else
    //             {
    //                 if (selectedFromInventory)
    //                 {
    //                     MoveToBoardFromInventory(currentSelectedChipID, cursorGridPos);
    //                 }
    //                 else
    //                 {
    //                     MoveToNewLocationOnBoard(currentSelectedChipID, cursorGridPos);
    //                 }
    //             }
    //         }

    //         currentSelectedChipID = -1;
    //         selectedFromInventory = false;
    //     }
    // }

    public void HoveredInventoryItem(int id)
    {
        if (hoveredInventoryChipID != id)
            hoveredInventoryChipID = id;
    }

    public void RemoveHoverInventoryItem(int id)
    {
        if (hoveredInventoryChipID != id) return;

        RemoveHoverInventoryItem();
    }

    public void RemoveHoverInventoryItem()
    {
        hoveredInventoryChipID = -1;
    }

    private Vector3 ConvertedGridPosToRealPos(Vector2Int pos)
    {
        return new Vector3((-halfWidthSize * UnitSize) + (UnitSize * pos.x), (halfHeightSize * UnitSize) - (UnitSize * pos.y));
    }

    private void UpdateStatsToo()
    {
        UpgradeMenuManager.Instance.UpdateStatUI(); // updates the stats with the new data.
        // the manager will fetch the data.
    }
}
