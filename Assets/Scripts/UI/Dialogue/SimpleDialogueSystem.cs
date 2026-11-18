using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SimpleDialogueSystem : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_dialogueTextBox;

    [SerializeField]
    private float m_defaultTypeSpeed = 0.1f;

    [SerializeField]
    private Image m_iconImage;

    [SerializeField]
    private DialogueSO m_dialogueSO;

    private Coroutine m_currentDisplayingText;

    // private const char k_startKey = '{';
    // private const char k_endKey = '}';

    // private const char k_pauseTiming = 'p';
    // private const char k_characterTiming = 't';
    // private const char k_clearKey = 'c';

    void Awake()
    {
        // InputSystem.actions.FindAction("Jump").IsPressed();
    }

    void Start()
    {
        StartCoroutine(DisplayText("AEIJKOBGUOGBAEPUIOG N)UIP{GH A(ER GNAEUIOG JAE{UIOFG H AEUIOG HAERUIOGAHNG UON GUOPASHRGUOPAHE UOAEHGUOPAEHGAEUOP GHEUOA:IGG})}"));
    }

    private IEnumerator DisplayDialogue()
    {
        int currentIndex = 0;

        // TODO: imp

        yield return null;
    }

    private void SetSprite(Sprite sprite)
    {
        m_iconImage.sprite = sprite;
    }

    private IEnumerator DisplayText(string dialogue)
    {
        m_dialogueTextBox.maxVisibleCharacters = 0;
        m_dialogueTextBox.text = dialogue;
        m_dialogueTextBox.ForceMeshUpdate();

        int currentIndex = 0;

        float timer = 0f;

        while (currentIndex <= m_dialogueTextBox.textInfo.characterCount)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
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
}
