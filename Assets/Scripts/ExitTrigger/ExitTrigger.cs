using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(Constants.PlayerTag))
        {
            GameManager.Instance?.ReturnToHubWorld();
        }
    }
}
