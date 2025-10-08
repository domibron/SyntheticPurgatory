using UnityEngine;

public class ScrapDeposit : MonoBehaviour
{

    [SerializeField]
    Transform depoCollectionPoint;

    [SerializeField]
    float itemForce = 15f;

    private bool playerInRange = false;

    private float currentDelay = 0f;

    private Transform playerTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (currentDelay > 0f) currentDelay -= Time.deltaTime;


        if (playerTransform == null) return;

        if (playerInRange && currentDelay <= 0)
        {
            // * DEPO
            if (ScrapManager.Instance.GetScrapInInventory() <= 0) return;


            ScrapItemData scrapItemData = ScrapManager.GetPrefabWithHighestWorth(ScrapManager.Instance.GetScrapInInventory(), ScrapManager.Instance.DepositScrapWithWorth);

            GameObject scrapObject = Instantiate(scrapItemData.ScrapPrefab, playerTransform.position, Quaternion.identity);
            ScrapManager.Instance.DepositScrap(scrapItemData.ScrapWorth);

            // we need our angle needed to account for gravity for later.
            float angleNeededForProjectile = MathematicsUtility.GetAngleForFireProjectile(playerTransform.position, depoCollectionPoint.position, itemForce);

            // our forward
            Vector3 directionNoY = (new Vector3(depoCollectionPoint.position.x, 0, depoCollectionPoint.position.z) - new Vector3(playerTransform.position.x, 0, playerTransform.position.z)).normalized;
            // our right
            Vector3 directionRight = Quaternion.AngleAxis(-90, Vector3.up) * directionNoY;

            // the direction with gravity drop off acounted for.
            Vector3 forwardWithUpwardsAngle = Quaternion.AngleAxis(angleNeededForProjectile, directionRight) * (depoCollectionPoint.position - playerTransform.position).normalized;

            // add our force, boom, we have our force needed to reach that target point.
            Vector3 force = forwardWithUpwardsAngle.normalized * itemForce;

            // add the force to the object.
            scrapObject.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);

            // start the delay timer.
            currentDelay = ScrapManager.Instance.DepositRate;
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(Constants.PlayerTag))
        {
            if (playerTransform == null)
                playerTransform = other.transform;

            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(Constants.PlayerTag))
        {
            playerInRange = false;
        }
    }

}
