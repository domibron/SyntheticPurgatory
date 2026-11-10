using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ScrollBG : MonoBehaviour
{
    [SerializeField, FormerlySerializedAs("image")]
    Image m_image;

    [SerializeField, FormerlySerializedAs("direction")]
    Vector2 m_direction = Vector2.right;

    [SerializeField, FormerlySerializedAs("speed")]
    float m_speed = 1;

    Material m_scrollMat;

    void Awake()
    {
        if (m_image == null) m_image = GetComponent<Image>();

        if (m_image == null) return;

        m_scrollMat = new Material(m_image.material);

        m_image.material = m_scrollMat;
    }


    // Update is called once per frame
    void Update()
    {
        m_image.material.mainTextureOffset += m_direction * m_speed * Time.deltaTime;
    }
}
