using System;
using UnityEngine;

public class ScaleWithCamDistance : MonoBehaviour
{
    Transform camTransform;

    float currentMultiplier;
    Vector3 curScale;
    RemoveAfterTimeWithEasing easingComponent;

    [SerializeField]
    float fallOffPower = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (transform.GetComponent<RemoveAfterTimeWithEasing>() != null)
        {
            easingComponent = transform.GetComponent<RemoveAfterTimeWithEasing>();
        }

        curScale = transform.localScale;
        SetTargetCamera();
    }

    void LateUpdate()
    {
        if (camTransform == null)
        {
            SetTargetCamera();
            return;
        }

        if (easingComponent) { curScale = easingComponent.GetStoredScale(); }

        currentMultiplier = GetTargetScale();
        transform.localScale = new Vector3(curScale.x * currentMultiplier, curScale.y * currentMultiplier, curScale.z * currentMultiplier);
    }

    public float GetTargetScale()
    {
        float distanceFromCam = Vector3.Distance(transform.position, camTransform.position);

        return Mathf.Pow(distanceFromCam, fallOffPower) / 4;
    }


    public void SetTargetCamera()
    {
        try
        {
            camTransform = Camera.main.transform;
        }
        catch (NullReferenceException)
        {
            Debug.LogError("Main camera was not detected!", this);
        }
    }
}
