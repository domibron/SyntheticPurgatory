using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // public string HubWorldSceneName = "HubWorld";

    public void StartNewGame()
    {
        // should have a little menu for chips but we load into the level for now.
        if (LevelLoading.Instance == null) return;

        if (LevelLoading.Instance.IsLoading) return;

        LevelLoading.Instance.LoadScene(LevelCollection.LevelKey.HubWorld.ToString());
    }

    public void Quit()
    {
        Application.Quit();
    }
}
