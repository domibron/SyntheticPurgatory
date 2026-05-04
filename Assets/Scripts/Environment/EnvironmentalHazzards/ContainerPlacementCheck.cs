using Unity.AI.Navigation;
using UnityEngine;

public class ContainerPlacementCheck : MonoBehaviour
{

    BoxCollider boxCollider;

    [SerializeField]
    LayerMask layerMask;

    [SerializeField]
    FallingTileArenaManager fallingTileArenaManager;

    private Vector3 resetPoint;

    void Awake()
    {
        resetPoint = transform.position;
    }

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

    public void ResetPosition()
    {
        transform.position = resetPoint;
    }

    public bool SampleContainerPosition(Vector3 targetPos)
    {
        transform.position = targetPos;

        Collider[] colliders = Physics.OverlapBox(transform.position + boxCollider.center, boxCollider.size / 2f, transform.rotation, layerMask);

        if (colliders.Length > 0)
        {
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.CompareTag(Constants.MediumDetailTag) || collider.gameObject.CompareTag(Constants.LowDetailTag))
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
