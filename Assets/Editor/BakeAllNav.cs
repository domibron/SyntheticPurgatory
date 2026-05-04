using Unity.AI.Navigation;
using UnityEngine;

# if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(BakeAllNav))]
public class BakeAllNavGUI : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BakeAllNav bakeAllNav = (BakeAllNav)target;


        if (GUILayout.Button("BakeAll"))
        {
            NavMeshSurface[] navmeshes = (target as GameObject).GetComponents<NavMeshSurface>();

            foreach (var nav in navmeshes)
            {
                nav.BuildNavMesh();
            }
        }
    }
}

#endif
public class BakeAllNav : MonoBehaviour
{

    public void BakeAll()
    {
        NavMeshSurface[] navmeshes = GetComponents<NavMeshSurface>();

        foreach (var nav in navmeshes)
        {
            nav.BuildNavMesh();
        }
    }
}
