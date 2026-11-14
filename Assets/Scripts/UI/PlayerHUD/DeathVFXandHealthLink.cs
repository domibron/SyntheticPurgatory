using System;
using UnityEngine;

public class DeathVFXandHealthLink : MonoBehaviour
{
    GameViewFilterFSRF m_deathEffect;

    Health m_playerHealth;

    bool m_isDead = false;
    // bool m_takenDamage = false;

    // float m_localTime = 0f;

    float m_deathFadeIn = 0.3f;

    float m_damageFadeIn = 0.1f;

    float m_damageIntensityMax = 0.3f;
    float m_damageIntensityStart = 0.9f;

    float m_smoothIntensity = 0f;
    float m_currentDamageIntensity = 0f;

    float m_lastHealthState = 1f;


    void Start()
    {
        m_playerHealth = GetComponent<Health>();

        m_playerHealth.OnDeath += OnDeath;

        m_deathEffect = GameViewFilterFSRF.Instance;
        if (m_deathEffect == null) Debug.LogError($"{nameof(GameViewFilterFSRF)} is not ready!");
    }


    void Update()
    {
        m_lastHealthState = m_playerHealth.GetHealthNormalized();

        if (!m_deathEffect) m_deathEffect = GameViewFilterFSRF.Instance;


        if (m_isDead)
        {
            // m_localTime += Time.fixedDeltaTime * (1 / m_deathFadeIn);
            // Mathf.Clamp01(m_localTime);
            // m_currentDamageIntensity = Mathf.Lerp(m_currentDamageIntensity, 1, Time.fixedDeltaTime * (1 / m_deathFadeIn));

            m_smoothIntensity = Mathf.Lerp(m_smoothIntensity, 1, Time.deltaTime * (1 / m_deathFadeIn));

            m_currentDamageIntensity = Mathf.Lerp(0, 1, m_smoothIntensity);

            m_deathEffect?.SetEffectActive(true);
            m_deathEffect?.SetIntensity(m_currentDamageIntensity);

            return;
        }



        if (m_lastHealthState >= m_damageIntensityStart)
        {

            if (m_smoothIntensity > 0)
            {
                m_smoothIntensity -= Time.deltaTime * (1 / m_damageFadeIn);

                m_deathEffect?.SetDarken();
                m_deathEffect?.SetEffectActive(true);
                m_deathEffect?.SetIntensity(m_currentDamageIntensity);
            }
            else
            {
                m_smoothIntensity = 0f;
                m_deathEffect?.ResetEffect();
            }

            return;
        }



        float target01 = (m_damageIntensityStart - m_lastHealthState) / m_damageIntensityStart;
        // print($"{EaseExpo(target01)}");

        m_smoothIntensity = Mathf.Lerp(m_smoothIntensity, EaseExpo(target01), Time.deltaTime * (1 / m_damageFadeIn));

        m_currentDamageIntensity = Mathf.Lerp(0, m_damageIntensityMax, m_smoothIntensity);

        m_deathEffect?.SetDarken();
        m_deathEffect?.SetEffectActive(true);
        m_deathEffect?.SetIntensity(m_currentDamageIntensity);
    }

    private float EaseExpo(float x)
    {
        x = Mathf.Clamp01(x);
        return x == 0 ? 0 : Mathf.Pow(2, 10 * x - 10);
    }

    private void OnDeath()
    {
        m_isDead = true;
        // m_localTime = 0f;
        // m_smoothIntensity = 0f;
        m_deathEffect.SetDarken(); // only once.
    }
}
