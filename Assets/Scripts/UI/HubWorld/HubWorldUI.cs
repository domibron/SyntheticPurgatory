using UnityEngine;

public class HubWorldUI : MonoBehaviour
{
    public string DungeonWorldSceneName = "DungeonWorld";
    public string BossWorldSceneName = "BossWorld";
    public string MainMenuSceneName = "MainMenu";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartNextRun()
    {
        if (LevelLoading.Instance != null)
            LevelLoading.Instance.LoadScene(DungeonWorldSceneName);
    }

    public void LoadMainMenu()
    {
        if (LevelLoading.Instance != null)
            LevelLoading.Instance.LoadScene(MainMenuSceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
