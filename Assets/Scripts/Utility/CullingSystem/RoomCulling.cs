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
    List<MeshRenderer> lowDetail = new List<MeshRenderer>();
    List<MeshRenderer> mediumDetail = new List<MeshRenderer>();
    List<MeshRenderer> highDetail = new List<MeshRenderer>();
    List<EntityCulling> entities = new List<EntityCulling>();
    List<Animator> animators = new List<Animator>();

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

    void Start() // TODO, jank fix, will replace with proper que system. aka event hook.
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

        // yield return new WaitForSeconds(1);

        // SetUpRoomCulling();

    }

    public void SetupRoomCulling()
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer.transform.GetComponent<Rigidbody>() != null)
            {
                continue;
            }

            if (renderer.gameObject.CompareTag(Constants.LowDetailTag))
            {
                lowDetail.Add(renderer);
            }
            else if (renderer.gameObject.CompareTag(Constants.MediumDetailTag))
            {
                mediumDetail.Add(renderer);
            }
            else if (renderer.gameObject.CompareTag(Constants.HighDetailTag))
            {
                highDetail.Add(renderer);
            }

        }

        Animator[] collectedAnimators = GetComponentsInChildren<Animator>();

        foreach (Animator animator in collectedAnimators)
        {
            if (animator.GetComponent<Rigidbody>()) continue;

            if (animator.gameObject.CompareTag(Constants.EnemyTag)) continue;

            animators.Add(animator);
        }

        EntityCulling[] gatheredEntityCullings = GetComponentsInChildren<EntityCulling>();

        entities.AddRange(gatheredEntityCullings);

        foreach (EntityCulling entityCulling in entities)
        {
            entityCulling.OverrideCulling();
            entityCulling.TryOverrideMeshVisibility(false);
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
        SetRenderStateBasedOnVisiblityState(state);


        switch (state) // I do feel this is a bit bad, but eh, fuck it.
        {
            case VisibleState.Unload: // everything else
                SetEntityCulling(false);
                SetAnimatorState(false);
                break;
            case VisibleState.Minimal: // 2nd layer rooms
                SetEntityCulling(false);
                SetAnimatorState(false);
                break;
            case VisibleState.Medium: // 1st layer rooms
                SetEntityCulling(true);
                SetAnimatorState(false);
                break;
            case VisibleState.Maximum: // Current room
                SetEntityCulling(true);
                SetAnimatorState();
                break;
        }
    }

    private void SetRenderStateBasedOnVisiblityState(VisibleState visibleState)
    {
        switch (visibleState) // I do feel this is a bit bad, but eh, fuck it.
        {
            case VisibleState.Unload: // everything else
                SetLowDetailState(false);
                SetMediumState(false);
                SetHighDetailState(false);
                break;
            case VisibleState.Minimal: // 2nd layer rooms
                SetLowDetailState(true); // need to set LOD state.
                SetMediumState(false);
                SetHighDetailState(false);
                break;
            case VisibleState.Medium: // 1st layer rooms
                SetLowDetailState(true); // need to set LOD state.
                SetMediumState(true);
                SetHighDetailState(false); // need two levels of detail. lower and higher
                break;
            case VisibleState.Maximum: // Current room
                SetLowDetailState(true);
                SetMediumState(true);
                SetHighDetailState(true);
                break;
        }
    }

    private void SetEntityCulling(bool isVisible = true)
    {
        StartCoroutine(SetEntityCullingRenderState(isVisible));
    }

    IEnumerator SetEntityCullingRenderState(bool isVisible)
    {
        foreach (EntityCulling entity in entities)
        {
            if (entity == null) continue; // TODO: should have enemies or what ever not be collected.


            entity.TryOverrideMeshVisibility(isVisible);
        }
        yield return null;
    }

    private void SetLowDetailState(bool isVisible = true)
    {
        // print("render state " + isVisible);

        StartCoroutine(SetRenderStateWhenReady(isVisible, lowDetail));
    }

    private void SetHighDetailState(bool isVisible = true)
    {
        // print("render state " + isVisible);

        StartCoroutine(SetRenderStateWhenReady(isVisible, highDetail));
    }

    private void SetMediumState(bool isVisible = true)
    {
        // print("render state " + isVisible);

        StartCoroutine(SetRenderStateWhenReady(isVisible, mediumDetail));
    }



    IEnumerator SetRenderStateWhenReady(bool isVisible, List<MeshRenderer> meshRenderers)
    {

        while (!isReady) yield return null; // wait until meshes are gathered.


        foreach (var meshRenderer in meshRenderers)
        {
            if (meshRenderer == null) continue;
            if (meshRenderer.enabled == isVisible) continue;

            meshRenderer.enabled = isVisible;
        }

    }

    private void SetAnimatorState(bool isEnabled = true)
    {
        // StartCoroutine(SetAnimatorStateWhenReady(isEnabled));
    }

    private IEnumerator SetAnimatorStateWhenReady(bool isEnabled)
    {
        while (!isReady) yield return null;

        foreach (var animator in animators)
        {
            if (animator == null) continue;
            if (animator.enabled == isEnabled) continue;

            animator.enabled = isEnabled;
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

    // bool OutsideLowerBounds(Vector3 playerPos, Vector3 lowerBounds, float maxDistanceFromRoom)
    // {
    //     return playerPos.x < lowerBounds.x - maxDistanceFromRoom || playerPos.y < lowerBounds.y - maxDistanceFromRoom || playerPos.z < lowerBounds.z - maxDistanceFromRoom;
    // }

    // bool OutsideUpperBounds(Vector3 playerPos, Vector3 upperBounds, float maxDistanceFromRoom)
    // {
    //     return playerPos.x > upperBounds.x + maxDistanceFromRoom || playerPos.y > upperBounds.y + maxDistanceFromRoom || playerPos.z > upperBounds.z + maxDistanceFromRoom;
    // }
}
