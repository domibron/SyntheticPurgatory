using UnityEngine;
using UnityEngine.Events;

public class DirectLever : MonoBehaviour, IKickable
{

    // private int roomID = -1;

    public UnityEvent OnLeverActivate;

    public void KickObject(Vector3 forceAndDir, ForceMode forceMode = ForceMode.Force)
    {
        OnLeverActivate?.Invoke();

    }
}
