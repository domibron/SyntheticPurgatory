using UnityEngine;

public class PlayerRefFetcher : MonoBehaviour
{
    public static PlayerRefFetcher Instance { get => instance; }

    private static PlayerRefFetcher instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    public GameObject GetPlayerRef()
    {
        return gameObject;
    }
}
