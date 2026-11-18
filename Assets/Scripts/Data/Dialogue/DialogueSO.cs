using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Dialogue", menuName = "ScriptableObjects/Dialogue/DialogueSO")]
public class DialogueSO : ScriptableObject
{
    public DialogueData[] Data = new DialogueData[0];
}

[Serializable]
public struct DialogueData
{
    [TextArea]
    public string Text;
    public Sprite Icon;
}
