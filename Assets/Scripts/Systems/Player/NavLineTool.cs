using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavLineTool : MonoBehaviour
{
    [SerializeField]
    LineRenderer lineRenderer;

    [SerializeField]
    private int maxConnections = 5;

    [SerializeField]
    private float maxRange = 15f;

    List<Transform> allNearByNavLines = new List<Transform>();

    private Rigidbody rb;

    [SerializeField]
    float updateEvery = 1f;

    private float currentUpdateTime = 0f;

    [SerializeField]
    LayerMask lineBlockers;

    private bool canConnectTo = false;

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

        if (rb.linearVelocity.magnitude > 1)
        {
            if (Physics.CheckSphere(transform.position, 1f, LayerMask.GetMask("Ground"), QueryTriggerInteraction.Ignore))
            {
                if (rb.linearVelocity.magnitude < 1)
                    rb.linearVelocity = Vector3.zero;
                else
                    rb.linearVelocity -= rb.linearVelocity * Time.deltaTime;

            }
            canConnectTo = false;
            lineRenderer.enabled = false;
            return; // wait until it stops moving.
        }

        if (currentUpdateTime > 0)
        {
            currentUpdateTime -= Time.deltaTime;
            return;
        }

        canConnectTo = true;

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

        // print(allNearByNavLines.Count);

        if (allNearByNavLines.Count <= 0)
        {
            lineRenderer.enabled = false;
            return; // no other nav lines
        }


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

    public bool IsConnectable()
    {
        return canConnectTo;
    }
}
