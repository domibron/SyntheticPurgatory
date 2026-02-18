using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCulling : MonoBehaviour
{
    MeshRenderer[] meshRenderers;

    Transform player;

    float maxDistance = 48;

    bool overrideCulling = false;

    Vector2Int gridCoordinates = Vector2Int.zero;

    LevelGenerator levelGenerator;

    private bool isReady = false;

    [SerializeField]
    private bool alsoDisablePhysics = true;

    private Rigidbody rb;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        while (PlayerRefFetcher.Instance == null) yield return null;

        player = PlayerRefFetcher.Instance.GetPlayerRef().transform;

        meshRenderers = GetComponentsInChildren<MeshRenderer>();

        rb = GetComponent<Rigidbody>();


        if (LevelGenObjectRefGetter.Instance == null)
        {
            Debug.LogWarning("NON CRITICAL: LevelGenObjectRefGetter could not be found!");

        }
        else
        {
            levelGenerator = LevelGenObjectRefGetter.Instance.transform.GetComponent<LevelGenerator>();

            gridCoordinates = levelGenerator.GetGridCoordinates(transform.position);

            isReady = true;

        }

    }

    // Update is called once per frame
    void Update()
    {
        if (levelGenerator == null) return;

        if (player == null) return;

        if (levelGenerator.GetGridCoordinates(transform.position) != gridCoordinates)
        {
            overrideCulling = false; // object is on the move.
        }


        if (overrideCulling) return;

        if (Vector3.Distance(player.position, transform.position) > maxDistance)
        {
            SetMeshVisibility(false);
        }
        else
        {
            SetMeshVisibility(true);
        }
    }

    private void SetMeshVisibility(bool isVisible = true)
    {
        StartCoroutine(TrySetMeshVisibility(isVisible));
    }


    private IEnumerator TrySetMeshVisibility(bool isVisible)
    {
        while (!isReady) yield return null;

        foreach (var renderer in meshRenderers)
        {
            if (renderer.enabled != isVisible)
                renderer.enabled = isVisible;
        }

        if (rb != null && alsoDisablePhysics)
        {
            rb.isKinematic = !isVisible;
        }
    }

    public void OverrideCulling()
    {
        overrideCulling = true;
    }

    public void TryOverrideMeshVisibility(bool isVisible = false)
    {
        if (!overrideCulling) return;

        SetMeshVisibility(isVisible);
    }
}
