using UnityEngine;
using TMPro;

// By Vincent Pressey

public class DeathCanvas : MonoBehaviour
{
    /// <summary>
    /// Object on the canvas that contains all the death canvas GUI
    /// </summary>
    [SerializeField]
    private GameObject deathCanvasCollection;
    /// <summary>
    /// Text Object for displaying deposited scrap text
    /// </summary>
    [SerializeField]
    private TMP_Text depoScrapStatText;
    /// <summary>
    /// Text Object for displaying lost scrap stat
    /// </summary>
    [SerializeField]
    private TMP_Text lostScrapStatText;


    /// <summary>
    /// Activate and enable visibility of the death canvas
    /// </summary>
    /// <param name="state">Whether to turn on or off the death canvas</param>
    public void ActivateCanvas(bool state)
    {
        deathCanvasCollection.SetActive(state);
    }


    /// <summary>
    /// Show the stats of the level onto the screen
    /// </summary>
    /// <param name="scrapDeposited">Total value for the total scrap deposited</param>
    /// <param name="scrapLost">Value of the held scrap on death</param>
    public void ShowStats(int scrapDeposited, int scrapLost)
    {
        depoScrapStatText.text = (depoScrapStatText.text + scrapDeposited);
        lostScrapStatText.text = (lostScrapStatText.text + scrapLost);
    }


    /// <summary>
    /// Return back to hub
    /// </summary>
    public void ReturnToHUB()
    {
        GameManager.Instance.ReturnToHubWorld();
    }
}
