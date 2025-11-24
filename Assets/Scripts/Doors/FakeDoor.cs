using UnityEngine;

public class FakeDoor : MonoBehaviour, IKickable, IShootable
{
    private Rigidbody rb;

    private bool isKnockedOver = false;

    [SerializeField]
    float targetSpeed = 30f;



    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!isKnockedOver) return;

        Vector3 angVel = rb.angularVelocity;
        rb.AddRelativeTorque((new Vector3(-targetSpeed * Time.deltaTime, 0, 0) * Mathf.Deg2Rad) - angVel);

    }

    void IShootable.HitObject()
    {
        rb.isKinematic = false;
        isKnockedOver = true;
    }

    void IKickable.KickObject(Vector3 forceAndDir, ForceMode forceMode)
    {
        rb.isKinematic = false;
        isKnockedOver = true;
    }
}
