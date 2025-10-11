using UnityEngine;

public class ScrapDropper : MonoBehaviour
{
    ScrapManager scrapM;
    float newScrap = 3;
    float sideForce = 3.5f;
    float upForce = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scrapM = ScrapManager.Instance;
    }

    public void SpawnScrapGroup(int scrapTotal)
    {
        bool skippedHighest = false;

        while (scrapTotal > 0)
        {
            ScrapItemData nextScrap = ScrapManager.GetPrefabWithHighestWorth(scrapTotal, ScrapManager.Instance.ScrapPrefabsWithWorth);

            if (nextScrap.ScrapWorth * 2 >= scrapTotal && !skippedHighest)
            {
                
                nextScrap = ScrapManager.GetPrefabWithHighestWorth(Mathf.FloorToInt(scrapTotal / 2), ScrapManager.Instance.ScrapPrefabsWithWorth);

                GameObject newScrap = scrapM.SpawnScrap(nextScrap.ScrapWorth, transform.position);
                YeetScrap(newScrap, sideForce, upForce);

                newScrap = scrapM.SpawnScrap(nextScrap.ScrapWorth, transform.position);
                YeetScrap(newScrap, sideForce, upForce);

                scrapTotal -= nextScrap.ScrapWorth * 2;

                skippedHighest = true;
                print("test");
            }
            else
            {
                GameObject newScrap = scrapM.SpawnScrap(nextScrap.ScrapWorth, transform.position);
                YeetScrap(newScrap, sideForce, upForce);

                scrapTotal -= nextScrap.ScrapWorth;
            }




        }

    
    }


    public void YeetScrap(GameObject scrap, float xzForce, float yForce)
    {

        float angle = Random.Range(0, 359) * Mathf.PI / 180;
        float angleX = Mathf.Cos(angle);
        float angleZ = Mathf.Sin(angle);


        Vector2 scrapDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

        scrap.GetComponent<Rigidbody>().AddForce(new Vector3(angleX * xzForce, yForce, angleZ * xzForce), ForceMode.Impulse);

    }
}
