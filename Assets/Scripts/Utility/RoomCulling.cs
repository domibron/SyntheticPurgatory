using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum VisibleState
{
    Unload,
    Minimal,
    Medium,
    Maximum,
}

[RequireComponent(typeof(BoxCollider))]
public class RoomCulling : MonoBehaviour
{
    List<MeshRenderer> wallsAndFloorMeshRenderers = new List<MeshRenderer>();
    List<MeshRenderer> decorMeshRenderers = new List<MeshRenderer>();
    List<EntityCulling> entityCullings = new List<EntityCulling>();

    // Transform player;

    // float maxDistance = 64;

    BoxCollider boxCollider;

    private bool isReady = false;

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

    IEnumerator Start() // TODO, jank fix, will replace with proper que system. aka event hook.
    {
        // while (PlayerRefFetcher.Instance == null)
        // {
        //     yield return null;
        // }

        // player = PlayerRefFetcher.Instance.GetPlayerRef().transform;

        // TODO: figure this out later, should not be a issue since rooms are square.
        // bounds.extents = transform.localRotation * bounds.extents; // rotate the extents.
        // bounds.center = transform.localRotation * bounds.center;

        // TODO: remove meshes that can move.

        yield return new WaitForSeconds(1);

        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer.transform.GetComponent<Rigidbody>() != null)
            {
                continue;
            }

            if (renderer.gameObject.CompareTag(Constants.WallTag) || renderer.gameObject.CompareTag(Constants.FloorTag))
            {
                wallsAndFloorMeshRenderers.Add(renderer);
            }
            else if (renderer.gameObject.CompareTag(Constants.DecorationTag))
            {
                decorMeshRenderers.Add(renderer);
            }

        }

        EntityCulling[] gatheredEntityCullings = GetComponentsInChildren<EntityCulling>();

        entityCullings.AddRange(gatheredEntityCullings);

        foreach (EntityCulling entityCulling in entityCullings)
        {
            entityCulling.OverrideCulling();
            entityCulling.TryOverrideMeshVisiblity(false);
        }

        isReady = true;

    }

    // void Update()
    // {
    //     if (PlayerWithinRange())
    //     {
    //         foreach (var meshRenderer in meshRenderers)
    //         {
    //             if (meshRenderer.enabled) continue;
    //             meshRenderer.enabled = true;
    //         }
    //     }
    //     else
    //     {
    //         foreach (var meshRenderer in meshRenderers)
    //         {
    //             if (!meshRenderer.enabled) continue;
    //             meshRenderer.enabled = false;
    //         }
    //     }
    // }

    public void SetRendererState(VisibleState state)
    {
        switch (state) // I do feel this is a bit bad, but eh, fuck it.
        {
            case VisibleState.Unload: // everything else
                SetWallsAndFloorRendererState(false);
                SetDecorRendererState(false);
                SetEntityCulling(false);
                break;
            case VisibleState.Minimal: // 2nd layer rooms
                SetWallsAndFloorRendererState(true); // need to set LOD state.
                SetDecorRendererState(false);
                SetEntityCulling(false);
                break;
            case VisibleState.Medium: // 1st layer rooms
                SetWallsAndFloorRendererState(true); // need to set LOD state.
                SetDecorRendererState(true); // need two levels of detail. lower and higher
                SetEntityCulling(true);
                break;
            case VisibleState.Maximum: // Current room
                SetWallsAndFloorRendererState(true);
                SetDecorRendererState(true);
                SetEntityCulling(true);
                break;
        }
    }

    private void SetEntityCulling(bool isVusible = true)
    {
        StartCoroutine(SetEntityCullingRenderState(isVusible));
    }

    IEnumerator SetEntityCullingRenderState(bool isVusible)
    {
        foreach (EntityCulling entity in entityCullings)
        {
            if (entity == null) continue; // TODO: should have enemies or what ever not be collected.


            entity.TryOverrideMeshVisiblity(isVusible);
        }
        yield return null;
    }

    private void SetWallsAndFloorRendererState(bool isVisible = true)
    {
        // print("render state " + isVisible);

        StartCoroutine(SetRenderStateWhenReady(isVisible, wallsAndFloorMeshRenderers));
    }

    private void SetDecorRendererState(bool isVisible = true)
    {
        // print("render state " + isVisible);

        StartCoroutine(SetRenderStateWhenReady(isVisible, decorMeshRenderers));
    }

    IEnumerator SetRenderStateWhenReady(bool isVisible, List<MeshRenderer> meshRenderers)
    {

        while (!isReady) yield return null; // wait until meshes are gathered.


        foreach (var meshRenderer in meshRenderers)
        {
            if (meshRenderer.enabled == isVisible) continue;

            meshRenderer.enabled = isVisible;
        }

    }

    // bool PlayerWithinRange()
    // {
    //     if (player == null) return false;

    //     Vector3 playerPos = player.position;

    //     bool isEven = (transform.rotation.eulerAngles.y <= 0.01f && transform.rotation.eulerAngles.y >= -0.01f) ? true : (Mathf.FloorToInt(transform.rotation.eulerAngles.y / 90f) % 2) == 0;
    //     Vector3 sizeRotated = boxCollider.size;

    //     if (!isEven)
    //     {
    //         sizeRotated.z = boxCollider.size.x;
    //         sizeRotated.x = boxCollider.size.z;
    //     }

    //     Vector3 lowerBounds = transform.position + (transform.rotation * boxCollider.center) - (sizeRotated / 2f);
    //     Vector3 upperBounds = transform.position + (transform.rotation * boxCollider.center) + (sizeRotated / 2f);

    //     if (OutsideLowerBounds(playerPos, lowerBounds, maxDistance) || OutsideUpperBounds(playerPos, upperBounds, maxDistance))
    //     {
    //         return false;
    //     }
    //     else
    //     {
    //         return true;
    //     }
    // }

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
