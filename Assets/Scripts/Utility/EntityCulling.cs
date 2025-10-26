using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCulling : MonoBehaviour
{
    MeshRenderer[] meshRenderers;

    Transform player;

    float maxDistance = 64;

    bool overrideCulling = false;

    Vector2Int gridCoordiantes = Vector2Int.zero;

    LevelGenerator levelGenerator;

    private bool isReady = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        while (PlayerRefFetcher.Instance == null) yield return null;

        player = PlayerRefFetcher.Instance.GetPlayerRef().transform;

        meshRenderers = GetComponentsInChildren<MeshRenderer>();

        levelGenerator = LevelGenObjectRefGetter.Instance.GetReference().GetComponent<LevelGenerator>();

        gridCoordiantes = levelGenerator.GetGridCoordinates(transform.position);

        isReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        if (levelGenerator.GetGridCoordinates(transform.position) != gridCoordiantes)
        {
            overrideCulling = false; // object is on the move.
        }


        if (overrideCulling) return;

        if (Vector3.Distance(player.position, transform.position) > maxDistance)
        {
            SetMeshVisiblity(false);
        }
        else
        {
            SetMeshVisiblity(true);
        }
    }

    private void SetMeshVisiblity(bool isVisible = true)
    {
        StartCoroutine(TrySetMeshVisiblity(isVisible));
    }


    private IEnumerator TrySetMeshVisiblity(bool isVisible)
    {
        while (!isReady) yield return null;

        foreach (var renderer in meshRenderers)
        {
            if (renderer.enabled != isVisible)
                renderer.enabled = isVisible;
        }
    }

    public void OverrideCulling()
    {
        overrideCulling = true;
    }

    public void TryOverrideMeshVisiblity(bool isVisible = false)
    {
        if (!overrideCulling) return;

        SetMeshVisiblity(isVisible);
    }
}
