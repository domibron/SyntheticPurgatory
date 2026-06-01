using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// Loads the level with the specified name.
/// </summary>
public class LoadNextLevel : MonoBehaviour
{
    /// <summary>
    /// The name of the level in the scene index to load.
    /// </summary>
    public string NameOfLevel;

    /// <summary>
    /// Load the level specified with <see cref="NameOfLevel"/>.
    /// </summary>
    public void LoadLevel()
    {
        if (LevelLoading.Instance != null)
        {
            if (LevelCollection.CheckSceneInCollection(NameOfLevel))
            {
                LevelLoading.Instance.LoadScene(LevelCollection.GetCollectionNameFromScene(NameOfLevel));
            }
        }
        else
            SceneManager.LoadSceneAsync(NameOfLevel);
    }
}
