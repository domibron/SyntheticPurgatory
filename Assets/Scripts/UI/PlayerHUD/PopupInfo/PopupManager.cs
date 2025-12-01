using UnityEngine;

public class PopupManager : MonoBehaviour
{
    [SerializeField]
    private GameObject popupTextPrefab;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnPopupText("Some stinky message");
        }
    }

    private void SpawnPopupText(string text, Sprite sprite = null)
    {
        PopupInfoText popupInfoText = Instantiate(popupTextPrefab, transform).GetComponent<PopupInfoText>();

        popupInfoText.Initialize(text, sprite);

    }
}
