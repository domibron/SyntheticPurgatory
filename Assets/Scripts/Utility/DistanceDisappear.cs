using System;
using TMPro;
using UnityEngine;

public class DistanceDisappear : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer spriteRenderer;
    [SerializeField]
    TMP_Text textRendererAlternative;

    float baseOpacity;
    Transform camTransform;

    [SerializeField]
    float minDistance = 0;
    [SerializeField]
    float maxDistance = 10;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (spriteRenderer == null)
        {
            baseOpacity = textRendererAlternative.color.a;
        }
        else
        {
            baseOpacity = spriteRenderer.color.a;
        }
            
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

        float distanceFromCam = Vector3.Distance(transform.position, camTransform.position);
        float targetOpacity = Mathf.Clamp(distanceFromCam - minDistance, 0 , maxDistance) / maxDistance * baseOpacity;

        if (spriteRenderer == null)
        {
            textRendererAlternative.color = new Color(textRendererAlternative.color.r, textRendererAlternative.color.g, textRendererAlternative.color.b, targetOpacity);
        }
        else
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, targetOpacity);
        }
        

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
