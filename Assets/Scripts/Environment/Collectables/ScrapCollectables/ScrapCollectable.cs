using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ScrapCollectable : CollectableBase
{
    [SerializeField]
    int scrapWorth = 1;


    public void Initialize(int scrapWorth)
    {
        this.scrapWorth = scrapWorth;
    }

    /// <summary>
    /// Adds as much scrap into the player's inventory.
    /// </summary>
    protected override void CollectItem()
    {
        // 
        if (!CanTargetCollect()) return;

        if (Vector3.Distance(transform.position, targetTransform.position) > collectItemRange) return;

        // we drop any scrap we cannot fit into the inventory.
        int remaining = ScrapManager.Instance.CollectScrap(scrapWorth);

        // spawn the remaining scrap as objects in the world.
        while (remaining > 0) // slight stutter but barely noticeable, could replace with coroutine?
        {
            ScrapItemData prefabToSpawn = ScrapManager.GetPrefabWithHighestWorth(remaining, ScrapManager.Instance.ScrapPrefabsWithWorth);

            Instantiate(prefabToSpawn.ScrapPrefab, transform.position, Quaternion.identity);

            remaining -= prefabToSpawn.ScrapWorth;
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Does the player have any available inventory space.
    /// </summary>
    /// <returns>True if there is some space.</returns>
    protected override bool CanTargetCollect()
    {
        if (!base.CanTargetCollect()) return false;

        if (!ScrapManager.Instance.HaveInventorySpace())
        {
            ScrapManager.Instance.InvokeOnInventoryFull();
            return false;
        }

        return true;
    }



}
