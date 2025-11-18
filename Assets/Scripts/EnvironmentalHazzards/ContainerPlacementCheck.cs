using Unity.AI.Navigation;
using UnityEngine;

public class ContainerPlacementCheck : MonoBehaviour
{

    BoxCollider boxCollider;

    [SerializeField]
    LayerMask layerMask;

    [SerializeField]
    FallingTileArenaManager fallingTileArenaManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        // Physics.CheckBox

        // print(SampleContainerPosition(transform.position));

    }

    public bool SampleContainerPosition(Vector3 targetPos)
    {
        transform.position = targetPos;

        Collider[] colliders = Physics.OverlapBox(transform.position + boxCollider.center, boxCollider.size / 2f, transform.rotation, layerMask);

        if (colliders.Length > 0)
        {
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.CompareTag(Constants.DecorationTag) || collider.gameObject.CompareTag(Constants.WallTag))
                {
                    return false;
                }
            }
        }

        if (fallingTileArenaManager.CheckTilesFallenInArea(transform.position + boxCollider.center, boxCollider.size / 2f))
        {
            // print("Tiles failed");
            return false;
        }



        return true;
    }
}
