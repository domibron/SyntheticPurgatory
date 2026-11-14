using System;
using UnityEngine;

public class DeltDamageStorer : MonoBehaviour
{
    [SerializeField]
    bool isPlayer = false;

    RunManager gameManager;

    void Awake()
    {
        gameManager = RunManager.Instance;
        GetComponent<Health>().HealthUpdated += OnHealthChanged;
    }


    void OnDisable()
    {
        GetComponent<Health>().HealthUpdated -= OnHealthChanged;
    }

    private void OnHealthChanged(HealthChangedContext context)
    {
        float damageReceived = context.PreviousHealth - context.CurrentHealth;

        if (damageReceived <= 0) return;

        // Bug fix, enemies not taking damage when not loading from persistent / boot strap
        if (gameManager == null) return;

        if (!isPlayer)
            gameManager.statsHolder.damageDealt += damageReceived;
        else
            gameManager.statsHolder.damageReceived += damageReceived;
    }




}
