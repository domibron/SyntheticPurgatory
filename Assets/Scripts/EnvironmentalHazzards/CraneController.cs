using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// For the boss arena
/// </summary>
public class CraneController : MonoBehaviour
{
    [SerializeField]
    private GameObject containerWalls;

    [SerializeField]
    private GameObject physicsContainer;

    private List<GameObject> allOurSpawnedContainers = new List<GameObject>();

    [SerializeField]
    private Transform containerSpawnPoint;

    private GameObject currentContainer;

    [SerializeField]
    private Crane crane;

    public event Action OnJobCompleted;

    [SerializeField]
    private Transform restingPoint;

    private bool inJob = false;

    [SerializeField]
    private AudioSource alarm;

    [SerializeField]
    private AudioSource creaking;

    [SerializeField]
    private LightFlash craneLightFlash;

    const float FLOATING_POINT_FUCKERY = 0.1f;

    private float lastJob = 0f;

    [SerializeField]
    private BoxCollider dropZone;

    [SerializeField]
    private ContainerPlacementCheck containerPlacementCheck;

    // private GameObject currentContainer;

    // private float lastMoveCheck = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // StartCoroutine(GetAndPlaceContainerWall());
    }

    // Update is called once per frame
    void Update()
    {
        // if (lastMoveCheck <= 0 && creaking.isPlaying)
        // {
        //     creaking.Pause();
        // }
        // else if (lastMoveCheck > 0) lastMoveCheck -= Time.deltaTime;

        // if (crane.GetDistanceFromTargetWithOffsets() > 0) lastMoveCheck = 5f;

        // StartDropContainerJob();

        if (!inJob && lastJob > 0) lastJob -= Time.deltaTime;
        if (!inJob && lastJob <= 0) ResetCrane();
        else if (inJob) lastJob = 1f;
    }

    public bool IsStillInJob()
    {
        return inJob;
    }

    private void JobStart()
    {
        inJob = true;
        alarm.Play();
        craneLightFlash.StartFlashing();
    }

    private void JobEnd()
    {
        currentContainer = null;

        craneLightFlash.StopFlashing();


        // print("completed");
        inJob = false;
        OnJobCompleted?.Invoke();
    }


    public bool ResetCrane()
    {
        if (inJob) return false;
        StartCoroutine(ResetCraneJob());
        return true;
    }


    private IEnumerator ResetCraneJob()
    {
        inJob = true;

        yield return new WaitForEndOfFrame();
        crane.SetTargetPoint(restingPoint);
        yield return new WaitForEndOfFrame();

        while (crane.GetXZDistance() > FLOATING_POINT_FUCKERY)
        {
            yield return new WaitForEndOfFrame();
        }

        OnJobCompleted?.Invoke();
        inJob = false;
    }

    public bool PlaceContainerWall(Vector3 targetPoint, out GameObject container)
    {
        container = null;
        if (inJob) return false;

        currentContainer = Instantiate(containerWalls, containerSpawnPoint.position, Quaternion.identity);
        AddContainer(currentContainer);

        StartCoroutine(GetAndPlaceContainerWall(targetPoint));
        container = currentContainer;
        return true;
    }

    public bool StartDropContainerJob()
    {
        if (inJob) return false;

        StartCoroutine(DropContainer());
        return true;
    }

    private IEnumerator DropContainer()
    {
        JobStart();

        Vector3 backRightCorner = dropZone.transform.position + (dropZone.size / 2f);
        Vector3 frontLeftCorner = dropZone.transform.position - (dropZone.size / 2f);
        Vector3 randomPoint = new Vector3(Mathf.Lerp(frontLeftCorner.x, backRightCorner.x, UnityEngine.Random.Range(0f, 1f)), 0, Mathf.Lerp(frontLeftCorner.z, backRightCorner.z, UnityEngine.Random.Range(0f, 1f)));

        Vector3 targetPoint = Vector3.zero;

        while (true)
        {
            bool res = NavMesh.SamplePosition(randomPoint, out NavMeshHit sampleHit, 1f, NavMesh.AllAreas);

            if (res && containerPlacementCheck.SampleContainerPosition(sampleHit.position))
            {
                targetPoint = sampleHit.position;
                break;
            }

            randomPoint = new Vector3(Mathf.Lerp(frontLeftCorner.x, backRightCorner.x, UnityEngine.Random.Range(0f, 1f)), 0.5f, Mathf.Lerp(frontLeftCorner.z, backRightCorner.z, UnityEngine.Random.Range(0f, 1f)));
            yield return new WaitForEndOfFrame();
        }


        // if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 10f, NavMesh.GetAreaFromName("Container")))
        // {
        //     targetPoint = hit.position;
        // }
        // else
        // {
        //     Debug.LogError("Cannot drop container, nav mesh check failed.");
        //     JobEnd();
        //     yield break;
        // }

        currentContainer = Instantiate(physicsContainer, containerSpawnPoint.position, Quaternion.identity);


        ICraneGrabbable craneGrabbable = currentContainer.GetComponent<ICraneGrabbable>();

        yield return new WaitForEndOfFrame(); // reset to frame time.
        crane.OverrideBoomDropDist(boomDropAmount: 0);
        yield return new WaitForEndOfFrame();

        while (crane.GetYDistance() > FLOATING_POINT_FUCKERY)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame(); // reset to frame time.
        crane.SetTargetPoint(craneGrabbable.GetGrabPoint());
        yield return new WaitForEndOfFrame();

        while (crane.GetXZDistance() > FLOATING_POINT_FUCKERY)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame(); // reset to frame time.
        crane.OverrideBoomDropDist(false);
        yield return new WaitForEndOfFrame();

        while (crane.GetYDistance() > FLOATING_POINT_FUCKERY)
        {
            yield return new WaitForEndOfFrame();
        }


        // print("pickup container");
        while (!craneGrabbable.GetIsOnHook())
        {
            yield return new WaitForEndOfFrame();
            craneGrabbable.PickUpObject(crane.GetHookTransform());
            // print("pickup container again");
        }
        yield return new WaitForSeconds(1f);

        // print("raise boom");
        yield return new WaitForEndOfFrame(); // reset to frame time.
        crane.OverrideBoomDropDist(boomDropAmount: 0); // then override in a frame
        yield return new WaitForEndOfFrame(); // then we can wait another frame.

        while (crane.GetYDistance() > FLOATING_POINT_FUCKERY)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame(); // reset to frame time.
        // print("heading to placement point");
        crane.SetTargetPoint(targetPoint + new Vector3(0, craneGrabbable.GetGrabPoint().position.y - craneGrabbable.GetPlacementPoint().position.y, 0));
        yield return new WaitForEndOfFrame();

        while (crane.GetXZDistance() > FLOATING_POINT_FUCKERY)
        {
            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < 4; i++)
        {
            alarm.Play();
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForEndOfFrame();
        craneGrabbable.DropObject();
        yield return new WaitForEndOfFrame();
        containerPlacementCheck.ResetPosition();

        JobEnd();
    }

    private IEnumerator GetAndPlaceContainerWall(Vector3 targetPoint)
    {
        JobStart();

        // lastMoveCheck = 5f;

        // if (!creaking.isPlaying) creaking.time = UnityEngine.Random.Range(0f, creaking.clip.length); creaking.Play();

        // print("spawn container");


        ICraneGrabbable craneGrabbable = currentContainer.GetComponent<ICraneGrabbable>();

        // print("raise boom");

        yield return new WaitForEndOfFrame(); // reset to frame time.
        crane.OverrideBoomDropDist(boomDropAmount: 0);
        yield return new WaitForEndOfFrame();

        while (crane.GetYDistance() > FLOATING_POINT_FUCKERY)
        {
            yield return new WaitForEndOfFrame();
        }

        // print("heading to container");

        yield return new WaitForEndOfFrame(); // reset to frame time.
        crane.SetTargetPoint(craneGrabbable.GetGrabPoint());
        yield return new WaitForEndOfFrame();

        while (crane.GetXZDistance() > FLOATING_POINT_FUCKERY)
        {
            yield return new WaitForEndOfFrame();
        }


        // print("Lowering boom");

        yield return new WaitForEndOfFrame(); // reset to frame time.
        crane.OverrideBoomDropDist(false);
        yield return new WaitForEndOfFrame();


        while (crane.GetYDistance() > FLOATING_POINT_FUCKERY)
        {
            yield return new WaitForEndOfFrame();
        }

        // print("pickup container");
        while (!craneGrabbable.GetIsOnHook())
        {
            yield return new WaitForEndOfFrame();
            craneGrabbable.PickUpObject(crane.GetHookTransform());
            // print("pickup container again");
        }
        yield return new WaitForSeconds(1f);

        // print("raise boom");
        yield return new WaitForEndOfFrame(); // reset to frame time.
        crane.OverrideBoomDropDist(boomDropAmount: 0); // then override in a frame
        yield return new WaitForEndOfFrame(); // then we can wait another frame.

        while (crane.GetYDistance() > FLOATING_POINT_FUCKERY)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame(); // reset to frame time.
        // print("heading to placement point");
        crane.SetTargetPoint(targetPoint + new Vector3(0, craneGrabbable.GetGrabPoint().position.y - craneGrabbable.GetPlacementPoint().position.y, 0));
        yield return new WaitForEndOfFrame();

        while (crane.GetXZDistance() > FLOATING_POINT_FUCKERY)
        {
            yield return new WaitForEndOfFrame();
        }

        // print("Lowering boom");

        yield return new WaitForEndOfFrame(); // reset to frame time.
        crane.OverrideBoomDropDist(false);
        yield return new WaitForEndOfFrame();


        while (crane.GetYDistance() > FLOATING_POINT_FUCKERY)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame(); // reset to frame time.
        // print("releasing container");
        craneGrabbable.DropObject();
        yield return new WaitForEndOfFrame();

        // print("raise boom");

        yield return new WaitForEndOfFrame(); // reset to frame time.
        crane.OverrideBoomDropDist(boomDropAmount: 0);
        yield return new WaitForEndOfFrame();

        while (crane.GetYDistance() > FLOATING_POINT_FUCKERY)
        {
            yield return new WaitForEndOfFrame();
        }

        // print("heading to resting point");
        // crane.SetTargetPoint(restingPoint);
        // yield return new WaitForEndOfFrame();

        // while (crane.GetXZDistance() > floatingPointFuckery)
        // {
        //     yield return new WaitForEndOfFrame();
        // }

        yield return new WaitForEndOfFrame();
        // audioSource.Stop();

        JobEnd();

    }

    public bool RemoveContainerWall(GameObject container)
    {
        if (inJob) return false;

        StartCoroutine(GetAndRemoveContainerWall(container));
        return true;
    }

    private IEnumerator GetAndRemoveContainerWall(GameObject container)
    {
        JobStart();

        // lastMoveCheck = 5f;

        // if (!creaking.isPlaying) creaking.time = UnityEngine.Random.Range(0f, creaking.clip.length); creaking.Play();

        // print("spawn container");
        currentContainer = container;

        ICraneGrabbable craneGrabbable = currentContainer.GetComponent<ICraneGrabbable>();

        // print("raise boom");

        yield return new WaitForEndOfFrame(); // reset to frame time.
        crane.OverrideBoomDropDist(boomDropAmount: 0);
        yield return new WaitForEndOfFrame();

        while (crane.GetYDistance() > FLOATING_POINT_FUCKERY)
        {
            yield return new WaitForEndOfFrame();
        }

        // print("heading to container");

        yield return new WaitForEndOfFrame(); // reset to frame time.
        crane.SetTargetPoint(craneGrabbable.GetGrabPoint());
        yield return new WaitForEndOfFrame();

        while (crane.GetXZDistance() > FLOATING_POINT_FUCKERY)
        {
            yield return new WaitForEndOfFrame();
        }


        // print("Lowering boom");

        yield return new WaitForEndOfFrame(); // reset to frame time.
        crane.OverrideBoomDropDist(false);
        yield return new WaitForEndOfFrame();


        while (crane.GetYDistance() > FLOATING_POINT_FUCKERY)
        {
            yield return new WaitForEndOfFrame();
        }

        // print("pickup container");
        while (!craneGrabbable.GetIsOnHook())
        {
            yield return new WaitForEndOfFrame();
            craneGrabbable.PickUpObject(crane.GetHookTransform());
            // print("pickup container again");
        }
        yield return new WaitForSeconds(1f);

        // print("raise boom");
        yield return new WaitForEndOfFrame(); // reset to frame time.
        crane.OverrideBoomDropDist(boomDropAmount: 0); // then override in a frame
        yield return new WaitForEndOfFrame(); // then we can wait another frame.

        while (crane.GetYDistance() > FLOATING_POINT_FUCKERY)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame(); // reset to frame time.
        // print("heading to placement point");
        crane.SetTargetPoint(containerSpawnPoint.position + new Vector3(0, craneGrabbable.GetGrabPoint().position.y - craneGrabbable.GetPlacementPoint().position.y, 0));
        yield return new WaitForEndOfFrame();

        while (crane.GetXZDistance() > FLOATING_POINT_FUCKERY)
        {
            yield return new WaitForEndOfFrame();
        }

        // print("Lowering boom");

        yield return new WaitForEndOfFrame(); // reset to frame time.
        crane.OverrideBoomDropDist(false);
        yield return new WaitForEndOfFrame();


        while (crane.GetYDistance() > FLOATING_POINT_FUCKERY)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame(); // reset to frame time.
        // print("releasing container");
        craneGrabbable.DropObject();
        yield return new WaitForEndOfFrame();

        // print("raise boom");

        yield return new WaitForEndOfFrame(); // reset to frame time.
        crane.OverrideBoomDropDist(boomDropAmount: 0);
        yield return new WaitForEndOfFrame();

        while (crane.GetYDistance() > FLOATING_POINT_FUCKERY)
        {
            yield return new WaitForEndOfFrame();
        }

        // print("heading to resting point");
        // crane.SetTargetPoint(restingPoint);
        // yield return new WaitForEndOfFrame();

        RemoveContainer(container);

        // while (crane.GetXZDistance() > floatingPointFuckery)
        // {
        //     yield return new WaitForEndOfFrame();
        // }

        yield return new WaitForEndOfFrame();
        // audioSource.Stop();

        // print("completed");
        JobEnd();

    }

    private void AddContainer(GameObject container)
    {
        allOurSpawnedContainers.Add(container);
    }

    private void RemoveContainer(GameObject container)
    {
        allOurSpawnedContainers.Remove(container);
        Destroy(container);
    }

    public GameObject SpawnInContainerWall()
    {
        GameObject returnedGO = Instantiate(containerWalls, containerSpawnPoint.position, Quaternion.identity);
        AddContainer(returnedGO);

        return returnedGO;
    }

}
