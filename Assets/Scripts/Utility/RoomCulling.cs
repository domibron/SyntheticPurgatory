using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RoomCulling : MonoBehaviour
{
    List<MeshRenderer> meshRenderers = new List<MeshRenderer>();

    Transform player;

    float maxDistance = 64;

    BoxCollider boxCollider;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = false;
    }

    void OnValidate()
    {
        boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
            boxCollider.enabled = false;
    }

    void Start()
    {
        player = PlayerRefFetcher.Instance.GetPlayerRef().transform;

        // TODO: figure this out later, should not be a issue since rooms are square.
        // bounds.extents = transform.localRotation * bounds.extents; // rotate the extents.
        // bounds.center = transform.localRotation * bounds.center;

        // TODO: remove meshes that can move.


        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer.transform.GetComponent<Rigidbody>() != null)
            {
                continue;
            }

            meshRenderers.Add(renderer);
        }
    }

    void Update()
    {
        if (PlayerWithinRange())
        {
            foreach (var meshRenderer in meshRenderers)
            {
                if (meshRenderer.enabled) continue;
                meshRenderer.enabled = true;
            }
        }
        else
        {
            foreach (var meshRenderer in meshRenderers)
            {
                if (!meshRenderer.enabled) continue;
                meshRenderer.enabled = false;
            }
        }
    }

    bool PlayerWithinRange()
    {
        Vector3 playerPos = player.position;

        bool isEven = (transform.rotation.eulerAngles.y <= 0.01f && transform.rotation.eulerAngles.y >= -0.01f) ? true : (Mathf.FloorToInt(transform.rotation.eulerAngles.y / 90f) % 2) == 0;
        Vector3 sizeRotated = boxCollider.size;

        if (!isEven)
        {
            sizeRotated.z = boxCollider.size.x;
            sizeRotated.x = boxCollider.size.z;
        }

        Vector3 lowerBounds = transform.position + boxCollider.center - (sizeRotated / 2f);
        Vector3 upperBounds = transform.position + boxCollider.center + (sizeRotated / 2f);

        if (OutsideLowerBounds(playerPos, lowerBounds, maxDistance) || OutsideUpperBounds(playerPos, upperBounds, maxDistance))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    // void OnDrawGizmos()
    // {
    //     BoxCollider bc = GetComponent<BoxCollider>();
    //     Gizmos.DrawWireCube(transform.position + bc.center, bc.size);

    //     bool isEven = (transform.rotation.eulerAngles.y <= 0.01f && transform.rotation.eulerAngles.y >= -0.01f) ? true : (Mathf.FloorToInt(transform.rotation.eulerAngles.y / 90f) % 2) == 0;
    //     Vector3 sizeRotated = boxCollider.size;

    //     if (!isEven)
    //     {
    //         sizeRotated.z = boxCollider.size.x;
    //         sizeRotated.x = boxCollider.size.z;
    //     }

    //     Gizmos.DrawSphere(transform.position + boxCollider.center - (sizeRotated / 2f), 0.1f);
    //     Gizmos.DrawSphere(transform.position + boxCollider.center + (sizeRotated / 2f), 0.1f);
    // }

    bool OutsideLowerBounds(Vector3 playerPos, Vector3 lowerBounds, float maxDistanceFromRoom)
    {
        return playerPos.x < lowerBounds.x - maxDistanceFromRoom || playerPos.y < lowerBounds.y - maxDistanceFromRoom || playerPos.z < lowerBounds.z - maxDistanceFromRoom;
    }

    bool OutsideUpperBounds(Vector3 playerPos, Vector3 upperBounds, float maxDistanceFromRoom)
    {
        return playerPos.x > upperBounds.x + maxDistanceFromRoom || playerPos.y > upperBounds.y + maxDistanceFromRoom || playerPos.z > upperBounds.z + maxDistanceFromRoom;
    }
}
