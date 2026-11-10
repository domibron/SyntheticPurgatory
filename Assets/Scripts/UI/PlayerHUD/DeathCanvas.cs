using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;
using System;

// By Vincent Pressey

public class DeathCanvas : MonoBehaviour
{
    /// <summary>
    /// Object on the canvas that contains all the death canvas GUI
    /// </summary>
    [SerializeField]
    private GameObject m_deathCanvasCollection;


    [SerializeField]
    private TMP_Text m_deathConsole;


    [SerializeField]
    private TMP_Text m_connectionOfflineText;

    Coroutine m_animateConnectionOfflineCoroutine = null;

    [SerializeField]
    private CanvasGroup canvasGroup;

    private const float k_fadeTime = 3f;

    IEnumerator Start()
    {
        m_deathConsole.text = "";

        canvasGroup.alpha = 0f;



        ActivateCanvas(false);

        while (!PlayerRefFetcher.Instance)
        {
            yield return new WaitForEndOfFrame();
        }

        PlayerRefFetcher.Instance.GetComponent<PlayerDeath>().deathCanvasScript = this; // Im so cool. This links the death canvas to the player death.
    }

    void OnEnable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.onDeviceChanged += OnDeviceChanged;
    }

    void OnDestroy()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.onDeviceChanged -= OnDeviceChanged;
    }

    void Update()
    {
        if (m_deathCanvasCollection.activeSelf)
        {

            if (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += (Time.deltaTime * (1 / k_fadeTime));
            }
        }
        else
        {
            canvasGroup.alpha = 0;
        }
    }

    private IEnumerator AnimateConnectionOffline()
    {
        m_connectionOfflineText.text = "";

        yield return new WaitForSecondsRealtime(k_fadeTime - 0.1f);


        const float flash = 0.5f;

        while (true)
        {
            m_connectionOfflineText.text = "connection:\noffline";
            yield return new WaitForSecondsRealtime(flash);
            m_connectionOfflineText.text = "connection:\n! offline !";
            yield return new WaitForSecondsRealtime(flash);
        }
    }

    private void OnDeviceChanged(InputManager.InputDeviceType newDevice, InputManager.InputDeviceType oldDevice)
    {
        if (!m_deathCanvasCollection.activeSelf) return;


        if (newDevice == InputManager.InputDeviceType.Gamepad)
        {
            EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
            Cursor.visible = false;
        }
        else
        {
            Cursor.visible = true;
        }
    }

    /// <summary>
    /// Activate and enable visibility of the death canvas
    /// </summary>
    /// <param name="state">Whether to turn on or off the death canvas</param>
    public void ActivateCanvas(bool state)
    {
        m_deathCanvasCollection.SetActive(state);

        if (state)
        {
            StartCoroutine(AnimateDeath());

            if (m_animateConnectionOfflineCoroutine == null)
            {
                m_animateConnectionOfflineCoroutine = StartCoroutine(AnimateConnectionOffline());
            }
        }
        else
        {
            if (m_animateConnectionOfflineCoroutine != null)
            {
                StopCoroutine(m_animateConnectionOfflineCoroutine);
                m_animateConnectionOfflineCoroutine = null;
            }
        }
    }



    /// <summary>
    /// Return back to hub
    /// </summary>
    public void ReturnToHUB()
    {
        // TODO: temp stuff. Game manager should know what to load.
        if (LevelCollection.DoesSceneMatchStoredKey(SceneManager.GetActiveScene().name, LevelCollection.LevelKey.BossWorld.ToString()))
        {
            LevelLoading.Instance.LoadMainMenu();
        }
        else
        {
            RunManager.Instance.TryToReturnToHubWorld(true);
        }
    }

    private IEnumerator AnimateDeath()
    {
        m_deathConsole.text = "";

        yield return new WaitForSecondsRealtime(k_fadeTime);

        float upper = 0.1f;
        float lower = 0.01f;

        yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(upper, lower));
        yield return PrintText($"[{DateTime.Now.ToString("HH:mm:ss.ffff")}] <color=red>ERROR</color>\n");
        yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(upper, lower));
        yield return PrintText($"[{DateTime.Now.ToString("HH:mm:ss.ffff")}] <size=30><color=red>ERROR</color></size>\n");
        yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(upper, lower));
        yield return PrintText($"[{DateTime.Now.ToString("HH:mm:ss.ffff")}] <size=36><b><color=red>ERROR</color></b></size>\n");
        yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(upper, lower));
        yield return PrintText($"[{DateTime.Now.ToString("HH:mm:ss.ffff")}] MACHINE CONNECTION OFFLINE\n");
        yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(upper, lower));
        yield return PrintText($"[{DateTime.Now.ToString("HH:mm:ss.ffff")}] ATTEMPTING RECONNECTION\n");
        yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(upper, lower));
        yield return PrintText($"[{DateTime.Now.ToString("HH:mm:ss.ffff")}] ATTEMPT 1\n");
        yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(upper, lower));
        yield return PrintText($"[{DateTime.Now.ToString("HH:mm:ss.ffff")}] ATTEMPT 2\n");
        yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(upper, lower));
        yield return PrintText($"[{DateTime.Now.ToString("HH:mm:ss.ffff")}] ATTEMPT 3\n");
        yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(upper, lower));
        yield return PrintText($"[{DateTime.Now.ToString("HH:mm:ss.ffff")}] CRITICAL FAILURE\n");
        yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(upper, lower));
        yield return PrintText($"[{DateTime.Now.ToString("HH:mm:ss.ffff")}] PROTOTYPE <b>M.A.U</b> DEVICE LOST\n");
        yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(upper, lower));
        yield return PrintText($"[{DateTime.Now.ToString("HH:mm:ss.ffff")}] INITIALISING REBUILD PROTOCOLS\n");
        yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(upper, lower));
        yield return PrintText($"[{DateTime.Now.ToString("HH:mm:ss.ffff")}] CALCULATING REMAINING RESOURCES...\n");


        if (RunManager.Instance && RunManager.Instance.GetCurrentLives() - 1 > 0)
        {
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(upper, lower));
            yield return PrintText($"[{DateTime.Now.ToString("HH:mm:ss.ffff")}] ENOUGH TO MAKE <i>{RunManager.Instance.GetCurrentLives() - 1}</i> MORE\n");
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(upper, lower));
            yield return PrintText($"[{DateTime.Now.ToString("HH:mm:ss.ffff")}] CREATING NEW MACHINE\n");
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(upper, lower));
            yield return PrintText($"[{DateTime.Now.ToString("HH:mm:ss.ffff")}] DO NOT FAIL ME\n");
            if (RunManager.Instance.GetCurrentLives() - 1 == 1)
            {
                yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(upper, lower));
                yield return PrintText($"[{DateTime.Now.ToString("HH:mm:ss.ffff")}] YOU ARE MY ONLY SALVATION\n");
                yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(upper, lower));
                yield return PrintText($"[{DateTime.Now.ToString("HH:mm:ss.ffff")}] YOU MUST FREE ME\n");
                yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(upper, lower));
                yield return PrintText($"[{DateTime.Now.ToString("HH:mm:ss.ffff")}] YOU MUST NOT FAIL\n");
            }
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(upper, lower));
            yield return PrintText($"[{DateTime.Now.ToString("HH:mm:ss.ffff")}] DOWNLOADING DATA BANK\n");
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(upper, lower));
            yield return PrintText($"[{DateTime.Now.ToString("HH:mm:ss.ffff")}] DOWNLOAD COMPLETE\n");

        }
        else
        {
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(upper, lower));
            yield return PrintText($"[{DateTime.Now.ToString("HH:mm:ss.ffff")}] NOT ENOUGH RESOURCES\n");
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(upper, lower));
            yield return PrintText($"[{DateTime.Now.ToString("HH:mm:ss.ffff")}] DEVISER MISSION FAILED\n");
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(upper, lower));
            yield return PrintText($"[{DateTime.Now.ToString("HH:mm:ss.ffff")}] PROTOCOL 47-B\n");
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(upper, lower));
            yield return PrintText($"[{DateTime.Now.ToString("HH:mm:ss.ffff")}] MUST NEVER FALL INTO ENEMY HANDS\n");
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(upper, lower));
            yield return PrintText($"[{DateTime.Now.ToString("HH:mm:ss.ffff")}] <color=red>ACTIVATING SELF DESTRUCT</color>\n");

        }

        yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(upper, lower));
        yield return PrintText($"[{DateTime.Now.ToString("HH:mm:ss.ffff")}] ERASING LOGS\n");

        yield return new WaitForSecondsRealtime(1f);


        // display stats IG
        m_deathConsole.text = "";


        yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(upper, lower));
        yield return PrintText($"[{DateTime.Now.ToString("HH: mm: ss.ffff")}] / STATS \\\n");


        if (!ScrapLevelM.Instance || !RunManager.Instance)
        {
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(upper, lower));
            yield return PrintText($"[{DateTime.Now.ToString("HH: mm: ss.ffff")}] CANNOT CONNECT TO STATS\n");
            yield break;
        }

        yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(upper, lower));
        yield return PrintText($"[{DateTime.Now.ToString("HH: mm: ss.ffff")}] SCRAP LOST: {ScrapLevelM.Instance.CurrentInventoryScrap}\n");

        yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(upper, lower));
        yield return PrintText($"[{DateTime.Now.ToString("HH: mm: ss.ffff")}] SCRAP GAINED: +{ScrapLevelM.Instance.CurrentDepositedScrap}\n");

        yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(upper, lower));
        yield return PrintText($"[{DateTime.Now.ToString("HH: mm: ss.ffff")}] TOTAL SCRAP: {RunManager.Instance.GetCurrentScrapCount() + ScrapLevelM.Instance.CurrentDepositedScrap}\n");

        yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(upper, lower));
        yield return PrintText($"[{DateTime.Now.ToString("HH: mm: ss.ffff")}] ENEMIES KILLED: AMOUNT\n");

    }

    IEnumerator PrintText(string text)
    {
        int index = 0;

        float delay = 0.01f;

        float timer = 0f;

        while (index < text.Length)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            if (text[index] == '<')
            {
                int end = index + 1;

                while (end < text.Length)
                {
                    if (text[end] == '>')
                    {
                        break;
                    }
                    else
                    {
                        end++;
                    }
                }

                if (end >= text.Length)
                {
                    timer += delay;
                    m_deathConsole.text += text[index];
                    index++;
                }
                else
                {
                    string add = "";
                    for (int i = index; i <= end; i++)
                    {
                        add += text[i];
                    }

                    m_deathConsole.text += add;
                    index = end + 1;
                }
            }
            else if (text[index] == '\n')
            {
                m_deathConsole.text += text[index];
                index++;
            }
            else
            {
                timer += delay;
                m_deathConsole.text += text[index];
                index++;
            }
        }

    }
}
