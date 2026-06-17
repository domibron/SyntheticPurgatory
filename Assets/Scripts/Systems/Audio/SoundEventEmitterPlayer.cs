using UnityEngine;

public class SoundEventEmitterPlayer : MonoBehaviour
{
    [SerializeField]
    FMODUnity.StudioEventEmitter emitter;
    FMOD.Studio.EventInstance eventInstance;

    [SerializeField]
    bool is3DSound = true;

    [SerializeField]
    bool isPausable = true;

    void Awake()
    {
        if (emitter == null)
        {
            emitter = GetComponent<FMODUnity.StudioEventEmitter>();
        }

        if (emitter == null)
        {
            Debug.LogError($"Could not get {nameof(FMODUnity.StudioEventEmitter)}, make sure to attach it to this object or link the reference in {nameof(emitter)}.", gameObject);
        }
    }

    void Start()
    {
        // eventInstance = FMODUnity.RuntimeManager.CreateInstance(emitter.EventReference);
        // FMODUnity.RuntimeManager.AttachInstanceToGameObject(eventInstance, gameObject, true);
    }

    void Update()
    {
        if (!isPausable || !eventInstance.isValid()) return;

        bool paused = Time.timeScale == 0;

        eventInstance.setPaused(paused);
    }

    public void Play()
    {
        emitter.Play();

        eventInstance = emitter.EventInstance;

        if (is3DSound)
        {
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(eventInstance, gameObject, true);
        }
    }

    public void Stop()
    {
        emitter.Stop();

        if (is3DSound)
        {
            FMODUnity.RuntimeManager.DetachInstanceFromGameObject(eventInstance);
        }


    }
}
