# if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(LevelGenerator)), CanEditMultipleObjects]
public class ForceGenerationButton : Editor
{
    public override void OnInspectorGUI()
    {
        LevelGenerator levelGenerator = (LevelGenerator)target;
        GUILayout.Label("DO NOT click this on the dungeon level the level generator will not remove existing rooms!");
        if (GUILayout.Button("Generate Random Level"))
        {
            levelGenerator.SetupRoomGeneration();
            levelGenerator.StartSequence();
        }

        if (GUILayout.Button("Remove all rooms"))
        {
            levelGenerator.AbortSequence();
            levelGenerator.DestroyAllRooms();
        }

        DrawDefaultInspector();
    }
}

[CustomEditor(typeof(Sequencer)), CanEditMultipleObjects]
public class ForceSequenceButton : Editor
{
    public override void OnInspectorGUI()
    {
        Sequencer sequencer = (Sequencer)target;
        GUILayout.Label("DO NOT click this on the dungeon level!");
        if (GUILayout.Button("Generate Random Level"))
        {
            sequencer.StartTheSequence();
        }
        if (GUILayout.Button("ABORT"))
        {
            sequencer.RESETSEQ();
        }

        DrawDefaultInspector();
    }
}

#endif