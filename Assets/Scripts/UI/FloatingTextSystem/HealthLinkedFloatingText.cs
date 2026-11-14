using System;
using UnityEngine;

public class HealthLinkedFloatingText : MonoBehaviour
{
    [SerializeField]
    private Health m_health;

    [SerializeField]
    private FloatingTextSystem m_floatingText;

    void Start()
    {
        if (!m_health)
            m_health = GetComponent<Health>();

        if (m_health)
            m_health.HealthUpdated += OnHealthUpdated;
    }

    private void OnHealthUpdated(HealthChangedContext context)
    {
        throw new NotImplementedException();
    }
}
