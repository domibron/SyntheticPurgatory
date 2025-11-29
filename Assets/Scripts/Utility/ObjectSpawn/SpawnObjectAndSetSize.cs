using UnityEngine;

public class SpawnObjectAndSetSize : MonoBehaviour
{
    [SerializeField]
    private GameObject prefabToSpawn;

    [SerializeField]
    private bool addPositionOffset = false;

    [SerializeField]
    private Vector3 spawnOffset;

    [SerializeField]
    private bool addRotationOffset = false;

    [SerializeField]
    private Vector3 rotationOffset;

    [SerializeField]
    private bool setScale = false;

    [SerializeField]
    private Vector3 scale = Vector3.one;


    public void SpawnObject()
    {
        GameObject objectThatSpawned = Instantiate(prefabToSpawn, transform.position, transform.rotation);
        if (addPositionOffset) objectThatSpawned.transform.position += spawnOffset;
        if (addRotationOffset) objectThatSpawned.transform.Rotate(rotationOffset);
        if (setScale) objectThatSpawned.transform.localScale = scale;
    }
}
