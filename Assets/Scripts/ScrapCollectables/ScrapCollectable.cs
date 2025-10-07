using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ScrapCollectable : MonoBehaviour
{
    Transform playerTransform;

    Rigidbody rb;

    [SerializeField]
    int scrapWorth = 1;

    // [SerializeField]
    // float flyAccel = 5f; // TODO static constant thing.

    // [SerializeField]
    // float flyMaxSpeed = 15f;

    // [SerializeField]
    // float flyDistanceBoost = 10f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckForPlayer();
        CollectItem();

    }

    void FixedUpdate()
    {
        FlyTowardsPlayer();
    }

    public void Initialize(int scrapWorth)
    {
        this.scrapWorth = scrapWorth;
    }

    private void CheckForPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, ScrapManager.Instance.MaxCollectionRange, LayerMask.GetMask("Player"));

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

    private void FlyTowardsPlayer()
    {
        if (!CanPlayerCollect())
        {
            rb.useGravity = true;
            return;
        }

        rb.useGravity = false;

        Vector3 wishDir = (playerTransform.position - transform.position).normalized;
        float projVel = Vector3.Dot(rb.linearVelocity, wishDir.normalized);
        float accel = ScrapManager.Instance.flyAccel + Mathf.Pow(ScrapManager.Instance.flyDistanceBoost, Mathf.Max(Mathf.FloorToInt(ScrapManager.Instance.MaxCollectionRange - Vector3.Distance(transform.position, playerTransform.position)), 1)) * Time.deltaTime;

        if (projVel + accel > ScrapManager.Instance.flyMaxSpeed)
            accel = Mathf.Max(0, ScrapManager.Instance.flyMaxSpeed - projVel);

        wishDir = wishDir.normalized * accel;


        rb.AddForce(wishDir, ForceMode.VelocityChange);

        // Add collison bounce back and shit.
    }

    private void CollectItem()
    {
        if (!CanPlayerCollect()) return;

        if (Vector3.Distance(transform.position, playerTransform.position) > ScrapManager.Instance.CollectItemRange) return;

        int remaining = ScrapManager.Instance.CollectScrap(scrapWorth);

        while (remaining > 0) // fingers cross this doesn't fuck up. It will in the future and cause a stutter, you watch.
        {
            ScrapItemData prefabToSpawn = ScrapManager.Instance.GetPrefabWithHighestWorth(remaining);

            Instantiate(prefabToSpawn.ScrapPrefab, transform.position, Quaternion.identity);

            remaining -= prefabToSpawn.ScrapWorth;
        }

        Destroy(gameObject);
    }

    private bool CanPlayerCollect()
    {
        if (playerTransform == null)
            return false;

        if (!ScrapManager.Instance.HaveInventorySpace())
            return false;

        return true;

    }

}
