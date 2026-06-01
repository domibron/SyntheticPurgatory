using UnityEngine;

// TODO: find a opt in solution. This is bad, having to add additional function for normal behaviour.

/// <summary>
/// Releases the level loading. Use if you are not handling overriding the loading.
/// </summary>
public class ReleaseLoading : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LevelLoading.Instance?.ReleaseLevelLoading();
    }
}
