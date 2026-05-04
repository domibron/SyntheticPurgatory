using UnityEngine;

public class LevelFog : MonoBehaviour
{
    // Very bad quick script

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }
}
