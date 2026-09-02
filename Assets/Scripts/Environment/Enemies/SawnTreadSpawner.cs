using UnityEngine;

public class SawnTreadSpawner : MonoBehaviour
{
    public GameObject treadmarkObject;

    public GameObject treadCollector;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        treadCollector = new GameObject("TreadCollecter");

        InvokeRepeating("SpawnTreadmarkObject", 0.8f, 0.0125f);
    }


    public void SpawnTreadmarkObject()
    {
        Quaternion treadRotation = Quaternion.Euler(90, transform.eulerAngles.y, 0);
        GameObject treadDecal = Instantiate(treadmarkObject, transform.position, treadRotation);

        treadDecal.transform.parent = treadCollector.transform;

    }
}
