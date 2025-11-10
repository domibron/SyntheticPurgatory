using UnityEngine;

public class DepotGrateController : MonoBehaviour
{
    [SerializeField]
    private Animator grateAnimator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.PlayerTag))
        {
            grateAnimator.SetBool("Opened", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Constants.PlayerTag))
        {
            grateAnimator.SetBool("Opened", false);
        }
    }
}
