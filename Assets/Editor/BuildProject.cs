using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
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
        BuildReport buildReport = BuildPipeline.BuildPlayer(EnabledLevels(), "Build/Game.exe", BuildTarget.StandaloneWindows, BuildOptions.Development);
        BuildSummary buildSummary = buildReport.summary;

        Debug.Log(buildSummary.result.ToString());

    }
}
