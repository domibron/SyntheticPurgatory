using UnityEngine;

public class EnemyGroupSpawner : MonoBehaviour
{
    /// <summary>
    /// Tier of enemy groups to spawn
    /// </summary>
    [SerializeField, Range(1,3)]
    private int groupTier = 1;
    /// <summary>
    /// List of enemy group objects counted as 'tier 1'
    /// </summary>
    [SerializeField]
    private GameObject[] tier1GroupPrefabs;
    /// <summary>
    /// List of enemy group objects counted as 'tier 2'
    /// </summary>
    [SerializeField]
    private GameObject[] tier2GroupPrefabs;
    /// <summary>
    /// List of enemy group objects counted as 'tier 3'
    /// </summary>
    [SerializeField]
    private GameObject[] tier3GroupPrefabs;
    /// <summary>
    /// Object that will be deleted after actual model is loaded
    /// </summary>
    [SerializeField]
    private GameObject temporaryPiece;
    /// <summary>
    /// Whether or not to randomise the Y-axis rotation
    /// </summary>
    [SerializeField]
    private bool randomiseYRotation;
    /// <summary>
    /// Percentage chance for object to delete itself before generating anything
    /// </summary>
    [SerializeField, Range(0, 100)]
    private float noActivationChance = 0;
    /// <summary>
    /// Percentage chance for group to improve tier
    /// </summary>
    [SerializeField]
    private float tierUpChance = 0;
    /// <summary>
    /// Percentage chance to lower group tier
    /// </summary>
    [SerializeField]
    private float tierDownChance = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        LevelGenObjectRefGetter.Instance.GetComponent<Sequencer>().OnSequencesEnd += SpawnEnemies;
    }

    void SpawnEnemies()
    {
        if (noActivationChance > Random.Range(0, 99))
        {
            // print(Random.Range(0, 99));
            Destroy(temporaryPiece); // Destroy the piece used for creation
            Destroy(this);
            return;
        }

        if (tierUpChance > Random.Range(0, 99))
        {
            groupTier = Mathf.Min(groupTier++, 3);
        }
        if (tierDownChance > Random.Range(0, 99))
        {
            groupTier = Mathf.Max(groupTier--, 1);
        }


        GameObject[] chosenGroup;
        chosenGroup = tier1GroupPrefabs;

        switch (groupTier)
        {
            case 1:
                chosenGroup = tier1GroupPrefabs;
                break;
            case 2:
                chosenGroup = tier2GroupPrefabs;
                break;
            case 3:
                chosenGroup = tier3GroupPrefabs;
                break;
        }


        // Choose random piece from given list then spawn at this object with same rotation
        GameObject newobject = Instantiate(chosenGroup[Random.Range(0, chosenGroup.Length - 1)], transform.position, transform.rotation, transform);
        if (randomiseYRotation)
        {
            newobject.transform.rotation = Quaternion.Euler(newobject.transform.rotation.eulerAngles.x, Random.Range(0, 359), newobject.transform.rotation.eulerAngles.z);
        }

        Destroy(temporaryPiece); // Destroy the piece used for creation
        Destroy(this);
    }

}



