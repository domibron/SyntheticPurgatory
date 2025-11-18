using UnityEngine;

public class SetupScreen : MonoBehaviour
{
    public void StartGame()
    {
        if (LevelLoading.Instance == null) return;

        if (LevelLoading.Instance.IsLoading) return;

        LevelLoading.Instance.LoadScene(LevelCollection.LevelKey.DungeonWorld.ToString());
    }

    public void ReturnToMenu()
    {
        if (LevelLoading.Instance == null) return;

        if (LevelLoading.Instance.IsLoading) return;

        LevelLoading.Instance.LoadScene(LevelCollection.LevelKey.MainMenu.ToString());
    }
}
