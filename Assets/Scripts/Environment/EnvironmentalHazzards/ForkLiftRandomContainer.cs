using UnityEngine;

public class ForkLiftRandomContainer : MonoBehaviour
{
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
