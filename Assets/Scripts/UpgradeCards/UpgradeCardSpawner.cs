using UnityEngine;

public class UpgradeCardSpawner : MonoBehaviour
{
    [SerializeField]
    CardTier cardTier;

    [SerializeField]
    private bool spawnOnStart = false;

    private GameObject upgradeCardToSpawn;

    [SerializeField]
    private GameObject temporaryObject;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        upgradeCardToSpawn = UpgradeCardManager.Instance?.GetUpgradeCardPrefab(cardTier);

        if (spawnOnStart)
        {
            SpawnCard();
        }
    }

    public void SpawnCard()
    {
        Instantiate(upgradeCardToSpawn, transform.position, Quaternion.identity);
        Destroy(temporaryObject);
    }
}
