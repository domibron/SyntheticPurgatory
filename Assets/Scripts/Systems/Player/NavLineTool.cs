using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Navigational line the player can place down to track where they were and where is the spawn.
/// </summary>
public class NavLineTool : MonoBehaviour
{
    /// <summary>
    /// Line renderer to display the line to connect to other nav lines.
    /// </summary>
    [SerializeField]
    LineRenderer lineRenderer;

    /// <summary>
    /// The max amount of connections allowed.
    /// </summary>
    [SerializeField]
    private int maxConnections = 5;

    /// <summary>
    /// The max range of connection.
    /// </summary>
    [SerializeField]
    private float maxRange = 15f;

    /// <summary>
    /// A collection of all nearby nav lines.
    /// </summary>
    List<Transform> allNearByNavLines = new List<Transform>();

    /// <summary>
    /// The attached rigidbody.
    /// </summary>
    private Rigidbody rb;

    /// <summary>
    /// How often to update the lines once stationary.
    /// </summary>
    [SerializeField]
    float updateEvery = 1f;

    /// <summary>
    /// The current time remaining before the next update.
    /// </summary>
    private float currentUpdateTime = 0f;

    /// <summary>
    /// All layers that block the lines from connection.
    /// </summary>
    [SerializeField]
    LayerMask lineBlockers;

    /// <summary>
    /// Can other nav lines connect to us.
    /// </summary>
    private bool isConnectable = false;

    /// <summary>
    /// Are we ready to display lines and connect to others.
    /// </summary>
    private bool isReady = false;

    IEnumerator Start()
    {
        rb = GetComponent<Rigidbody>();
        lineRenderer.enabled = false;

        yield return new WaitForSeconds(1f);
        isReady = true;
    }

    void FixedUpdate()
    {
        if (!isReady) return;

        // Check to see if we are still moving and set the appropriate variables.
        if (rb.linearVelocity.magnitude > 1)
        {
            if (Physics.CheckSphere(transform.position, 1f, LayerMask.GetMask("Ground"), QueryTriggerInteraction.Ignore))
            {
                if (rb.linearVelocity.magnitude < 1)
                    rb.linearVelocity = Vector3.zero;
                else
                    rb.linearVelocity -= rb.linearVelocity * Time.deltaTime;

            }
            isConnectable = false;
            lineRenderer.enabled = false;
            return; // wait until it stops moving.
        }

        // Update time tick.
        if (currentUpdateTime > 0)
        {
            currentUpdateTime -= Time.deltaTime;
            return;
        }

        // We are now connectable.
        isConnectable = true;


        // Find nearby nav lines.
        allNearByNavLines.Clear();

        Collider[] colliders = Physics.OverlapSphere(transform.position, maxRange);

        // TODO: add optimization where newer ones connect to old?
        // TODO: Prioritize the closest ones.

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag(Constants.NavLineTag))
            {
                if (allNearByNavLines.Contains(collider.transform)) continue;

                if (collider.GetComponent<NavLineTool>()?.IsConnectable() == false) continue;

                if (!Physics.Linecast(transform.position, collider.transform.position, lineBlockers))
                    allNearByNavLines.Add(collider.transform);
            }
        }


        if (allNearByNavLines.Count <= 0)
        {
            lineRenderer.enabled = false;
            return; // no other nav lines
        }


        // Display the lines.

        lineRenderer.enabled = true;


        List<Vector3> navPointsToConnect = new List<Vector3>();

        foreach (Transform t in allNearByNavLines)
        {
            navPointsToConnect.Add(transform.position);
            navPointsToConnect.Add(t.position);
        }

        lineRenderer.positionCount = navPointsToConnect.Count;
        lineRenderer.SetPositions(navPointsToConnect.ToArray());

        currentUpdateTime = updateEvery;
    }

    /// <summary>
    /// Check to see if this nav line is connectable.
    /// </summary>
    /// <returns>True if you can connect.</returns>
    public bool IsConnectable()
    {
        return isConnectable;
    }
}
