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
    [MenuItem("Build/Build Chess iOS")]
    public static void BuildiOS()
    {
        Debug.Log("Start Build iOS");

        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        String[] arguments = Environment.GetCommandLineArgs();

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

    [MenuItem("Build/Build Chess Andriod")]
    public static void BuildAndroid()
    {
        Debug.Log("Start Build Android");

        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        String[] arguments = Environment.GetCommandLineArgs();

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
        buildPlayerOptions.locationPathName = desktopPath + "/build/chessstar.apk";
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.None;

        EditorPrefs.SetString("AndroidSdkRoot", Environment.GetEnvironmentVariable("ANDROID_SDK_ROOT"));

        PlayerSettings.Android.keystoreName = desktopPath + "/data/turbo-labz/projects/keystores/instant-chess/instant-chess-signing.keystore";
        PlayerSettings.Android.keystorePass = "0_turbolabzsignature-instant-chess_1";
        PlayerSettings.Android.keyaliasPass = "0_turbolabzsignature-instant-chess_1";
        PlayerSettings.Android.keyaliasName = "instant-chess-signing";


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

        Debug.Log("End Build Android");

    }
}
