using UnityEngine;

public class RandomlyShowOneMesh : MonoBehaviour
{
    [SerializeField]
    GameObject[] Objects;

    [SerializeField]
    bool destroyOthers = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (destroyOthers)
            RandomlyEnableObjectDestroyRest();
        else
            RandomlyEnableObject();
    }

    public void RandomlyEnableObject()
    {
        foreach (var obj in Objects)
        {
            obj.SetActive(false);
        }

        Objects[UnityEngine.Random.Range(0, Objects.Length)].SetActive(true);
    }

    public void RandomlyEnableObjectDestroyRest()
    {
        foreach (var obj in Objects)
        {
            obj.SetActive(false);
        }

        Objects[UnityEngine.Random.Range(0, Objects.Length)].SetActive(true);

        foreach (var obj in Objects)
        {
            if (!obj.activeSelf)
            {
                Destroy(obj);
            }
        }
    }
}
