using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CollectableBase : MonoBehaviour
{

    protected Transform playerTransform;

    protected Rigidbody rb;

    protected float collectItemRange = 0f;
    protected float maxCollectionRange = 0f;
    protected float flyAccel = 0f;
    protected float flyMaxSpeed = 0f;
    protected float flyDistanceBoost = 0f;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (GameManager.Instance != null)
        {
            MiscellaneousStats collectableStats = GameStatsManager.Instance.GetStats<MiscellaneousStats>(Stats.miscellaneous);

            if (collectableStats == null)
            {
                Debug.LogError("Collectable stats are null?!", this);
                // maxInventoryScrap = new CollectableStats().MaxInventoryScrap;
                collectableStats = new();
            }

            collectItemRange = collectableStats.CollectItemRange;
            maxCollectionRange = collectableStats.MaxCollectionRange;
            flyAccel = collectableStats.FlyAccel;
            flyMaxSpeed = collectableStats.FlyMaxSpeed;
            flyDistanceBoost = collectableStats.FlyDistanceBoost;
        }
    }

    protected virtual void Update()
    {
        CheckForPlayer();
        CollectItem();

    }

    protected virtual void FixedUpdate()
    {
        FlyTowardsPlayer();
    }

    protected virtual void CheckForPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, maxCollectionRange, LayerMask.GetMask("Player"));

        if (colliders.Length <= 0)
        {
            playerTransform = null;
        }

        foreach (Collider col in colliders)
        {
            if (!col.gameObject.CompareTag(Constants.PlayerTag)) continue;

            if (Physics.Linecast(transform.position, col.transform.position, ~LayerMask.GetMask("Player"))) continue;

            playerTransform = col.transform;

        }

    }

    protected virtual void FlyTowardsPlayer()
    {
        if (!CanPlayerCollect())
        {
            rb.useGravity = true;
            return;
        }

        rb.useGravity = false;

        Vector3 wishDir = (playerTransform.position - transform.position).normalized;
        float projVel = Vector3.Dot(rb.linearVelocity, wishDir.normalized);
        float accel = flyAccel + Mathf.Pow(flyDistanceBoost, Mathf.Max(Mathf.FloorToInt(maxCollectionRange - Vector3.Distance(transform.position, playerTransform.position)), 1)) * Time.deltaTime;

        if (projVel + accel > flyMaxSpeed)
            accel = Mathf.Max(0, flyMaxSpeed - projVel);

        wishDir = wishDir.normalized * accel;


        rb.AddForce(wishDir, ForceMode.VelocityChange);

        // Add collison bounce back and shit.
    }

    protected virtual void CollectItem() { }

    protected virtual bool CanPlayerCollect()
    {
        if (playerTransform == null)
            return false;

        if (!ScrapManager.Instance.HaveInventorySpace())
            return false;

        return true;

    }
}
