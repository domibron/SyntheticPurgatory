using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Unity event version for the health on death event to plug scripts in using inspector.
/// <br />May change health events into unityEvents.
/// </summary>
public class EntityOnDeath : MonoBehaviour
{
    public UnityEvent OnDeathEvent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Health>().onDeath += OnDeath;
    }

    private void OnDeath()
    {
        OnDeathEvent?.Invoke();
    }
}
