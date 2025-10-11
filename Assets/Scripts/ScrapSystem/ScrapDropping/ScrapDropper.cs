using UnityEngine;

// By Vince Pressey

public class ScrapDropper : MonoBehaviour
{
    ScrapManager scrapM;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scrapM = ScrapManager.Instance;
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
            ScrapItemData nextScrap = ScrapManager.GetPrefabWithHighestWorth(scrapTotal, ScrapManager.Instance.ScrapPrefabsWithWorth);

            if (nextScrap.ScrapWorth * 2 >= scrapTotal && !skippedHighest && nextScrap.ScrapWorth != 1) // Check if can't spawn two of highest value, skips this if only one scrap is left
            {
                // Get second highest value scrap, assumes that it is halve the value of the previous
                nextScrap = ScrapManager.GetPrefabWithHighestWorth(Mathf.FloorToInt(scrapTotal / 2), ScrapManager.Instance.ScrapPrefabsWithWorth);

                GameObject newScrap = scrapM.SpawnScrap(nextScrap.ScrapWorth, transform.position); // Spawns first scrap object
                YeetObject(newScrap, xzForce, yForce);

                newScrap = scrapM.SpawnScrap(nextScrap.ScrapWorth, transform.position); // Spawns second scrap object
                YeetObject(newScrap, xzForce, yForce);

                scrapTotal -= nextScrap.ScrapWorth * 2; // Subtracts value of the two spawned scrap from total

                skippedHighest = true; // Skips to else outcome below after highest value is converted to double lower value scrap
            }
            else // Normal scrap spawning method, overall works like binary
            {
                GameObject newScrap = scrapM.SpawnScrap(nextScrap.ScrapWorth, transform.position); // Spawns scrap object
                YeetObject(newScrap, xzForce, yForce);

                scrapTotal -= nextScrap.ScrapWorth;
            }

        }
    }


    /// <summary>
    /// Yeet an object in a random direction
    /// </summary>
    /// <param name="toYeet">Object that will be flung</param>
    /// <param name="xzForce">Force applied horizontally</param>
    /// <param name="yForce">Force applied vertically</param>
    public void YeetObject(GameObject toYeet, float xzForce, float yForce)
    {
        // Choose random direction
        float angle = Random.Range(0, 359) * Mathf.PI / 180;
        float dirX = Mathf.Cos(angle);
        float dirZ = Mathf.Sin(angle);

        // Add force to object in the previously created direction, multiplied changeable force and additional random number for variety
        toYeet.GetComponent<Rigidbody>().AddForce
            (new Vector3(dirX * xzForce * Random.Range(0.8f, 1.2f), yForce, dirZ * xzForce * Random.Range(0.8f, 1.2f)), ForceMode.Impulse);

    }
}
