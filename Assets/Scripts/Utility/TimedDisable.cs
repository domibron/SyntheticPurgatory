using System.Collections;
using UnityEngine;

// By Vince Pressey

public class TimedDisable : MonoBehaviour
{
    /// <summary>
    /// Script that will be disabled on awake
    /// </summary>
    [SerializeField]
    private MonoBehaviour selectedObject;
    /// <summary>
    /// Time in seconds the script will be disabled for
    /// </summary>
    [SerializeField]
    private float disableTime = 0.5f;


    private void Awake()
    {
        selectedObject.enabled = false; // Disable Object
        StartCoroutine(EnableComponent(disableTime)); // Start countdown coroutine
    }

    /// <summary>
    /// Enabled a component after a set amount of seconds
    /// </summary>
    IEnumerator EnableComponent(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        selectedObject.enabled = true; // Re-enable object
    }
}
