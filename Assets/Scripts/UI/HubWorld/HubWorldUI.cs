using TMPro;
using UnityEngine;

public class HubWorldUI : MonoBehaviour
{
    // public string DungeonWorldSceneName = "DungeonWorld";
    // public string BossWorldSceneName = "BossWorld";
    // public string MainMenuSceneName = "MainMenu";

    public TMP_Text ScrapText;

    public GameObject MainUI;
    public GameObject UpgradeUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OpenMainScene();
    }

    // Update is called once per frame
    void Update()
    {
        ScrapText.text = $"Scrap: {GameManager.Instance.GetCurrentScrapCount()}";

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartNextRun()
    {
        if (LevelLoading.Instance != null)
            LevelLoading.Instance.LoadScene(LevelCollection.LevelKey.DungeonWorld.ToString());
    }

    public void LoadMainMenu()
    {
        if (GameManager.Instance != null)
        {
            // remove this so no issues occur.
            Destroy(GameManager.Instance.gameObject);
        }

        if (LevelLoading.Instance != null)
            LevelLoading.Instance.LoadScene(LevelCollection.LevelKey.MainMenu.ToString());
    }

    public void OpenUpgradeScreen()
    {
        MainUI.SetActive(false);
        UpgradeUI.SetActive(true);
    }

    public void OpenMainScene()
    {
        MainUI.SetActive(true);
        UpgradeUI.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
