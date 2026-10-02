using UnityEngine;
using UnityEngine.Serialization;

public class VisualFollowTarget : MonoBehaviour
{
    [SerializeField, FormerlySerializedAs("target")]
    Transform m_target;

    [SerializeField]
    float m_timeUntilConverging = 0.1f;

    Vector3 m_targetsVel;

    Vector3 m_convergentPoint;

    float m_localTimer = 0f;

    Vector3 startPoint;


    void Update()
    {
        if (m_target == null)
        {
            Destroy(gameObject);
            return;
        }

        if (m_localTimer < m_timeUntilConverging)
        {
            m_localTimer += Time.deltaTime;
            transform.position = Vector3.Lerp(startPoint, m_convergentPoint, m_localTimer / m_timeUntilConverging);
        }
        else
        {
            transform.position = m_target.position;
        }
    }

    public void SetVisualTargetLocation(Vector3 world, Vector3 targetVel)
    {
        transform.position = world;
        transform.parent = null;

        m_targetsVel = targetVel;

        startPoint = world;
        m_convergentPoint = m_target.position + (m_targetsVel * m_timeUntilConverging);

        // print(m_convergentPoint + " " + startPoint);
    }
}
