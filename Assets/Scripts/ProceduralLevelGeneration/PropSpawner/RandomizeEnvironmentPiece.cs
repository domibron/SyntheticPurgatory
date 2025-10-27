using UnityEngine;

// By Vincent Pressey

public class RandomizeEnvironmentPiece : MonoBehaviour
{
    /// <summary>
    /// List of objects that be used to randomly pick from
    /// </summary>
    [SerializeField]
    private GameObject[] objectPrefabs;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (noActivationChance > Random.Range(0, 99))
        {
            // print(Random.Range(0, 99));
            Destroy(temporaryPiece); // Destroy the piece used for creation
            Destroy(this);
            return;
        }

        // Choose random piece from given list then spawn at this object with same rotation
        GameObject newobject = Instantiate(objectPrefabs[Random.Range(0, objectPrefabs.Length - 1)], transform.position, transform.rotation, transform);
        if (randomiseYRotation)
        {
            newobject.transform.rotation = Quaternion.Euler(newobject.transform.rotation.eulerAngles.x, Random.Range(0, 359), newobject.transform.rotation.eulerAngles.z);
        }

        Destroy(temporaryPiece); // Destroy the piece used for creation
        Destroy(this);
    }

}
