using System;
using UnityEngine;

public class PlayerRefFetcher : MonoBehaviour
{
    public static PlayerRefFetcher Instance { private set; get; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject); //  fuck the other one.
            Instance = this;
        }
        else
        {
            Instance = this;
        }

        Debug.Log("PLAYER REF FETCHER: Has set up the reference!");
    }

    [Obsolete("It just gets this game object. Don't bother using this function", false)]
    public GameObject GetPlayerRef()
    {
        return gameObject;
    }
}
