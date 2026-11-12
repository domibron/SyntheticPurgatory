using TMPro;
using UnityEngine;

public class Tips : MonoBehaviour
{
    [SerializeField]
    private TipsDataSO m_tipsData;

    [SerializeField]
    private TMP_Text m_textBox;

    [SerializeField]
    private float m_secondsPerTip = 4f;

    private float m_currentTipTime = 0f;

    private string m_prefix = "<b>TIP:</b> ";

    private int m_index = 0;

    void OnEnable()
    {
        m_currentTipTime = m_secondsPerTip;

        m_index = GetNextIndex();

        m_textBox.text = GetTip(m_index);
    }

    void Awake()
    {
        m_currentTipTime = m_secondsPerTip;

        m_index = GetNextIndex();

        m_textBox.text = GetTip(m_index);
    }

    void Update()
    {
        if (m_currentTipTime > 0)
        {
            m_currentTipTime -= Time.deltaTime;
        }
        else
        {
            m_currentTipTime = m_secondsPerTip;

            m_index = GetNextIndex();

            m_textBox.text = GetTip(m_index);
        }
    }

    private int GetNextIndex()
    {
        return UnityEngine.Random.Range(0, m_tipsData.Tips.Length);
    }

    private string GetTip(int index)
    {
        if (index < 0 || index >= m_tipsData.Tips.Length) return m_prefix + "Huh, no tip right now";

        return m_prefix + m_tipsData.Tips[index];
    }
}
