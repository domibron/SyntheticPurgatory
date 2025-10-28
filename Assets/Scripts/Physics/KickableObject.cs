using UnityEngine;

public class KickableObject : MonoBehaviour, IKickable
{
    private Rigidbody rb;

    [SerializeField]
    private bool disableNavAgent;

    [SerializeField]
    private BaseEnemy enemyClass; 

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void IKickable.KickObject(Vector3 forceAndDir, ForceMode forceMode)
    {
        Vector3 alteredForceDir;

        if (disableNavAgent)
        {
            enemyClass.KnockbackAI();

            // Alter given force to have forced upward direction and to account for the mass of the object
            alteredForceDir = new Vector3(forceAndDir.x * 5 / (rb.mass / 2), Mathf.Max(forceAndDir.y, 5), forceAndDir.z * 5 / (rb.mass / 2));
            rb.AddForce(alteredForceDir, forceMode);

            return;
        }

        alteredForceDir = new Vector3(forceAndDir.x / (rb.mass / 2), Mathf.Max(forceAndDir.y, 5), forceAndDir.z / (rb.mass / 2));
        rb.AddForce(alteredForceDir, forceMode);
    }
}
