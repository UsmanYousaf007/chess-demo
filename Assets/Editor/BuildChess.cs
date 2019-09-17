using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System;
using TurboLabz.TLUtils;

public class BuildChess : MonoBehaviour
{ 
    static string BUILD_OUTPUT_PATH = "/build";
    static string BUILD_OUTPUT_ANDROID_SUBPATH = "/Android";
    static string BUILD_OUTPUT_IOS_SUBPATH = "/iOS";

    static string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);

    static string androidAPK = "chessstar";
    static string bundleVersion = PlayerSettings.bundleVersion;
    static string[] gameScenes = new string[] { "Game" };
    static string[] gameScenFiles = new string[] {
            "Assets/InstantFramework/Scenes/Splash.unity",
            "Assets/Game/Scenes/Game.unity"
            };

    static string bundleVersionCodeiOS = PlayerSettings.iOS.buildNumber;
    static string bundleVersionCodeAndroid = PlayerSettings.Android.bundleVersionCode.ToString();

    private static void ProcessArgs()
    {
        LogUtil.Log("Process Args", "yellow");
        string[] args = Environment.GetCommandLineArgs();
        int i = 0;
        foreach (string a in args)
        {
            LogUtil.Log("Args: " + a);

            if (a == "-inBundleVersion")
            {
                bundleVersion = args[i + 1];
                LogUtil.Log("bundleVersion: " + bundleVersion);
            }
            else if (a == "-inBundleVersionCode")
            {
                bundleVersionCodeiOS = args[i + 1];
                bundleVersionCodeAndroid = args[i + 1];
                LogUtil.Log("bundleVersionCode: " + args[i + 1]);
            }
            i++;
        }
    }

    private static void ProcessSkinLinks()
    {
        LogUtil.Log("Process skin links", "yellow");
        Scene scene = EditorSceneManager.GetSceneByName(gameScenes[0]);
        ClearSkinLinks.Init();
        EditorSceneManager.SaveScene(scene);
    }

    private static void ProcessBuild(BuildPlayerOptions buildPlayerOptions)
    {
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            LogUtil.Log("Build succeeded: " + summary.totalSize + " bytes", "cyan");
        }

        if (summary.result == BuildResult.Failed)
        {
            LogUtil.Log("Build failed", "red");
        }
    }

    [MenuItem("Build/Build Chess iOS")]
    public static void BuildiOS()
    {
        LogUtil.Log("Start Build iOS", "yellow");

        ProcessArgs();
        ProcessSkinLinks();

        PlayerSettings.bundleVersion = bundleVersion;
        PlayerSettings.Android.bundleVersionCode = Int32.Parse(bundleVersionCodeiOS);

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = gameScenFiles;  
        buildPlayerOptions.locationPathName = desktopPath + BUILD_OUTPUT_PATH + BUILD_OUTPUT_IOS_SUBPATH;
        buildPlayerOptions.target = BuildTarget.iOS;
        buildPlayerOptions.options = BuildOptions.None;

        ProcessBuild(buildPlayerOptions);

        LogUtil.Log("End Build iOS", "yellow");
    }

    [MenuItem("Build/Build Chess Andriod")]
    public static void BuildAndroid()
    {
        LogUtil.Log("Start Build Android");

        ProcessArgs();
        ProcessSkinLinks();

        PlayerSettings.bundleVersion = bundleVersion;
        PlayerSettings.Android.bundleVersionCode = Int32.Parse(bundleVersionCodeAndroid);

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = gameScenFiles; 
        buildPlayerOptions.locationPathName = desktopPath + BUILD_OUTPUT_PATH + BUILD_OUTPUT_ANDROID_SUBPATH + "/" + androidAPK + bundleVersionCodeAndroid + ".apk";
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.None;

        EditorPrefs.SetString("AndroidSdkRoot", Environment.GetEnvironmentVariable("ANDROID_SDK_ROOT"));

        PlayerSettings.Android.keystoreName = desktopPath + "/data/turbo-labz/projects/keystores/instant-chess/instant-chess-signing.keystore";
        PlayerSettings.Android.keystorePass = "0_turbolabzsignature-instant-chess_1";
        PlayerSettings.Android.keyaliasPass = "0_turbolabzsignature-instant-chess_1";
        PlayerSettings.Android.keyaliasName = "instant-chess-signing";
        PlayerSettings.Android.renderOutsideSafeArea = false;

        ProcessBuild(buildPlayerOptions);

        LogUtil.Log("End Build Android");
    }
}
