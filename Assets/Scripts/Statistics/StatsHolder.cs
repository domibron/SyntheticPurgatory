using UnityEngine;

public class StatsHolder
{

    public float runTime = 0; // Time passed while in level
    public int deaths = 0;
    public bool outcome = false;
    public int totalScrap = 0;
    public int enemiesDefeated = 0;
    public int enemiesDefeatedScore = 0;
    public float damageDealt = 0;
    public float damageReceived = 0;
    public int todPunts = 0;

    public void LoseLife()
    {
        deaths++;
    }
}
