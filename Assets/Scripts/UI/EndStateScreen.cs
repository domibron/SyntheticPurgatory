using UnityEngine;
using TMPro;

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
        UpdateStats();
        endStateCanvasCollection.SetActive(state);
    }

    public void ReturnToMenu()
    {
        if (LevelLoading.Instance == null) return;

        if (LevelLoading.Instance.IsLoading) return;

        LevelLoading.Instance.LoadMainMenu();
    }


    // Mmm variables

    [SerializeField] private Transform runTime; // Time passed while in level
    [SerializeField] private Transform deaths; // Instances of player dying
    [SerializeField] private Transform difficulty;
    [SerializeField] private Transform outcome; // Whether or not player beat the boss
    [SerializeField] private Transform levelsCleared; // Number of levels passed
    [SerializeField] private Transform totalScrap; // Score from all scrap collected across run
    [SerializeField] private Transform enemiesDefeated; // Number of enemies defeated + total value
    [SerializeField] private Transform damageDealt; // Damage dealt to other objects
    [SerializeField] private Transform damageReceived; // Damage taken from environment/enemies
    [SerializeField] private Transform todPunts;
    [SerializeField] private Transform totalPoints;

    public void UpdateStats()
    {
        int calculatedScore = 0;
        int curHeldScore;

        GameManager gm = GameManager.Instance;


        // TIMED SCORE
        runTime.GetChild(0).GetComponent<TMP_Text>().text = gm.statsHolder.runTime.ToString();
        curHeldScore = (int)(1000 - Mathf.Floor(gm.statsHolder.runTime / 2));
        runTime.GetChild(1).GetComponent<TMP_Text>().text = curHeldScore.ToString();
        calculatedScore += curHeldScore;

        // DEATHS PENALTY
        deaths.GetChild(0).GetComponent<TMP_Text>().text = gm.statsHolder.deaths.ToString();
        curHeldScore = (gm.statsHolder.deaths * -500);
        deaths.GetChild(1).GetComponent<TMP_Text>().text = curHeldScore.ToString();
        calculatedScore += curHeldScore;

        // GAME OUTCOME SCORE
        if (GameManager.Instance.statsHolder.outcome) // Won
        {
            outcome.GetChild(0).GetComponent<TMP_Text>().text = "Victory";
            outcome.GetChild(1).GetComponent<TMP_Text>().text = "4000";
            calculatedScore += 4000;
        }
        else // Lost
        {
            outcome.GetChild(0).GetComponent<TMP_Text>().text = "Defeat";
            outcome.GetChild(1).GetComponent<TMP_Text>().text = "0";
        }

        // ENEMIES DEFEATED SCORE
        enemiesDefeated.GetChild(0).GetComponent<TMP_Text>().text = gm.statsHolder.enemiesDefeated.ToString();
        enemiesDefeated.GetChild(1).GetComponent<TMP_Text>().text = gm.statsHolder.enemiesDefeatedScore.ToString();
        calculatedScore += gm.statsHolder.enemiesDefeatedScore;

        // SCRAP COLLECTION SCORE
        totalScrap.GetChild(0).GetComponent<TMP_Text>().text = gm.statsHolder.totalScrap.ToString();
        curHeldScore = (gm.statsHolder.totalScrap * 3);
        totalScrap.GetChild(1).GetComponent<TMP_Text>().text = curHeldScore.ToString();
        calculatedScore += curHeldScore;

        // DAMAGE DEALT SCORE
        damageDealt.GetChild(0).GetComponent<TMP_Text>().text = ((int)gm.statsHolder.damageDealt).ToString();
        curHeldScore = (int)(gm.statsHolder.damageDealt / 20);
        damageDealt.GetChild(1).GetComponent<TMP_Text>().text = curHeldScore.ToString();
        calculatedScore += curHeldScore;

        //// DAMAGE TAKEN PENALTY
        damageReceived.GetChild(0).GetComponent<TMP_Text>().text = ((int)gm.statsHolder.damageReceived).ToString();
        curHeldScore = (int)(-gm.statsHolder.damageReceived / 5);
        damageReceived.GetChild(1).GetComponent<TMP_Text>().text = curHeldScore.ToString();
        calculatedScore += curHeldScore;

        // TOD SCORE
        curHeldScore = gm.statsHolder.todPunts;
        todPunts.GetChild(0).GetComponent<TMP_Text>().text = curHeldScore.ToString();
        todPunts.GetChild(1).GetComponent<TMP_Text>().text = curHeldScore.ToString();
        calculatedScore += curHeldScore;

        // DEATHS MULTIPLIER
        difficulty.GetChild(0).GetComponent<TMP_Text>().text = gm.GetCurrentDifficulty().ToString();
        difficulty.GetChild(1).GetComponent<TMP_Text>().text = ("x" + gm.GetCurrentDifficulty().ToString()); // NEED MULTIPLIER HERE





        // FINAL SCORE
        totalPoints.GetChild(0).GetComponent<TMP_Text>().text = calculatedScore.ToString();


        // LEVELS CLEARED TEXT (Not a scorable stat as it can be abused)
        levelsCleared.GetChild(0).GetComponent<TMP_Text>().text = (GameManager.Instance.GetCurrentLevel() - 2).ToString();
    }



}
