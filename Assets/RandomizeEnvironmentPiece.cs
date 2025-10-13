using UnityEngine;

public class RandomizeEnvironmentPiece : MonoBehaviour
{
    [SerializeField]
    private GameObject[] levelpieces;
    [SerializeField]
    private GameObject temporaryPiece;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instantiate(levelpieces[Random.Range(0, levelpieces.Length - 1)], transform.position, transform.rotation, transform);
        Destroy(temporaryPiece);
        Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
