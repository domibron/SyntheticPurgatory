using UnityEngine;


/// <summary>
/// Spawns scrap and adds forces to them.
/// </summary>
public class ScrapDropper : MonoBehaviour
{
    ScrapLevelM scrapM;

    /// <summary>
    /// Offset of position for spawning scrap
    /// </summary>
    [SerializeField]
    private Vector3 spawnOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scrapM = ScrapLevelM.Instance;
    }

    /// <summary>
    /// Spawns and scatters a group of scrap in random directions
    /// </summary>
    /// <param name="scrapTotal">Total value of scrap to spawn</param>
    /// <param name="xzForce">Force applied horizontally on spawn</param>
    /// <param name="yForce">Force applied vertically on spawn</param>
    public void SpawnScrapGroup(int scrapTotal, float xzForce, float yForce)
    {
        bool skippedHighest = false; // Prioritize quantity over spawning highest value scrap

        while (scrapTotal > 0) // Keep spawning until total value is exhausted
        {
            if (ScrapLevelM.Instance == null) { scrapTotal = 0; return; }
            ScrapItemData nextScrap = ScrapLevelM.GetPrefabWithHighestWorth(scrapTotal, ScrapLevelM.Instance.ScrapPrefabsWithWorth);

            if (nextScrap.ScrapWorth * 2 >= scrapTotal && !skippedHighest && nextScrap.ScrapWorth != 1) // Check if can't spawn two of highest value, skips this if only one scrap is left
            {
                // Get second highest value scrap, assumes that it is halve the value of the previous
                nextScrap = ScrapLevelM.GetPrefabWithHighestWorth(Mathf.FloorToInt(scrapTotal / 2), ScrapLevelM.Instance.ScrapPrefabsWithWorth);

                GameObject newScrap = scrapM.SpawnScrap(nextScrap.ScrapWorth, transform.position + spawnOffset); // Spawns first scrap object
                Utils.ThrowObject(newScrap, xzForce, yForce);

                newScrap = scrapM.SpawnScrap(nextScrap.ScrapWorth, transform.position + spawnOffset); // Spawns second scrap object
                Utils.ThrowObject(newScrap, xzForce, yForce);

                scrapTotal -= nextScrap.ScrapWorth * 2; // Subtracts value of the two spawned scrap from total

                skippedHighest = true; // Skips to else outcome below after highest value is converted to double lower value scrap
            }
            else // Normal scrap spawning method, overall works like binary
            {
                GameObject newScrap = scrapM.SpawnScrap(nextScrap.ScrapWorth, transform.position + spawnOffset); // Spawns scrap object
                Utils.ThrowObject(newScrap, xzForce, yForce);

                scrapTotal -= nextScrap.ScrapWorth;
            }

        }
    }



}
