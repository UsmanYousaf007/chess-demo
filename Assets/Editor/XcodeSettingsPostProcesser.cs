using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEditor.Callbacks;
using System.Collections;

public class XcodeSettingsPostProcesser
{
    [PostProcessBuildAttribute (1)]
    public static void OnPostprocessBuild (BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget != BuildTarget.iOS)
        {
            return;
        }

        TurboLabz.TLUtils.LogUtil.Log("POST PROCESS XCODE", "cyan");

        string projPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";

        PBXProject proj = new PBXProject();
        proj.ReadFromString(File.ReadAllText(projPath));

        string target = proj.TargetGuidByName("Unity-iPhone");
        string targetGuid = proj.TargetGuidByName(PBXProject.GetUnityTargetName());

        //Required Frameworks
        proj.AddFrameworkToProject(target, "UserNotifications.framework", false);

        // Additional files
        TurboLabz.TLUtils.LogUtil.Log("Adding ChessAI files..", "cyan");
        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);

        string aiFolder = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/')) + "/ChessAI/";
        //string aiFolder =  desktopPath + "/data/turbo-labz/projects/chess-ai-plugin/iOSChessPlugin/ChessAI/";

        string[] arrayAIFiles = {
            "unity.h",
            "unity.cpp",
            "ucioption.cpp",
            "uci.h",
            "uci.cpp",
            "types.h",
            "tt.h",
            "tt.cpp",
            "timeman.h",
            "timeman.cpp",
            "thread.h",
            "thread.cpp",
            "thread_win32.h",
            "tbprobe.h",
            "tbprobe.cpp",
            "search.h",
            "search.cpp",
            "psqt.cpp",
            "position.h",
            "position.cpp",
            "pawns.h",
            "pawns.cpp",
            "movepick.h",
            "movepick.cpp",
            "movegen.h",
            "movegen.cpp",
            "misc.h",
            "misc.cpp",
            "material.h",
            "material.cpp",
            "main.cpp",
            "evaluate.h",
            "evaluate.cpp",
            "endgame.h",
            "endgame.cpp",
            "bitboard.h",
            "bitboard.cpp",
            "bitbase.cpp",
            "benchmark.cpp"
            };

        foreach (string f in arrayAIFiles)
        {
            string aiFileID = proj.AddFile(aiFolder+f, "/ChessAI/"+f, PBXSourceTree.Sdk);
            TurboLabz.TLUtils.LogUtil.Log("Adding file:" + aiFolder + f + " id=" + aiFileID, "cyan");
            proj.AddFileToBuild(targetGuid, aiFileID);
        }

        proj.AddCapability(targetGuid, PBXCapabilityType.PushNotifications);
        proj.AddCapability(targetGuid, PBXCapabilityType.BackgroundModes);

        //patch v.5.13.14,its for xcode below v11.4
        proj.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
        proj.SetBuildProperty(targetGuid, "CLANG_ENABLE_MODULES", "YES");

        // Get plist
        string plistPath = pathToBuiltProject + "/Info.plist";
        PlistDocument plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(plistPath));

        // Get root
        PlistElementDict rootDict = plist.root;

        // Set encryption usage boolean
        string encryptKey = "ITSAppUsesNonExemptEncryption";
        rootDict.SetBoolean(encryptKey, false);

        // remove exit on suspend if it exists.
        string exitsOnSuspendKey = "UIApplicationExitsOnSuspend";
        if (rootDict.values.ContainsKey(exitsOnSuspendKey))
        {
            rootDict.values.Remove(exitsOnSuspendKey);
        }

        // Change value of CFBundleVersion in Xcode plist
        var buildKey = "UIBackgroundModes";
        rootDict.CreateArray(buildKey).AddString("remote-notification");
        rootDict.SetBoolean("GADIsAdManagerApp", true);
        rootDict.SetString("AppLovinSdkKey", "5c875d6cdbbaa3558555ce2e");

        File.WriteAllText(plistPath, plist.WriteToString());
        File.WriteAllText(projPath, proj.WriteToString());
    }
}
