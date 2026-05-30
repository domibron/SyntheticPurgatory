using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    private GameObject playerObject;

    private PlayerCombat playerCombat;
    private Health playerHealth;

    RunManager gameManager;

    // [SerializeField]
    // private TMP_Text ammoText;

    [SerializeField]
    float healthBarSmoothing = 5f;
    [SerializeField]
    float healthLossBarSmoothing = 5f;

    [SerializeField]
    float waitBeforeUpdating = 1f;

    private float waitTimer = 0f;

    private float currentValue = 0f;
    private float targetValue = 0f;
    private float animationTimer = 0f;


    [SerializeField]
    private Image healthBarImage;

    [SerializeField]
    private Image healthBarLossImage;

    [SerializeField]
    private TMP_Text currentTimeText;

    [SerializeField]
    private Image damageVignette;

    private float apearTime = 0.3f;

    private float currentApearTime = 0f;

    private float savedAlpha = 0f;

    private bool playerDied = false;

    private int lastDivisible = 0;

    private float fontSize = 0;

    [SerializeField]
    private Image gunChargeBarFill;
    [SerializeField]
    private Image meleeChargeBarFill;
    [SerializeField]
    private Image bashChargeBarFill;

    [SerializeField]
    private TMP_Text heldScrapText;
    private int curHeldScrapNum;
    [SerializeField]
    private TMP_Text depositedScrapText;
    private int curDepoScrapNum;
    [SerializeField]
    private float scrapCountersSpeed = 0.05f;
    private float curScrapCounterTime;

    [SerializeField]
    private GameObject bottomRightUI;

    private float flashOverheatTime = 0.5f;

    private float currentFlashTime = 0;
    private bool isFlash = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerObject = PlayerRefFetcher.Instance.GetPlayerRef();
        playerCombat = playerObject.GetComponent<PlayerCombat>();
        playerHealth = playerObject.GetComponent<Health>();
        playerHealth.OnHealthChanged += OnHealthChanged;

        gameManager = RunManager.Instance;

        savedAlpha = damageVignette.color.a;
        damageVignette.color = new Color(damageVignette.color.a, damageVignette.color.g, damageVignette.color.b, 0);

        // if (gameManager != null)
        // {
        //     lastDivisible = ((int)(gameManager.GetCurrentTime()) / 30);
        //     if (lastDivisible > 2) lastDivisible -= 1;
        // }

        lastDivisible = -1;

        fontSize = currentTimeText.fontSize;

        healthBarImage.fillAmount = playerHealth.GetHealthNormalized();
        currentValue = 1;
        targetValue = 1;
    }

    private void OnHealthChanged(float newAmount, float oldAmount)
    {
        if (newAmount - oldAmount < 0)
        {
            currentApearTime = apearTime;

            waitTimer = waitBeforeUpdating;
            animationTimer = 0f;
            if (healthBarLossImage.fillAmount < healthBarImage.fillAmount) healthBarLossImage.fillAmount = healthBarImage.fillAmount;

            currentValue = healthBarLossImage.fillAmount;
            targetValue = playerHealth.GetHealthNormalized();
        }

        if (newAmount <= 0)
        {
            playerDied = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager != null && lastDivisible == -1)
        {
            lastDivisible = ((int)(gameManager.GetCurrentTime()) / 30);
        }

        // print(lastDivisible + " " + ((int)(gameManager.GetCurrentTime()) / 30));

        // ammoText.text = "REMOVED MECHANIC";
        if (waitTimer <= 0 && playerHealth.GetHealthNormalized() <= currentValue)
        {
            animationTimer += (1 - Mathf.Abs(targetValue - currentValue)) * healthLossBarSmoothing * Time.deltaTime;
            healthBarLossImage.fillAmount = Mathf.Lerp(currentValue, targetValue, animationTimer);
        }
        else if (waitTimer <= 0 && playerHealth.GetHealthNormalized() > currentValue)
        {
            healthBarLossImage.fillAmount = healthBarImage.fillAmount;
        }
        else
        {
            waitTimer -= Time.deltaTime;
        }

        // float displacement = Mathf.Abs(healthBarImage.fillAmount - health.GetHealthNormalized());

        healthBarImage.fillAmount = Mathf.Lerp(healthBarImage.fillAmount, playerHealth.GetHealthNormalized(), healthBarSmoothing * Time.deltaTime);



        if (playerDied) { return; }

        if (RunManager.Instance != null)
        {
            if (gameManager == null) gameManager = RunManager.Instance;


            if (!RunManager.Instance.IsTimerHidden())
                currentTimeText.text = ((int)gameManager.GetCurrentTime() / 60).ToString() + ":" + (((float)gameManager.GetCurrentTime() % 60f) < 10 ? "0" : "") + ((float)gameManager.GetCurrentTime() % 60f).ToString("F2");
            else
                currentTimeText.text = "";


            if (lastDivisible > ((int)gameManager.GetCurrentTime() / 30))
            {
                lastDivisible = ((int)gameManager.GetCurrentTime() / 30);
                if (lastDivisible > 1)
                {
                    RunManager.Instance.InvokeRemindTime();
                    StartCoroutine(FlashTimer());
                }
                else
                    StartCoroutine(KeepFlashing());
            }
        }

        if (currentApearTime > 0) currentApearTime -= Time.deltaTime;
        damageVignette.color = new Color(damageVignette.color.a, damageVignette.color.g, damageVignette.color.b, Mathf.Lerp(0, savedAlpha, currentApearTime / apearTime));

        if (currentFlashTime > 0) currentFlashTime -= Time.deltaTime;

        if (playerCombat.GetOverheatCoolDownNormalized() > 0)
        {
            if (currentFlashTime <= 0)
            {
                isFlash = !isFlash;
                currentFlashTime = flashOverheatTime;
            }


            gunChargeBarFill.color = (isFlash ? Color.red : new Color(0.5f, 0, 0, 1f));
            gunChargeBarFill.fillAmount = playerCombat.GetOverheatCoolDownNormalized();
        }
        else
        {
            gunChargeBarFill.color = Color.green;
            gunChargeBarFill.fillAmount = playerCombat.GetGunChargeAmount();
        }
        // gunChargeBarFill.fillAmount = playerCombat.GetGunChargeAmount();
        meleeChargeBarFill.fillAmount = 1 - playerCombat.GetMeleeChargeAmount();
        bashChargeBarFill.fillAmount = 1 - playerCombat.GetBashChargeAmount();

        curScrapCounterTime -= Time.fixedDeltaTime;
        if (curScrapCounterTime < 0)
        {
            UpdateScrapCounters();
            curScrapCounterTime = scrapCountersSpeed;
        }

    }

    private IEnumerator FlashTimer()
    {
        yield return new WaitForEndOfFrame();

        float counting = 5;
        float waitTime = 0.25f;

        while (counting > 0)
        {

            currentTimeText.color = Color.red;
            currentTimeText.fontSize = fontSize + 20f;


            yield return new WaitForSeconds(waitTime / 2f);


            currentTimeText.color = Color.white;
            currentTimeText.fontSize = fontSize;


            counting--;
            yield return new WaitForSeconds(waitTime / 2f);
        }

        currentTimeText.color = Color.white;
        currentTimeText.fontSize = fontSize;
    }

    private IEnumerator KeepFlashing()
    {
        float waitTime = 0.25f;

        while (true)
        {

            currentTimeText.color = Color.red;
            currentTimeText.fontSize = fontSize + 20f;


            yield return new WaitForSeconds(waitTime / 2f);


            currentTimeText.color = Color.white;
            currentTimeText.fontSize = fontSize;

            yield return new WaitForSeconds(waitTime / 2f);
        }
    }

    private void UpdateScrapCounters() // Peak programming // yes, yes it is.
    {
        if (ScrapLevelM.Instance == null)
        {
            bottomRightUI.SetActive(false);
            return;
        }
        else
        {
            bottomRightUI.SetActive(true);
        }

        int targetInvScrap = ScrapLevelM.Instance.currentInventoryScrap;
        if (curHeldScrapNum < targetInvScrap) { curHeldScrapNum++; }
        else if (curHeldScrapNum > targetInvScrap) { curHeldScrapNum--; }

        int targetDepoScrap = ScrapLevelM.Instance.currentDepositedScrap;
        if (curDepoScrapNum < targetDepoScrap) { curDepoScrapNum++; }
        else if (curDepoScrapNum > targetDepoScrap) { curDepoScrapNum--; }

        heldScrapText.text = curHeldScrapNum.ToString().PadLeft(3, '0');
        depositedScrapText.text = curDepoScrapNum.ToString().PadLeft(3, '0');

        if (curHeldScrapNum >= ScrapLevelM.Instance.GetMaxScrapInventory())
        {
            heldScrapText.color = Color.red;
        }
        else
        {
            heldScrapText.color = Color.black;
        }
    }
}
