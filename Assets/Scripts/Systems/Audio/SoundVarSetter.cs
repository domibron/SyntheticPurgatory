using UnityEngine;

public class SoundVarSetter : MonoBehaviour
{
    [SerializeField]
    FMODUnity.StudioEventEmitter emitter;
    FMOD.Studio.EventInstance eventInstance;

    void Start()
    {
        eventInstance = FMODUnity.RuntimeManager.CreateInstance(emitter.EventReference);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(eventInstance, gameObject, true);
    }
}
