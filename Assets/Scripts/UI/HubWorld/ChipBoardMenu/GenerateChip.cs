using UnityEngine;
using UnityEngine.UI;

public class GenerateChip : MonoBehaviour
{
    [SerializeField]
    private GameObject ChipSlotObj;

    // private const float UNIT_SIZE = 200.0f;

    private bool generatedShape = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateVisualChip(ChipSO chipData)
    {
        if (generatedShape)
        {
            Debug.LogError("Trying to set up the chip display more than once. This does not have that function.");
            return;
        }

        Vector2Int[] blockData = chipData.GetBlockLayout();

        foreach (var block in blockData)
        {
            GameObject blockObj = Instantiate(ChipSlotObj);
            blockObj.transform.SetParent(transform);

            blockObj.transform.localPosition = new Vector3(block.x * ChipBoardMenu.UnitSize, block.y * ChipBoardMenu.UnitSize, 0);

            blockObj.GetComponent<Image>().color = chipData.GetGenerativeColor();
        }
    }
}
