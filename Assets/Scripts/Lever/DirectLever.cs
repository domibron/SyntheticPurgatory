using UnityEngine;
using UnityEngine.Events;

public class DirectLever : MonoBehaviour, IKickable, IShootable
{
    public UnityEvent OnLeverActivate;

    private Animator animator;

    private bool state = false;

    private bool inMotion = false;

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
        if (inMotion) { return; }

        OnLeverActivate?.Invoke();
        state = !state;

        inMotion = true;
    }

    public void KickObject(Vector3 forceAndDir, ForceMode forceMode = ForceMode.Force)
    {
        if (inMotion) { return; }

        OnLeverActivate?.Invoke();
        state = !state;

        inMotion = true;
    }

    public void MotionEnded()
    {
        inMotion = false;
    }
}
