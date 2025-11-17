using Unity.AI.Navigation;
using UnityEngine;

public class ContainerPlacementCheck : MonoBehaviour
{

    BoxCollider boxCollider;

    [SerializeField]
    Transform backLeftCorner;

    [SerializeField]
    Transform frontRightCorner;

    [SerializeField]
    LayerMask layerMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        // Physics.CheckBox


    }

    public bool SampleContainerPosition(Vector3 targetPos)
    {
        transform.position = targetPos;

        Collider[] colliders = Physics.OverlapBox(transform.position + boxCollider.center, boxCollider.size / 2f, transform.rotation, layerMask);

        if (colliders.Length > 0) return false;

        return true; // ! TEMP
    }
}
