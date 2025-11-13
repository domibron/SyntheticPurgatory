using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For the boss arena
/// </summary>
public class CraneController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] containerWalls;

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
    }

    public bool PlaceContainerWall(Vector3 targetPoint, out GameObject container)
    {
        container = null;
        if (inJob) return false;

        currentContainer = Instantiate(containerWalls[UnityEngine.Random.Range(0, containerWalls.Length)], containerSpawnPoint.position, Quaternion.identity);
        AddContainer(currentContainer);

        StartCoroutine(GetAndPlaceContainerWall(targetPoint));
        container = currentContainer;
        return true;
    }

    private IEnumerator GetAndPlaceContainerWall(Vector3 targetPoint)
    {
        inJob = true;
        float floatingPointFuckery = 0.1f;
        alarm.Play();

        // lastMoveCheck = 5f;

        // if (!creaking.isPlaying) creaking.time = UnityEngine.Random.Range(0f, creaking.clip.length); creaking.Play();

        // print("spawn container");


        ICraneGrabbable craneGrabbable = currentContainer.GetComponent<ICraneGrabbable>();

        // print("raise boom");

        yield return new WaitForEndOfFrame(); // reset to frame time.
        crane.OverrideBoomDropDist(boomDropAmount: 0);
        yield return new WaitForEndOfFrame();

        while (crane.GetYDistance() > floatingPointFuckery)
        {
            yield return new WaitForEndOfFrame();
        }

        // print("heading to container");

        yield return new WaitForEndOfFrame(); // reset to frame time.
        crane.SetTargetPoint(craneGrabbable.GetGrabPoint());
        yield return new WaitForEndOfFrame();

        while (crane.GetXZDistance() > floatingPointFuckery)
        {
            yield return new WaitForEndOfFrame();
        }


        // print("Lowering boom");

        yield return new WaitForEndOfFrame(); // reset to frame time.
        crane.OverrideBoomDropDist(false);
        yield return new WaitForEndOfFrame();


        while (crane.GetYDistance() > floatingPointFuckery)
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

        while (crane.GetYDistance() > floatingPointFuckery)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame(); // reset to frame time.
        // print("heading to placement point");
        crane.SetTargetPoint(targetPoint + new Vector3(0, craneGrabbable.GetGrabPoint().position.y - craneGrabbable.GetPlacementPoint().position.y, 0));
        yield return new WaitForEndOfFrame();

        while (crane.GetXZDistance() > floatingPointFuckery)
        {
            yield return new WaitForEndOfFrame();
        }

        // print("Lowering boom");

        yield return new WaitForEndOfFrame(); // reset to frame time.
        crane.OverrideBoomDropDist(false);
        yield return new WaitForEndOfFrame();


        while (crane.GetYDistance() > floatingPointFuckery)
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

        while (crane.GetYDistance() > floatingPointFuckery)
        {
            yield return new WaitForEndOfFrame();
        }

        // print("heading to resting point");
        crane.SetTargetPoint(restingPoint);
        yield return new WaitForEndOfFrame();

        // while (crane.GetXZDistance() > floatingPointFuckery)
        // {
        //     yield return new WaitForEndOfFrame();
        // }

        yield return new WaitForEndOfFrame();
        // audioSource.Stop();

        currentContainer = null;

        // print("completed");
        inJob = false;
        OnJobCompleted?.Invoke();

    }

    public bool RemoveContainerWall(GameObject container)
    {
        if (inJob) return false;

        StartCoroutine(GetAndRemoveContainerWall(container));
        return true;
    }

    private IEnumerator GetAndRemoveContainerWall(GameObject container)
    {
        inJob = true;
        float floatingPointFuckery = 0.1f;
        alarm.Play();

        // lastMoveCheck = 5f;

        // if (!creaking.isPlaying) creaking.time = UnityEngine.Random.Range(0f, creaking.clip.length); creaking.Play();

        // print("spawn container");
        currentContainer = container;

        ICraneGrabbable craneGrabbable = currentContainer.GetComponent<ICraneGrabbable>();

        // print("raise boom");

        yield return new WaitForEndOfFrame(); // reset to frame time.
        crane.OverrideBoomDropDist(boomDropAmount: 0);
        yield return new WaitForEndOfFrame();

        while (crane.GetYDistance() > floatingPointFuckery)
        {
            yield return new WaitForEndOfFrame();
        }

        // print("heading to container");

        yield return new WaitForEndOfFrame(); // reset to frame time.
        crane.SetTargetPoint(craneGrabbable.GetGrabPoint());
        yield return new WaitForEndOfFrame();

        while (crane.GetXZDistance() > floatingPointFuckery)
        {
            yield return new WaitForEndOfFrame();
        }


        // print("Lowering boom");

        yield return new WaitForEndOfFrame(); // reset to frame time.
        crane.OverrideBoomDropDist(false);
        yield return new WaitForEndOfFrame();


        while (crane.GetYDistance() > floatingPointFuckery)
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

        while (crane.GetYDistance() > floatingPointFuckery)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame(); // reset to frame time.
        // print("heading to placement point");
        crane.SetTargetPoint(containerSpawnPoint.position + new Vector3(0, craneGrabbable.GetGrabPoint().position.y - craneGrabbable.GetPlacementPoint().position.y, 0));
        yield return new WaitForEndOfFrame();

        while (crane.GetXZDistance() > floatingPointFuckery)
        {
            yield return new WaitForEndOfFrame();
        }

        // print("Lowering boom");

        yield return new WaitForEndOfFrame(); // reset to frame time.
        crane.OverrideBoomDropDist(false);
        yield return new WaitForEndOfFrame();


        while (crane.GetYDistance() > floatingPointFuckery)
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

        while (crane.GetYDistance() > floatingPointFuckery)
        {
            yield return new WaitForEndOfFrame();
        }

        // print("heading to resting point");
        crane.SetTargetPoint(restingPoint);
        yield return new WaitForEndOfFrame();

        RemoveContainer(container);

        // while (crane.GetXZDistance() > floatingPointFuckery)
        // {
        //     yield return new WaitForEndOfFrame();
        // }

        yield return new WaitForEndOfFrame();
        // audioSource.Stop();

        // print("completed");
        inJob = false;
        OnJobCompleted?.Invoke();

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
        GameObject returnedGO = Instantiate(containerWalls[UnityEngine.Random.Range(0, containerWalls.Length)], containerSpawnPoint.position, Quaternion.identity);
        AddContainer(returnedGO);

        return returnedGO;
    }

}
