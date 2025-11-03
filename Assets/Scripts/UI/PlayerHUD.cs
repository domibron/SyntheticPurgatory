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

    // [SerializeField]
    // private TMP_Text ammoText;

    [SerializeField]
    private Image healthBarFill;

    [SerializeField]
    private TMP_Text currentTimeText;

    [SerializeField]
    private Image damageVignette;

    private float apearTime = 0.3f;

    private float currentApearTime = 0f;

    private float savedAlpha = 0f;

    [SerializeField]
    private Image chargeCursor;
    [SerializeField]
    private float minAngle = -90;
    [SerializeField]
    private float maxAngle = -270;

    [SerializeField]
    private Image chargeUpSegment;

    [SerializeField]
    private Image chargeBar;


    [SerializeField]
    private GameObject weaponChargeBar;

    [SerializeField]
    private Image weaponChargeBarFill;

    [SerializeField]
    private Image basicPassIndicator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerObject = PlayerRefFetcher.Instance.GetPlayerRef();
        playerCombat = playerObject.GetComponent<PlayerCombat>();
        playerHealth = playerObject.GetComponent<Health>();
        playerHealth.onHealthChanged += OnHealthChanged;

        gameManager = GameManager.Instance;
        if (gameManager != null) gameManager.StartTimer(); // TODO: move to level generator.

        savedAlpha = damageVignette.color.a;

        playerCombat.OnShowChargeBar += OnShowChargeBar;
        playerCombat.OnHideChargeBar += OnHideChargeBar;
        OnHideChargeBar();
    }

    private void OnHideChargeBar()
    {
        weaponChargeBar.SetActive(false);
    }

    private void OnShowChargeBar()
    {
        weaponChargeBar.SetActive(true);

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
        // ammoText.text = "REMOVED MECHANIC";
        healthBarFill.fillAmount = playerHealth.GetHealthNormalized();

        if (GameManager.Instance != null)
            currentTimeText.text = ((int)gameManager.GetCurrentTime() / 60).ToString() + ":" + ((float)gameManager.GetCurrentTime() % 60f).ToString("F2");

        if (currentApearTime > 0) currentApearTime -= Time.deltaTime;
        damageVignette.color = new Color(damageVignette.color.a, damageVignette.color.g, damageVignette.color.b, Mathf.Lerp(0, savedAlpha, currentApearTime / apearTime));


        // float halfwidth = chargeBar.rectTransform.sizeDelta.x / 2f;


        // chargeCursor.rectTransform.localPosition = new Vector3((2f * playerCombat.GetCursorPos() - 1f) * halfwidth, 0, 0);
        chargeCursor.rectTransform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, 0, minAngle), Quaternion.Euler(0, 0, maxAngle), playerCombat.GetCursorPos());

        // chargeUpSegment.rectTransform.sizeDelta = new Vector2(playerCombat.GetChargeUpSize() * (halfwidth * 2f), chargeUpSegment.rectTransform.sizeDelta.y);
        // chargeUpSegment.rectTransform.localPosition = new Vector3((2f * playerCombat.GetChargeUpPos() - 1f) * halfwidth, 0, 0);
        chargeUpSegment.fillAmount = (Mathf.Lerp(0, 180, playerCombat.GetChargeUpSize()) / 360f);
        float rotationAmount = Mathf.Lerp(270, 90, playerCombat.GetChargeUpPos()) + ((360f * chargeUpSegment.fillAmount) / 2f);
        chargeUpSegment.rectTransform.localRotation = Quaternion.Euler(0, 0, rotationAmount);


        weaponChargeBarFill.fillAmount = playerCombat.GetChargeAmount();

        // basicPassIndicator.rectTransform.sizeDelta = new Vector2(halfwidth, basicPassIndicator.rectTransform.sizeDelta.y);
        // basicPassIndicator.rectTransform.localPosition = new Vector3((playerCombat.IsChargeOnLeftSide() ? -halfwidth / 2f : halfwidth / 2f), 0, 0);
        basicPassIndicator.rectTransform.localRotation = Quaternion.Euler(0, 0, (playerCombat.IsChargeOnLeftSide() ? 270 : 180));
    }


}
