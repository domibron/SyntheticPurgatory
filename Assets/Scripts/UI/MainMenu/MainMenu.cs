using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // public string HubWorldSceneName = "HubWorld";

    public void StartNewGame()
    {
        if (LevelLoading.Instance == null) return;

        if (LevelLoading.Instance.IsLoading) return;

        LevelLoading.Instance.LoadScene(LevelCollection.LevelKey.SetupScreen.ToString());
    }

    public void Quit()
    {
        Application.Quit();
    }
}
