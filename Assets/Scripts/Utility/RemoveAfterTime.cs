using UnityEngine;


/// <summary>
/// Removes the game object this is attached to after a period of time.
/// </summary>
public class RemoveAfterTime : MonoBehaviour
{
    /// <summary>
    /// How long to wait before destroying the game object this is attached to.
    /// </summary>
    public float timeToWaitBeforeRemoving = 5f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timeToWaitBeforeRemoving);
    }
}
