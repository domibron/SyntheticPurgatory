using System;
using UnityEngine;

public class DeltDamageStorer : MonoBehaviour
{
    [SerializeField]
    bool isPlayer = false;

    GameManager gameManager;

    void Awake()
    {
        gameManager = GameManager.Instance;
        GetComponent<Health>().OnHealthChanged += OnHealthChanged;
    }


    void OnDisable()
    {
        GetComponent<Health>().OnHealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(float newHealthValue, float oldHealthValue)
    {
        float damageReceived = oldHealthValue - newHealthValue;

        if (damageReceived <= 0) return;

        if (!isPlayer)
            gameManager.statsHolder.damageDealt += damageReceived;
        else
            gameManager.statsHolder.damageReceived += damageReceived;
    }




}
