using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Dialogue", menuName = "ScriptableObjects/Dialogue/DialogueSO")]
public class DialogueSO : ScriptableObject
{
    public DialogueData[] Data = new DialogueData[0];
}

public enum DialogueNext
{
    /// <summary>
    /// Do nothing.
    /// </summary>
    ResumeGame,

    /// <summary>
    /// Resume the game and wait for a specific action.
    /// </summary>
    ResumeWaitForAction,

    /// <summary>
    /// Pause the game for this dialogue.
    /// </summary>
    PauseGame,

    /// <summary>
    /// Pause the game for this dialogue.
    /// </summary>
    ResumeWaitForActionNoDialogueBox,
}

[Serializable]
public struct DialogueData
{
    [TextArea]
    public string Dialogue;
    public string Name;
    public DialogueNext DialogueNext;

    public DialogueData(string name = "", string dialogue = "", DialogueNext dNext = DialogueNext.ResumeGame)
    {
        Dialogue = dialogue;
        Name = name;
        DialogueNext = dNext;
    }
}

