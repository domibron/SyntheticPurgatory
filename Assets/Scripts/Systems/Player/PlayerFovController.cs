using UnityEngine;

/// <summary>
/// Fov manager to manage all camera fovs the player uses.
/// </summary>
public class PlayerFovController : MonoBehaviour
{
    [SerializeField]
    private float m_standardFov = 60;

    private float m_extraFov = 0f;

    [SerializeField]
    Camera[] m_allCameras;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateAllCameraFovs();
    }

    private void UpdateAllCameraFovs()
    {
        foreach (var cam in m_allCameras)
        {
            cam.fieldOfView = GetFovCombined();
        }
    }

    public void SetExtraFov(float value)
    {
        m_extraFov = value;
    }

    public float GetExtraFov()
    {
        return m_extraFov;
    }

    public float GetFovCombined()
    {
        return m_standardFov + m_extraFov;
    }
}
