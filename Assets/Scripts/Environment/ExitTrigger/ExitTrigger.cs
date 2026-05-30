using UnityEngine;

/// <summary>
/// When the player touches this trigger they are sent back to the hub world.
/// </summary>
public class ExitTrigger : MonoBehaviour
{
    /// <summary>
    /// Whether to load the tutorial hub world (true) or the regular hub world.
    /// </summary>
    [SerializeField]
    private bool isTutorial;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(Constants.PlayerTag))
        {
            if (isTutorial)
            {
                LevelLoading.Instance.LoadScene(LevelCollection.LevelKey.TutorialHub.ToString());
            }
            else
            {
                RunManager.Instance?.ReturnToHubWorld();
            }

        }
    }
}
