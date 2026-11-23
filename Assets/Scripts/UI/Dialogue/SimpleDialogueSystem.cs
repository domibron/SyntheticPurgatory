using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SimpleDialogueSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject m_dialogueUIObj;

    [SerializeField]
    private TMP_Text m_dialogueTextBox;

    [SerializeField]
    private TMP_Text m_namePlateTextBox;

    [SerializeField]
    private float m_defaultTypeSpeed = 0.1f;

    [SerializeField]
    private DialogueSO m_dialogueSO;

    private Coroutine m_currentDisplayingText;
    private Coroutine m_currentDialogue;

    private const char k_startKey = '{';
    private const char k_endKey = '}';
    private const char k_separator = '=';

    [ReadOnly]
    public int CurrentDialogueID { get; private set; } = 0; // Use the SO list index for id l:

    private int m_upcomingIndex = 0;

    private bool m_hasPressed = false;

    private bool m_isNextDialogueRequested = false;

    private bool m_isWaitingForActionComplete = false;

    private string[] m_injectionStrings = new string[0];



    // Its all lines of machine code.
    // Always has been.
    // INSERT MEME IMAGE.


    void Start()
    {
        HideDialogueUI();
        // StartCoroutine(StartFullDialogue(m_dialogueSO));

        StartDialogue();
    }

    void Update()
    {

        if (InputSystem.actions.FindAction("Jump").IsPressed() && !m_hasPressed)
        {
            m_hasPressed = true;

            if (m_currentDisplayingText != null)
            {
                SkipCurrentTextAnimation();
            }
            else if (!m_isWaitingForActionComplete)
            {
                m_isNextDialogueRequested = true;
            }
        }
        else if (!InputSystem.actions.FindAction("Jump").IsPressed() && m_hasPressed)
        {
            m_hasPressed = false;
        }
    }

    public void StartDialogue(DialogueSO data = null)
    {
        if (m_currentDialogue != null)
        {
            StopCoroutine(m_currentDialogue);
        }

        if (data is not null)
        {
            m_currentDialogue = StartCoroutine(StartFullDialogue(data));
        }
        else
        {
            m_currentDialogue = StartCoroutine(StartFullDialogue(m_dialogueSO));
        }
    }

    private IEnumerator StartFullDialogue(DialogueSO dialogue)
    {
        ShowDialogueUI();
        m_dialogueTextBox.text = "";
        m_namePlateTextBox.text = "";

        m_isWaitingForActionComplete = false;

        CurrentDialogueID = 0;
        m_upcomingIndex = 0;

        yield return new WaitForEndOfFrame();

        for (int i = 0; i < dialogue.Data.Length; i++)
        {
            CurrentDialogueID = i;

            DisplayDialogueSegment(dialogue.Data[i]);

            m_upcomingIndex = i + 1;

            switch (dialogue.Data[i].DialogueNext)
            {
                case DialogueNext.ResumeGame:
                    PauseCanvas.CurrentPauseState = PauseCanvas.PauseState.Restore;

                    // TODO: make a less fucked up solution. Like maybe pause time in the bloody pause canvas?
                    if (PauseCanvas.CurrentPauseState == PauseCanvas.PauseState.Resumed)
                        Time.timeScale = 1; // unpause the game. might fail with pause system.

                    while (!m_isNextDialogueRequested)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    break;

                case DialogueNext.ResumeWaitForAction:
                    PauseCanvas.CurrentPauseState = PauseCanvas.PauseState.Restore;

                    if (PauseCanvas.CurrentPauseState == PauseCanvas.PauseState.Resumed)
                        Time.timeScale = 1; // unpause the game. might fail with pause system.

                    m_isWaitingForActionComplete = true;

                    while (m_isWaitingForActionComplete)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    break;

                case DialogueNext.PauseGame:
                    PauseCanvas.CurrentPauseState = PauseCanvas.PauseState.Override;
                    Time.timeScale = 0; // pause the game. might incur a bug with pause system..

                    while (!m_isNextDialogueRequested)
                    {
                        yield return new WaitForEndOfFrame();
                    }

                    PauseCanvas.CurrentPauseState = PauseCanvas.PauseState.Restore;

                    if (PauseCanvas.CurrentPauseState == PauseCanvas.PauseState.Resumed)
                        Time.timeScale = 1; // unpause the game. might fail with pause system.

                    break;

                case DialogueNext.ResumeWaitForActionNoDialogueBox:

                    while (!m_isNextDialogueRequested)
                    {
                        yield return new WaitForEndOfFrame();
                    }

                    HideDialogueUI();

                    PauseCanvas.CurrentPauseState = PauseCanvas.PauseState.Restore;

                    if (PauseCanvas.CurrentPauseState == PauseCanvas.PauseState.Resumed)
                        Time.timeScale = 1; // unpause the game. might fail with pause system.

                    m_isWaitingForActionComplete = true;

                    while (m_isWaitingForActionComplete)
                    {
                        yield return new WaitForEndOfFrame();
                    }

                    ShowDialogueUI();

                    break;
            }

            m_isNextDialogueRequested = false;
        }

        HideDialogueUI();
    }



    public void OnActionCompleted(int id)
    {
        if (CurrentDialogueID == id)
        {
            m_isWaitingForActionComplete = false;
        }
    }

    public void SetInjectionString(int idToInjectOn, string[] strings)
    {
        if (m_upcomingIndex == idToInjectOn)
        {
            m_injectionStrings = strings;
        }
    }



    private void HideDialogueUI()
    {
        m_dialogueUIObj.SetActive(false);
    }

    private void ShowDialogueUI()
    {
        m_dialogueUIObj.SetActive(true);
    }

    private void SkipCurrentTextAnimation()
    {
        if (m_currentDisplayingText != null)
        {
            StopCoroutine(m_currentDisplayingText);
            m_currentDisplayingText = null;
        }

        m_dialogueTextBox.maxVisibleCharacters = m_dialogueTextBox.textInfo.characterCount;
    }

    private bool DisplayDialogueSegment(DialogueData data)
    {
        if (m_currentDisplayingText != null) return false;

        m_namePlateTextBox.text = data.Name;
        m_currentDisplayingText = StartCoroutine(DisplayText(data.Dialogue));
        return true;
    }

    private IEnumerator DisplayText(string dialogue)
    {
        m_dialogueTextBox.maxVisibleCharacters = 0;
        m_dialogueTextBox.text = ParseString(dialogue);
        m_dialogueTextBox.ForceMeshUpdate();

        int currentIndex = 0;

        float timer = 0f;

        while (currentIndex <= m_dialogueTextBox.textInfo.characterCount)
        {
            if (timer > 0)
            {
                timer -= Time.unscaledDeltaTime;
                yield return new WaitForEndOfFrame();
            }

            if (timer <= 0)
            {
                m_dialogueTextBox.maxVisibleCharacters = ++currentIndex;
                timer += m_defaultTypeSpeed;
            }
        }

        m_currentDisplayingText = null;
    }

    private string ParseString(string textToConvert)
    {
        string returnSting = "";

        for (int i = 0; i < textToConvert.Length; i++)
        {
            if (textToConvert[i] != k_startKey)
            {
                returnSting += textToConvert[i];
            }
            else
            {
                string parsedValue = "";
                int skipIndex = i + 1;

                for (int e = i + 1; e < textToConvert.Length; e++)
                {
                    if (textToConvert[e] == k_endKey)
                    {
                        skipIndex = e;
                        break;
                    }

                    parsedValue += textToConvert[e];
                }


                returnSting += ParseCommand(parsedValue.Split(k_separator));
                i = skipIndex;
            }
        }

        return returnSting;
    }

    private string ParseCommand(string[] values)
    {
        if (values.Length <= 0) return "";

        switch (values[0])
        {
            default:
                return "";

            case "Input":
                if (values.Length <= 1)
                {
                    return "NO KEY SPECIFIED!";
                }

                return GetInput(Enum.Parse<InputManager.InputKey>(values[1]),
                    InputManager.Instance == null ?
                        (values.Length > 2 ? Enum.Parse<InputManager.InputDeviceType>(values[2]) : InputManager.InputDeviceType.Keyboard)
                        : InputManager.Instance.GetCurrentInputDevice());

            case "CString":
                int injectionIndex = 0;
                if (values.Length > 1 || int.TryParse(values[1], out injectionIndex))
                {
                    if (m_injectionStrings.Length >= injectionIndex)
                    {
                        return m_injectionStrings[injectionIndex];
                    }

                    return "NO VALID STRING AT INDEX!";
                }

                return "NO INDEX SPECIFIED!";
        }
    }

    private string GetInput(InputManager.InputKey inputKey, InputManager.InputDeviceType inputDevice)
    {
        string str = InputSystem.actions.FindAction(inputKey.ToString()).GetBindingDisplayString((int)inputDevice, InputBinding.DisplayStringOptions.DontUseShortDisplayNames);

        if (str == "Delta") // Unity having the best input naming. :smile:
        {
            return "Mouse";
        }

        return str;
    }
}
