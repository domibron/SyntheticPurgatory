using UnityEngine;

public class LevelGenObjectRefGetter : MonoBehaviour
{
    public static LevelGenObjectRefGetter Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public GameObject GetReference()
    {
        return this.gameObject;
    }
}
