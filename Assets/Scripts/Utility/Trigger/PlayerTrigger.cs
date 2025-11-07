using UnityEngine;
using UnityEngine.Events;

public class PlayerTrigger : MonoBehaviour
{
    public UnityEvent OnTriggerEnterEvent;
    public UnityEvent OnTriggerStayEvent;
    public UnityEvent OnTriggerExitEvent;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(Constants.PlayerTag))
        {
            OnTriggerEnterEvent?.Invoke();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag(Constants.PlayerTag))
        {
            OnTriggerStayEvent?.Invoke();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(Constants.PlayerTag))
        {
            OnTriggerExitEvent?.Invoke();
        }
    }
}
