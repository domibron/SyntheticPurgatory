using System.Linq;
using UnityEditor;
using UnityEngine;

public class BuildProject : MonoBehaviour
{
    private static string[] EnabledLevels()
    {
        return (from scene in EditorBuildSettings.scenes where scene.enabled select scene.path).ToArray();
    }

    [MenuItem("Build/Windows Development Build")]
    public static void BuildForWindowsDev()
    {
        BuildPipeline.BuildPlayer(EnabledLevels(), "Build/Game.exe", BuildTarget.StandaloneWindows, BuildOptions.Development);
    }
}
