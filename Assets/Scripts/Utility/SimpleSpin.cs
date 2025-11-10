using UnityEngine;

public class SimpleSpin : MonoBehaviour
{
    public Vector3 spinDirection;
    public float spinSpeed;


    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(spinDirection * spinSpeed);
    }
}
