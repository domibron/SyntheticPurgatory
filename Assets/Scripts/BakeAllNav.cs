using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BakeAllNav))]
public class BakeAllNavGUI : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BakeAllNav bakeAllNav = (BakeAllNav)target;


        if (GUILayout.Button("BakeAll"))
        {
            NavMeshSurface[] navmeshes = target.GetComponents<NavMeshSurface>();

            foreach (var nav in navmeshes)
            {
                nav.BuildNavMesh();
            }
        }
    }
}

public class BakeAllNav : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
