using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform targetLocation;

    public bool IsDisabled = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (IsDisabled) return;

        transform.position = targetLocation.position;
        transform.rotation = targetLocation.rotation;
    }

    public void DisableCameraInput(bool state)
    {
        IsDisabled = state;
    }
}
