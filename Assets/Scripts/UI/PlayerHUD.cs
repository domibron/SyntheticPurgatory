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

    private bool playerDied = false;

    private int lastDivisible = 0;

    private float fontSize = 0;

    [SerializeField]
    private Image weaponChargeBarFill;

    [SerializeField]
    private TMP_Text heldScrapText;
    private int curHeldScrapNum;
    [SerializeField]
    private TMP_Text depositedScrapText;
    private int curDepoScrapNum;
    [SerializeField]
    private float scrapCountersSpeed = 0.05f;
    private float curScrapCounterTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerObject = PlayerRefFetcher.Instance.GetPlayerRef();
        playerCombat = playerObject.GetComponent<PlayerCombat>();
        playerHealth = playerObject.GetComponent<Health>();
        playerHealth.onHealthChanged += OnHealthChanged;

        gameManager = GameManager.Instance;

        savedAlpha = damageVignette.color.a;
        damageVignette.color = new Color(damageVignette.color.a, damageVignette.color.g, damageVignette.color.b, 0);

        if (gameManager != null)
            lastDivisible = ((int)(gameManager.GetCurrentTime() - 1f) / 30);

        fontSize = currentTimeText.fontSize;

    }

    private void OnHealthChanged(float newAmount, float oldAmount)
    {
        if (newAmount - oldAmount < 0)
        {
            currentApearTime = apearTime;
        }

        if (newAmount <= 0)
        {
            playerDied = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // ammoText.text = "REMOVED MECHANIC";
        healthBarFill.fillAmount = playerHealth.GetHealthNormalized();

        if (playerDied) { return; }

        if (GameManager.Instance != null)
        {
            if (gameManager == null) gameManager = GameManager.Instance;


            if (!GameManager.Instance.IsTimerHidden())
                currentTimeText.text = ((int)gameManager.GetCurrentTime() / 60).ToString() + ":" + ((float)gameManager.GetCurrentTime() % 60f).ToString("F2");
            else
                currentTimeText.text = "";


            if (lastDivisible != ((int)gameManager.GetCurrentTime() / 30))
            {
                lastDivisible = ((int)gameManager.GetCurrentTime() / 30);
                if (lastDivisible > 1)
                    StartCoroutine(FlashTimer());
                else
                    StartCoroutine(KeepFlashing());
            }
        }

        if (currentApearTime > 0) currentApearTime -= Time.deltaTime;
        damageVignette.color = new Color(damageVignette.color.a, damageVignette.color.g, damageVignette.color.b, Mathf.Lerp(0, savedAlpha, currentApearTime / apearTime));

        weaponChargeBarFill.fillAmount = playerCombat.GetChargeAmount();

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

    private void UpdateScrapCounters() // Peak programming
    {
        int targetInvScrap = ScrapManager.Instance.currentInventoryScrap;
        if (curHeldScrapNum < targetInvScrap) { curHeldScrapNum++; }
        else if (curHeldScrapNum > targetInvScrap) { curHeldScrapNum--; }

        int targetDepoScrap = ScrapManager.Instance.currentDepositedScrap;
        if (curDepoScrapNum < targetDepoScrap) { curDepoScrapNum++; }
        else if (curDepoScrapNum > targetDepoScrap) { curDepoScrapNum--; }

        heldScrapText.text = curHeldScrapNum.ToString().PadLeft(3, '0');
        depositedScrapText.text = curDepoScrapNum.ToString().PadLeft(3, '0');
    }
}
