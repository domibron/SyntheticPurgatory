using UnityEngine;

/// <summary>
/// Randomly generates the forklift when the forklift object is enabled by gameObject.SetActive(true).
/// </summary>
public class ForkLiftRandomContainer : MonoBehaviour
{
    /// <summary>
    /// All the containers to pick from.
    /// </summary>
    [SerializeField]
    GameObject[] containers;

    void OnEnable()
    {
        foreach (var container in containers)
        {
            container.SetActive(false);
        }

        containers[UnityEngine.Random.Range(0, containers.Length)].SetActive(true);
    }
}
