using UnityEngine;
using UnityEngine.Serialization;


//spawns the card in for the second level gen.
public class ModuleSpawner : MonoBehaviour
{
    [SerializeField, FormerlySerializedAs("cardTier")]
    ModuleTier moduleTier;

    [SerializeField]
    private bool spawnOnStart = false;

    private GameObject upgradeCardToSpawn;

    [SerializeField]
    private GameObject temporaryObject;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        upgradeCardToSpawn = ModuleManager.Instance?.GetModulePrefab(moduleTier);

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
