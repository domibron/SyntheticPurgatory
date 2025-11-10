using UnityEngine;

public class UpgradeCardSpawner : MonoBehaviour
{
    [SerializeField]
    CardTeir carTier;

    [SerializeField]
    private bool spawnOnStart = false;

    private GameObject upgradeCardToSpawn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        upgradeCardToSpawn = UpgradeCardManager.Instance?.GetUpgradeCardPrefab(carTier);

        if (spawnOnStart)
        {
            SpawnCard();
        }
    }

    public void SpawnCard()
    {
        Instantiate(upgradeCardToSpawn, transform.position, Quaternion.identity);
    }
}