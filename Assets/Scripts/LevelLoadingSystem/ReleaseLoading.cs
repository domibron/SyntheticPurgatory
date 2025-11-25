using UnityEngine;

public class ReleaseLoading : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LevelLoading.Instance?.ReleaseLevelLoading();
    }
}
