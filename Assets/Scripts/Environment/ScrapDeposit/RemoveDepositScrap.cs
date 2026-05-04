using UnityEngine;
using UnityEngine.Events;

public class RemoveDepositScrap : MonoBehaviour
{
    public UnityEvent OnScrapRemoved;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag(Constants.DepoScrapTag)) return;

        OnScrapRemoved?.Invoke();


        Destroy(other.gameObject);
    }

    // void OnTriggerExit(Collider other)
    // {

    // }
}
