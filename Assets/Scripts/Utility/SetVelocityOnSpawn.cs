using UnityEngine;

public class SetVelocityOnSpawn : MonoBehaviour
{
    public Vector3 forceOrigin;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector3 desiredForce = (transform.position - forceOrigin).normalized;
        transform.GetComponent<Rigidbody>().AddForce(desiredForce * 5 + Vector3.up / 2, ForceMode.VelocityChange);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
