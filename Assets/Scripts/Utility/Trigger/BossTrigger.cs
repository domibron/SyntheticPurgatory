using UnityEngine;
using UnityEngine.Events;


public class BossTrigger : MonoBehaviour
{
    public UnityEvent OnTriggerEnterEvent;
    public UnityEvent OnTriggerStayEvent;
    public UnityEvent OnTriggerExitEvent;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(Constants.BossTag))
        {
            OnTriggerEnterEvent?.Invoke();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag(Constants.BossTag))
        {
            OnTriggerStayEvent?.Invoke();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(Constants.BossTag))
        {
            OnTriggerExitEvent?.Invoke();
        }
    }
}
