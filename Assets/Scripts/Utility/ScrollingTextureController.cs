using UnityEngine;

public class ScrollingTextureController : MonoBehaviour
{
    [Header("Only works with toon shader ATM")]
    public Vector2 ScrollDirection = Vector2.up;

    public float ScrollSpeed = 1;

    public bool IsScrolling = true;

    public bool IsReversed = false;

    // [SerializeField]
    private Material material;

    private Vector2 currentScroll;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentScroll.x < 0)
        {
            currentScroll.x = 1;
        }
        else if (currentScroll.x > 1)
        {
            currentScroll.x = 0;
        }

        if (currentScroll.y < 0)
        {
            currentScroll.y = 1;
        }
        else if (currentScroll.y > 1)
        {
            currentScroll.y = 0;
        }

        if (IsScrolling)
        {
            currentScroll += (IsReversed ? -1 : 1) * ScrollDirection * Time.deltaTime * ScrollSpeed;
        }

        material.SetTextureOffset("_MainTex", currentScroll);
        material.SetTextureOffset("_NormalMap", currentScroll);
    }
}
