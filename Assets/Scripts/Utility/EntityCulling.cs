using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCulling : MonoBehaviour
{
    MeshRenderer[] meshRenderers;

    Transform player;

    float maxDistance = 64;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        while (PlayerRefFetcher.Instance == null) yield return null;

        player = PlayerRefFetcher.Instance.GetPlayerRef().transform;

        meshRenderers = GetComponentsInChildren<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        if (Vector3.Distance(player.position, transform.position) > maxDistance)
        {
            foreach (var renderer in meshRenderers)
            {
                renderer.enabled = false;
            }
        }
        else
        {
            foreach (var renderer in meshRenderers)
            {
                renderer.enabled = true;
            }
        }
    }
}
