using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ScrapCollectable : CollectableBase
{

    [SerializeField]
    int scrapWorth = 1;

    // [SerializeField]
    // float flyAccel = 5f; // TODO static constant thing.

    // [SerializeField]
    // float flyMaxSpeed = 15f;

    // [SerializeField]
    // float flyDistanceBoost = 10f;

    // Update is called once per frame

    public void Initialize(int scrapWorth)
    {
        this.scrapWorth = scrapWorth;


    }

    protected override void CollectItem()
    {
        // 
        if (!CanPlayerCollect()) return;

        if (Vector3.Distance(transform.position, playerTransform.position) > collectItemRange) return;

        // we drop any scrap we cannot fit into the inventory.
        int remaining = ScrapManager.Instance.CollectScrap(scrapWorth);

        // spawn the remaning scrap as objects in the world.
        while (remaining > 0) // fingers cross this doesn't fuck up. It will in the future and cause a stutter, you watch. // Dont feel anything lol.
        {
            ScrapItemData prefabToSpawn = ScrapManager.GetPrefabWithHighestWorth(remaining, ScrapManager.Instance.ScrapPrefabsWithWorth);

            Instantiate(prefabToSpawn.ScrapPrefab, transform.position, Quaternion.identity);

            remaining -= prefabToSpawn.ScrapWorth;
        }

        Destroy(gameObject);
    }


}
