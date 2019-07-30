using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System;
//using UnityEngine.SceneManagement;

// Output the build size or a failure depending on BuildPlayer.

public class BuildChess : MonoBehaviour
{
    [MenuItem("Build/Build Chess")]

    public static void BuildiOS()
    {
        Debug.Log("Start Build iOS");


        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);

        String[] arguments = Environment.GetCommandLineArgs();

        //EditorSceneManager.OpenScene();
        //Scene scene = SceneManager.GetActiveScene();
        //PlayerSettings.bundleVersion = "999";
        //EditorSceneManager.SaveScene(scene);
        //Scene scene = EditorSceneManager.GetSceneByName("Game");
        //EditorSceneManager.SaveScene(scene);

        // Clear skin links
        Debug.Log("Clear skin links");
        Scene scene = EditorSceneManager.GetSceneByName("Game");
        ClearSkinLinks.Init();
        EditorSceneManager.SaveScene(scene);

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new string[] { 
            "Assets/InstantFramework/Scenes/Splash.unity",
            "Assets/Game/Scenes/Game.unity"
            };
        buildPlayerOptions.locationPathName = desktopPath + "/build/";
        buildPlayerOptions.target = BuildTarget.iOS;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }

        Debug.Log("End Build iOS");

    }
}
