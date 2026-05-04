using UnityEngine;
using UnityEngine.Events;

public class DirectLever : MonoBehaviour, IKickable, IShootable, IMeleeable
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

    public void MotionEnded()
    {
        inMotion = false;
    }

    public void HitObject()
    {
        ToggleLever();
    }

    public void KickObject(Vector3 forceAndDir, ForceMode forceMode = ForceMode.Force)
    {
        ToggleLever();

    }

    public void MeleeObject()
    {
        ToggleLever();

    }


    private void ToggleLever()
    {
        if (inMotion) { return; }

        OnLeverActivate?.Invoke();
        state = !state;

        inMotion = true;
    }
}
