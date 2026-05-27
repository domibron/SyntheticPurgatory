using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Destroy any object with the depo scrap tag.
/// </summary>
public class RemoveDepositScrap : MonoBehaviour
{
    public UnityEvent OnScrapRemoved;


    void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag(Constants.DepoScrapTag)) return;

        OnScrapRemoved?.Invoke();


        Destroy(other.gameObject);
    }
}
