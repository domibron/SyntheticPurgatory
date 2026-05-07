using UnityEngine;

/// <summary>
/// The base class for all collectable items, use this to create a collectable item.
/// <br />Make sure to override the CollectItem function.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class CollectableBase : MonoBehaviour
{
    // ****************************************
    // *              MOVEMENT                *
    // ****************************************

    /// <summary>
    /// The target's transform, used for checks and moving towards.
    /// </summary>
    protected Transform targetTransform;

    /// <summary>
    /// Attached rigidbody to this object.
    /// </summary>
    protected Rigidbody rb;


    // ****************************************
    // *              BEHAVIOUR               *
    // ****************************************

    /// <summary>
    /// How close the item must be to the target before it's collected.
    /// <br />This is using the players collect item range stats.
    /// </summary>
    protected float collectItemRange = 0f;

    /// <summary>
    /// The max distance for the the target to be out of range of move towards.
    /// <br />This is using the players collect item range stats.
    /// </summary>
    protected float maxCollectionRange = 0f;

    /// <summary>
    /// How fast does this item accelerate towards the target.
    /// <br />This is using the players collect item range stats.
    /// </summary>
    protected float flyAccel = 0f;

    /// <summary>
    /// The max speed the item can move towards the target.
    /// <br />This is using the players collect item range stats.
    /// </summary>
    protected float flyMaxSpeed = 0f;

    /// <summary>
    /// Additional acceleration boost when the item is further away.
    /// <br />This is using the players collect item range stats.
    /// </summary>
    protected float flyDistanceBoost = 0f;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Initialization of this script.
    /// </summary>
    void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Checks for the item.
    /// </summary>
    protected virtual void Update()
    {
        CheckForTarget();
        CollectItem();
    }


    /// <summary>
    /// Physics movement of the item.
    /// </summary>
    protected virtual void FixedUpdate()
    {
        MoveTowardsTarget();
    }

    /// <summary>
    /// Set's up the script's variables.
    /// <br /><b>NOTE:</b><i> Override this to set the variables to different values.</i>
    /// </summary>
    protected virtual void Initialize()
    {
        if (GameManager.Instance != null)
        {
            MiscellaneousStats collectableStats = GameStatsManager.Instance.GetStats<MiscellaneousStats>(Stats.miscellaneous);

            if (collectableStats == null)
            {
                Debug.LogError("Collectable stats are null?!", this); // * this should never hit unless the main menu scene was never loaded.
                collectableStats = new();
            }

            // Set the variables from the stats.
            collectItemRange = collectableStats.CollectItemIntoInventoryRange;
            maxCollectionRange = collectableStats.MaxCollectionRangeStat.GetCurrentValue();
            flyAccel = collectableStats.FlyAccel;
            flyMaxSpeed = collectableStats.FlyMaxSpeed;
            flyDistanceBoost = collectableStats.FlyDistanceBoost;
        }
    }


    /// <summary>
    /// Sets the target transform if this item can see the target. (Default target is the player)
    /// <br /><b>NOTE:</b><i> Override this to change checks but make sure to set targetTransform.</i>
    /// </summary>
    protected virtual void CheckForTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, maxCollectionRange, LayerMask.GetMask(Constants.PlayerLayer));

        if (colliders.Length <= 0)
        {
            targetTransform = null;
        }

        foreach (Collider col in colliders)
        {
            if (!col.gameObject.CompareTag(Constants.PlayerTag)) continue;

            if (Physics.Linecast(transform.position, col.transform.position, ~LayerMask.GetMask(Constants.PlayerLayer))) continue;

            targetTransform = col.transform;

        }

    }

    /// <summary>
    /// Moves the item towards the target.
    /// <br /><b>NOTE:</b><i> Only override if you are changing the move towards behavior.</i>
    /// </summary>
    protected virtual void MoveTowardsTarget()
    {
        if (!CanTargetCollect())
        {
            rb.useGravity = true;
            return;
        }

        rb.useGravity = false;

        Vector3 wishDir = (targetTransform.position - transform.position).normalized;
        float projVel = Vector3.Dot(rb.linearVelocity, wishDir.normalized);
        float accel = flyAccel + Mathf.Pow(flyDistanceBoost, Mathf.Max(Mathf.FloorToInt(maxCollectionRange - Vector3.Distance(transform.position, targetTransform.position)), 1)) * Time.deltaTime;

        if (projVel + accel > flyMaxSpeed)
            accel = Mathf.Max(0, flyMaxSpeed - projVel);

        wishDir = wishDir.normalized * accel;


        rb.AddForce(wishDir, ForceMode.VelocityChange);

        // Add collision bounce back and shit.
    }

    /// <summary>
    /// What to do when collecting the item.
    /// <br /><b>NOTE:</b><i> Please define this function for your derived class. Destroy, duplicate, move, teleport, do what you want.</i>
    /// </summary>
    protected virtual void CollectItem() { }

    /// <summary>
    /// Checks if the target can collect this item.
    /// <br /><b>NOTE:</b><i> You can override this function to run your own collection check logic.</i>
    /// </summary>
    /// <returns>Whether the target are able to collect the item.</returns>
    protected virtual bool CanTargetCollect()
    {
        if (targetTransform == null)
            return false;

        return true;

    }
}
