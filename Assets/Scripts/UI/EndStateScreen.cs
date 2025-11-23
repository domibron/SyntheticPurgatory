using UnityEngine;

public class EndStateScreen : MonoBehaviour
{
    /// <summary>
    /// Object on the canvas that contains all the endstate canvas GUI
    /// </summary>
    [SerializeField]
    private GameObject endStateCanvasCollection;
    /// <summary>
    /// Bool for checking if the death canvas is open
    /// </summary>
    private bool canvasActive = false;

    /// <summary>
    /// Activate and enable visibility of the endstate canvas
    /// </summary>
    /// <param name="state">Whether to turn on or off the endstate canvas</param>
    public void ActivateCanvas(bool state)
    {
        if (canvasActive == state) { return; }

        canvasActive = state;
        endStateCanvasCollection.SetActive(state);
    }

    public void ReturnToMenu()
    {
        if (LevelLoading.Instance == null) return;

        if (LevelLoading.Instance.IsLoading) return;

        LevelLoading.Instance.LoadMainMenu();
    }
}
