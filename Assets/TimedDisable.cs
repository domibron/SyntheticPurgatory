using System.Collections;
using UnityEngine;

public class TimedDisable : MonoBehaviour
{
    [SerializeField]
    private MonoBehaviour selectedObject;

    [SerializeField]
    private float disableTime = 0.5f;


    private void Awake()
    {
        selectedObject.enabled = false;
        StartCoroutine(EnableComponent());
    }

    IEnumerator EnableComponent()
    {
        yield return new WaitForSeconds(disableTime);

        selectedObject.enabled = true;
    }
}
