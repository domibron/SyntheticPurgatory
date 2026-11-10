using System;
using System.Collections;
using UnityEngine;

public class EntityCulling : MonoBehaviour
{
    MeshRenderer[] m_meshRenderers;

    Transform m_player;

    float m_maxDistance = 48;

    bool m_overrideCulling = false;

    Vector2Int m_gridCoordinates = Vector2Int.zero;

    LevelGenerator m_levelGenerator;

    private bool m_isReady = false;

    [SerializeField]
    private bool m_alsoDisablePhysics = true;

    private Rigidbody m_rb;

    private Animator m_animator;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_meshRenderers = GetComponentsInChildren<MeshRenderer>();

        m_rb = GetComponent<Rigidbody>();

        m_animator = GetComponent<Animator>();


        if (LevelGenObjectRefGetter.Instance == null)
        {
            Debug.LogWarning($"NON CRITICAL: {nameof(LevelGenObjectRefGetter)} could not be found!");

        }
        else
        {
            m_levelGenerator = LevelGenObjectRefGetter.Instance.transform.GetComponent<LevelGenerator>();

            m_gridCoordinates = m_levelGenerator.GetGridCoordinates(transform.position);

            m_isReady = true;
        }

        if (PlayerRefFetcher.Instance != null)
        {
            m_player = PlayerRefFetcher.Instance.transform;
        }
        else
        {
            return; // break the damn function.
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (m_levelGenerator == null) return;

        if (m_player == null) return;

        if (m_levelGenerator.GetGridCoordinates(transform.position) != m_gridCoordinates)
        {
            m_overrideCulling = false; // object is on the move.
        }


        if (m_overrideCulling) return;

        if (Vector3.Distance(m_player.position, transform.position) > m_maxDistance)
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
        while (!m_isReady) yield return null;

        foreach (var renderer in m_meshRenderers)
        {
            if (renderer.enabled != isVisible)
                renderer.enabled = isVisible;
        }

        if (m_rb != null && m_alsoDisablePhysics)
        {
            m_rb.isKinematic = !isVisible;
        }

        if (m_animator != null)
        {
            m_animator.enabled = isVisible;
        }
    }

    public void OverrideCulling(bool isOverridden = true)
    {
        m_overrideCulling = isOverridden;
    }

    public void TryOverrideMeshVisibility(bool isVisible = false)
    {
        if (!m_overrideCulling) return;

        SetMeshVisibility(isVisible);
    }

    public void ForceInitializeEntityCulling(Transform playerObject, LevelGenerator levelGenerator = null, bool isOverridden = true)
    {
        if (playerObject == null)
        {
            if (PlayerRefFetcher.Instance != null)
            {
                m_player = PlayerRefFetcher.Instance.transform;
            }
            else
            {
                throw new NullReferenceException("Cannot get the player!");
            }
        }

        if (levelGenerator != null)
        {
            m_levelGenerator = levelGenerator;

            m_gridCoordinates = m_levelGenerator.GetGridCoordinates(transform.position);

            Debug.Log("Fetched coordinates!");
        }
        else if (m_levelGenerator == null && levelGenerator == null)
        {
            m_levelGenerator = LevelGenObjectRefGetter.Instance.transform.GetComponent<LevelGenerator>();

            m_gridCoordinates = m_levelGenerator.GetGridCoordinates(transform.position);

            Debug.Log($"{nameof(m_levelGenerator)} was null but another fetch was made!");
        }

        if (!m_levelGenerator)
        {
            throw new NullReferenceException("Cannot get the level generator!");
        }

        OverrideCulling(isOverridden);

        m_isReady = true;
    }
}
