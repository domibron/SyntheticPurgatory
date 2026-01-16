using UnityEngine;

public class UIAudioSource : MonoBehaviour
{
    public static UIAudioSource Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public AudioSource GetAudioSource()
    {
        return GetComponent<AudioSource>();
    }
}
