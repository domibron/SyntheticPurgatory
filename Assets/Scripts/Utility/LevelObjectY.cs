using UnityEngine;

/// <summary>
/// Levels the object on the Y axis with the target y value specified.
/// </summary>
public class LevelObjectY : MonoBehaviour
{

    /// <summary>
    /// The target y value to set this object's height at.
    /// </summary>
    [SerializeField]
    float targetYLevel = 0;


    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x, targetYLevel, transform.position.z);
    }
}
