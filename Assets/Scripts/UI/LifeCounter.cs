using UnityEngine;
using UnityEngine.UI;

public class LifeCounter : MonoBehaviour
{
    [SerializeField]
    private Sprite lifeSprite;
    [SerializeField]
    private Sprite brokenLifeSprite;

    /// <summary>
    /// Life object prefab
    /// </summary>
    [SerializeField]
    private GameObject lifeObject;

    /// <summary>
    /// Object which holds the life objects
    /// </summary>
    [SerializeField]
    private GameObject lifeHolder;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateHearts();
    }

    public void GenerateHearts()
    {
        foreach (Transform child in lifeHolder.transform)
        {
            Destroy(child.gameObject);
        }

        if (GameManager.Instance == null)
        {
            GameObject heart = Instantiate(lifeObject, lifeHolder.transform);
            heart.GetComponent<Image>().sprite = brokenLifeSprite;
            return;
        }

        for (int i = 0; i < GameManager.Instance.GetMaxLives(); i++)
        {
            GameObject heart = Instantiate(lifeObject, lifeHolder.transform);
            if (i >= GameManager.Instance.GetCurrentLives())
            {
                heart.GetComponent<Image>().sprite = brokenLifeSprite;
            }
            else
            {
                heart.GetComponent<Image>().sprite = lifeSprite;
            }
        }
    }

}
