using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Crane controller for the boss arena.
/// </summary>
public class CraneController : MonoBehaviour
{
    /// <summary>
    /// The container for walls. Used to spawn in duplicates.
    /// </summary>
    [SerializeField]
    private GameObject containerWalls;

    /// <summary>
    /// The container that will dorp and fall breaking the floor tiles. Used to spawn in duplicates.
    /// </summary>
    [SerializeField]
    private GameObject physicsContainer;

    // TODO: see if mem gets filled when phys containers are spawned and see if they are added to list.
    /// <summary>
    /// List of all spawned in containers this crane controller spawned.
    /// </summary>
    private List<GameObject> allOurSpawnedContainers = new List<GameObject>();

    /// <summary>
    /// The spawn point for the duplicated containers.
    /// </summary>
    [SerializeField]
    private Transform containerSpawnPoint;

    /// <summary>
    /// The current container being moved.
    /// </summary>
    private GameObject currentContainer;

    /// <summary>
    /// The crane responsible for moving the containers.
    /// </summary>
    [SerializeField]
    private Crane crane;

    /// <summary>
    /// Event for other scripts to listen to know when this job was concluded.
    /// </summary>
    public event Action OnJobCompleted;

    /// <summary>
    /// The resting point for the crane.
    /// </summary>
    [SerializeField]
    private Transform restingPoint;

    /// <summary>
    /// Used to check if there is a job being completed.
    /// </summary>
    private bool inJob = false;

    /// <summary>
    /// Alarm sfx for when the crane is doing stuff.
    /// </summary>
    [SerializeField]
    private AudioSource alarm;

    /// <summary>
    /// Crane movement sfx for when the crane is moving.
    /// </summary>
    [SerializeField]
    private AudioSource creaking;

    /// <summary>
    /// Crane light to indicate to the player that the crane is doing something. AKA Danger, you will die to it. Because it's an enemy.
    /// </summary>
    [SerializeField]
    private LightFlash craneLightFlash;

    /// <summary>
    /// I hate floats sometimes. Used for leniency since floats can be imprecise at small values.
    /// </summary>
    const float FLOATING_POINT_EPSILON = 0.1f;

    /// <summary>
    /// Time out for when the crane is not in a job to move it back to the reset position.
    /// </summary>
    private float lastJob = 0f;

    /// <summary>
    /// The dorp area for the crane to drop container in.
    /// </summary>
    [SerializeField]
    private BoxCollider dropZone;

    /// <summary>
    /// Container placement checker to check if its a valid placement in the area with no overlap with "static" objects.
    /// </summary>
    [SerializeField]
    private ContainerPlacementCheck containerPlacementCheck;


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

    /// <summary>
    /// Is the crane controller currently performing a job.
    /// </summary>
    /// <returns>True if the crane is currently in a job.</returns>
    public bool IsStillInJob()
    {
        return inJob;
    }

    /// <summary>
    /// Enables and sets variables to mark this crane as in a job. Does the lights and sfx.
    /// </summary>
    private void JobStart()
    {
        inJob = true;
        alarm.Play();
        craneLightFlash.StartFlashing();
    }

    /// <summary>
    /// Disables the job and resets the variables, lights and sfx. 
    /// </summary>
    private void JobEnd()
    {
        currentContainer = null;

        craneLightFlash.StopFlashing();


        // print("completed");
        inJob = false;
        OnJobCompleted?.Invoke();
    }

    /// <summary>
    /// Tries to get the crane to reset.
    /// </summary>
    /// <returns>True if the job was given successfully.</returns>
    public bool ResetCrane()
    {
        if (inJob) return false;
        StartCoroutine(ResetCraneJob());
        return true;
    }

    /// <summary>
    /// Coroutine to move the crane to the resting position.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ResetCraneJob()
    {
        inJob = true;

        yield return new WaitForEndOfFrame();
        crane.SetTargetPoint(restingPoint);
        yield return new WaitForEndOfFrame();

        while (crane.GetXZDistance() > FLOATING_POINT_EPSILON)
        {
            yield return new WaitForEndOfFrame();
        }

        OnJobCompleted?.Invoke();
        inJob = false;
    }

    /// <summary>
    /// Tries to place a container wall at the target position.
    /// </summary>
    /// <param name="targetPoint">The target position to place the container.</param>
    /// <param name="container">The container to move to the target position.</param>
    /// <returns>True if the job was successfully assigned.</returns>
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

    /// <summary>
    /// Trie to start the drop container attack.
    /// </summary>
    /// <returns>True if the job was assigned successfully.</returns>
    public bool StartDropContainerJob()
    {
        if (inJob) return false;

        StartCoroutine(DropContainer());
        return true;
    }

    /// <summary>
    /// Coroutine for making the crane dropping the physics container.
    /// </summary>
    /// <returns></returns>
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

        while (crane.GetYDistance() > FLOATING_POINT_EPSILON)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame(); // reset to frame time.
        crane.SetTargetPoint(craneGrabbable.GetGrabPoint());
        yield return new WaitForEndOfFrame();

        while (crane.GetXZDistance() > FLOATING_POINT_EPSILON)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame(); // reset to frame time.
        crane.OverrideBoomDropDist(false);
        yield return new WaitForEndOfFrame();

        while (crane.GetYDistance() > FLOATING_POINT_EPSILON)
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

        while (crane.GetYDistance() > FLOATING_POINT_EPSILON)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame(); // reset to frame time.
        // print("heading to placement point");
        crane.SetTargetPoint(targetPoint + new Vector3(0, craneGrabbable.GetGrabPoint().position.y - craneGrabbable.GetPlacementPoint().position.y, 0));
        yield return new WaitForEndOfFrame();

        while (crane.GetXZDistance() > FLOATING_POINT_EPSILON)
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

    /// <summary>
    /// Coroutine that spawns in a container wall and places it down using the crane.
    /// </summary>
    /// <param name="targetPoint">The target position to place the container at.</param>
    /// <returns></returns>
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

        while (crane.GetYDistance() > FLOATING_POINT_EPSILON)
        {
            yield return new WaitForEndOfFrame();
        }

        // print("heading to container");

        yield return new WaitForEndOfFrame(); // reset to frame time.
        crane.SetTargetPoint(craneGrabbable.GetGrabPoint());
        yield return new WaitForEndOfFrame();

        while (crane.GetXZDistance() > FLOATING_POINT_EPSILON)
        {
            yield return new WaitForEndOfFrame();
        }


        // print("Lowering boom");

        yield return new WaitForEndOfFrame(); // reset to frame time.
        crane.OverrideBoomDropDist(false);
        yield return new WaitForEndOfFrame();


        while (crane.GetYDistance() > FLOATING_POINT_EPSILON)
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

        while (crane.GetYDistance() > FLOATING_POINT_EPSILON)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame(); // reset to frame time.
        // print("heading to placement point");
        crane.SetTargetPoint(targetPoint + new Vector3(0, craneGrabbable.GetGrabPoint().position.y - craneGrabbable.GetPlacementPoint().position.y, 0));
        yield return new WaitForEndOfFrame();

        while (crane.GetXZDistance() > FLOATING_POINT_EPSILON)
        {
            yield return new WaitForEndOfFrame();
        }

        // print("Lowering boom");

        yield return new WaitForEndOfFrame(); // reset to frame time.
        crane.OverrideBoomDropDist(false);
        yield return new WaitForEndOfFrame();


        while (crane.GetYDistance() > FLOATING_POINT_EPSILON)
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

        while (crane.GetYDistance() > FLOATING_POINT_EPSILON)
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


    /// <summary>
    /// Try to get the crane to remove a container.
    /// </summary>
    /// <param name="container">The container to pickup and remove.</param>
    /// <returns>True if the job was assigned successfully.</returns>
    public bool RemoveContainerWall(GameObject container)
    {
        if (inJob) return false;

        StartCoroutine(GetAndRemoveContainerWall(container));
        return true;
    }

    /// <summary>
    /// Coroutine that uses the crane pickup the target container and remove it. 
    /// </summary>
    /// <param name="container">The target container to pickup and remove.</param>
    /// <returns></returns>
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

        while (crane.GetYDistance() > FLOATING_POINT_EPSILON)
        {
            yield return new WaitForEndOfFrame();
        }

        // print("heading to container");

        yield return new WaitForEndOfFrame(); // reset to frame time.
        crane.SetTargetPoint(craneGrabbable.GetGrabPoint());
        yield return new WaitForEndOfFrame();

        while (crane.GetXZDistance() > FLOATING_POINT_EPSILON)
        {
            yield return new WaitForEndOfFrame();
        }


        // print("Lowering boom");

        yield return new WaitForEndOfFrame(); // reset to frame time.
        crane.OverrideBoomDropDist(false);
        yield return new WaitForEndOfFrame();


        while (crane.GetYDistance() > FLOATING_POINT_EPSILON)
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

        while (crane.GetYDistance() > FLOATING_POINT_EPSILON)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame(); // reset to frame time.
        // print("heading to placement point");
        crane.SetTargetPoint(containerSpawnPoint.position + new Vector3(0, craneGrabbable.GetGrabPoint().position.y - craneGrabbable.GetPlacementPoint().position.y, 0));
        yield return new WaitForEndOfFrame();

        while (crane.GetXZDistance() > FLOATING_POINT_EPSILON)
        {
            yield return new WaitForEndOfFrame();
        }

        // print("Lowering boom");

        yield return new WaitForEndOfFrame(); // reset to frame time.
        crane.OverrideBoomDropDist(false);
        yield return new WaitForEndOfFrame();


        while (crane.GetYDistance() > FLOATING_POINT_EPSILON)
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

        while (crane.GetYDistance() > FLOATING_POINT_EPSILON)
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

    /// <summary>
    /// Add a container to the list for tracking.
    /// </summary>
    /// <param name="container">The container to add.</param>
    private void AddContainer(GameObject container)
    {
        allOurSpawnedContainers.Add(container);
    }

    /// <summary>
    /// Removes the container from the tracking list.
    /// </summary>
    /// <param name="container">The container to remove.</param>
    private void RemoveContainer(GameObject container)
    {
        allOurSpawnedContainers.Remove(container);
        Destroy(container);
    }

    /// <summary>
    /// Spawns in a container and returns a game object reference to it. (Also adds it to the tracking list)
    /// </summary>
    /// <returns>The container game object that was spawned.</returns>
    public GameObject SpawnInContainerWall()
    {
        GameObject returnedGO = Instantiate(containerWalls, containerSpawnPoint.position, Quaternion.identity);
        AddContainer(returnedGO);

        return returnedGO;
    }

}
