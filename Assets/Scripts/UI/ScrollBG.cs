using UnityEngine;
using UnityEngine.UI;

public class ScrollBG : MonoBehaviour
{
    [SerializeField]
    Image image;

    [SerializeField]
    Vector2 direction = Vector2.right;

    [SerializeField]
    float speed = 1;

    void Awake()
    {
        if (image == null) image = GetComponent<Image>();
    }


    // Update is called once per frame
    void Update()
    {
        image.material.mainTextureOffset += direction * speed * Time.deltaTime;
    }
}
