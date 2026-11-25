using UnityEngine;
using UnityEngine.Events;

public class SequenceController : MonoBehaviour
{
    public UnityEvent OnSequenceEnd;

    public void SequenceEnd()
    {
        OnSequenceEnd?.Invoke();
    }
}
