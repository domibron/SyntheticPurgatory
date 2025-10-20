using UnityEngine;

// By Vincent Pressey

public class RandomizeEnvironmentPiece : MonoBehaviour
{
    /// <summary>
    /// List of objects that be used to randomly pick from
    /// </summary>
    [SerializeField]
    private GameObject[] levelpieces;
    /// <summary>
    /// Object that will be deleted after actual model is loaded
    /// </summary>
    [SerializeField]
    private GameObject temporaryPiece;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Choose random piece from given list then spawn at this object with same rotation
        Instantiate(levelpieces[Random.Range(0, levelpieces.Length - 1)], transform.position, transform.rotation, transform);

        Destroy(temporaryPiece); // Destroy the piece used for creation
        Destroy(this);
    }

}
