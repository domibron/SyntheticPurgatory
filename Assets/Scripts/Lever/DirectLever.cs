using UnityEngine;
using UnityEngine.Events;

public class DirectLever : MonoBehaviour, IKickable, IShootable
{
    public UnityEvent OnLeverActivate;

    public void HitObject()
    {
        OnLeverActivate?.Invoke();
    }

    public void KickObject(Vector3 forceAndDir, ForceMode forceMode = ForceMode.Force)
    {
        OnLeverActivate?.Invoke();

    }
}
