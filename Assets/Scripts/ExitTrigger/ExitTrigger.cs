using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    [SerializeField]
    private bool isTutorial;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(Constants.PlayerTag))
        {
            if (!isTutorial)
            {
                GameManager.Instance?.ReturnToHubWorld();
            }
            else
            {
                LevelLoading.Instance.LoadScene(LevelCollection.LevelKey.TutorialHub.ToString());
            }
            
        }
    }
}
