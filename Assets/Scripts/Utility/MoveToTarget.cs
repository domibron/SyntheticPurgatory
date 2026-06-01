using UnityEngine;

/// <summary>
/// Move camera to target 
/// </summary>
public class MoveToTarget : MonoBehaviour
{
    [SerializeField]
    private Transform TargetLocation;

    private bool isEnabled = true;

    // Update is called once per frame
    void Update()
    {
        if (!isEnabled) return;

        transform.position = TargetLocation.position;
        transform.rotation = TargetLocation.rotation;
    }

    public void SetEnabled(bool state = true)
    {
        isEnabled = state;
    }

    public bool IsEnabled()
    {
        return isEnabled;
    }
}
