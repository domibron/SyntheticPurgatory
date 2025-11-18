using UnityEngine;
using UnityEngine.Events;

public class DirectLever : MonoBehaviour, IKickable, IShootable
{
    public UnityEvent OnLeverActivate;

    private Animator animator;

    private bool state = false;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        animator.SetBool("isOn", state);
    }

    public void HitObject()
    {
        OnLeverActivate?.Invoke();
        state = !state;
    }

    public void KickObject(Vector3 forceAndDir, ForceMode forceMode = ForceMode.Force)
    {
        OnLeverActivate?.Invoke();
        state = !state;
    }
}
