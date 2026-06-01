using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform targetLocation;

    private bool isDisabled = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isDisabled) return;

        transform.position = targetLocation.position;
        transform.rotation = targetLocation.rotation;
    }

    public void SetCameraDisabled(bool state)
    {
        isDisabled = state;
    }

    public bool IsCameraDisabled()
    {
        return isDisabled;
    }
}
