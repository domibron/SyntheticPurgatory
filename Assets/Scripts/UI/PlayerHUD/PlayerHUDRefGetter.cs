using UnityEngine;

public class PlayerHUDRefGetter : MonoBehaviour
{
    public static PlayerHUDRefGetter Instance { get; private set; }

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

    public GameObject GetRef()
    {
        return gameObject;
    }
}
