using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    private GameObject playerObject;

    private PlayerCombat playerCombat;
    private Health playerHealth;

    GameManager gameManager;

    [SerializeField]
    private TMP_Text ammoText;

    [SerializeField]
    private Image healthBarFill;

    [SerializeField]
    private TMP_Text currentTimeText;

    [SerializeField]
    private Image damageVignette;

    private float apearTime = 0.3f;

    private float currentApearTime = 0f;

    private float savedAlpha = 0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerObject = PlayerRefFetcher.Instance.GetPlayerRef();
        playerCombat = playerObject.GetComponent<PlayerCombat>();
        playerHealth = playerObject.GetComponent<Health>();
        playerHealth.onHealthChanged += OnHealthChanged;

        gameManager = GameManager.Instance;
        gameManager.StartTimer(); // TODO: move to level generator.

        savedAlpha = damageVignette.color.a;
    }

    private void OnHealthChanged(float newAmount, float oldAmount)
    {
        if (newAmount - oldAmount < 0)
        {
            currentApearTime = apearTime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ammoText.text = playerCombat.GetCurrentAmmo().ToString() + "/" + playerCombat.GetMaxAmmo().ToString();
        healthBarFill.fillAmount = playerHealth.GetHealthNormalized();
        currentTimeText.text = ((int)gameManager.GetCurrentTime() / 60).ToString() + ":" + ((float)gameManager.GetCurrentTime() % 60f).ToString("F2");

        if (currentApearTime > 0) currentApearTime -= Time.deltaTime;
        damageVignette.color = new Color(damageVignette.color.a, damageVignette.color.g, damageVignette.color.b, Mathf.Lerp(0, savedAlpha, currentApearTime / apearTime));
    }


}
