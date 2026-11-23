using UnityEngine;

public class SendActionCompletionSignal : MonoBehaviour
{
    [SerializeField]
    private int m_id;

    private bool m_completed = false;

    private SimpleDialogueSystem m_dialogueSystem;

    void Awake()
    {
        m_dialogueSystem = FindAnyObjectByType<SimpleDialogueSystem>();
    }

    void Update()
    {
        if (!m_dialogueSystem) return;

        if (m_completed && m_dialogueSystem.CurrentDialogueID == m_id)
        {
            m_dialogueSystem.OnActionCompleted(m_id);
        }
    }

    public void CompleteAction()
    {
        m_completed = true;
    }
}
