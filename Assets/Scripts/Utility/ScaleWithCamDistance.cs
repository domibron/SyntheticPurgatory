using System;
using UnityEngine;

public class ScaleWithCamDistance : MonoBehaviour
{
    Vector3 baseScale;
    Transform camTransform;

    [SerializeField]
    float fallOffPower = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        baseScale = transform.localScale;
        SetTargetCamera();
    }

    // Update is called once per frame
    void Update()
    {
        if (camTransform == null)
        {
            SetTargetCamera();
            return;
        }

        transform.localScale = new Vector3(GetTargetScale(baseScale.x), GetTargetScale(baseScale.y), GetTargetScale(baseScale.z));
    }

    public float GetTargetScale(float value)
    {
        float distanceFromCam = Vector3.Distance(transform.position, camTransform.position);

        return value * Mathf.Pow(distanceFromCam, fallOffPower) / 8;
    }


    public void SetTargetCamera()
    {
        try
        {
            camTransform = Camera.main.transform;
        }
        catch (NullReferenceException)
        {
            //Debug.LogError("Main camera was not detected!", this);
        }
    }
}
