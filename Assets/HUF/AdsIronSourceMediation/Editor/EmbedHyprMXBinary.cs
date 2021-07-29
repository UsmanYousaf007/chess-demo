using System.IO;
using HUF.AdsIronSourceMediation.Runtime.API;
using HUF.Utils.Runtime.Logging;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_EDITOR_OSX
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
#endif

namespace HUF.AdsIronSourceMediation.Editor
{
    public static class PostProcessHyprMX
    {
        const string HYPR_MX_PODS_DIRECTORY = "Pods/HyprMX/";
        const string CORE_HYPR_MX_FRAMEWORK_NAME = "HyprMX.xcframework";
        const string HYPR_PERMISSIONS_FRAMEWORK_NAME = "HYPRPermissions.xcframework";

        [PostProcessBuild( 101 )]
        static void PostProcessBuild_HyprMX( BuildTarget target, string buildPath )
        {
#if UNITY_EDITOR_OSX
            if ( target == BuildTarget.iOS )
            {
                string projPath = PBXProject.GetPBXProjectPath( buildPath );
                PBXProject proj = new PBXProject();
                proj.ReadFromString( File.ReadAllText( projPath ) );
#if UNITY_2019_3_OR_NEWER
                // HyprMX is installed/linked by CocoaPods to the UnityFramework.framework, but the dynamic lib still
                // needs to be embedded into the application target.
                HLog.Log( HAdsIronSourceMediation.logPrefix , "Embedding HyperMX frameworks into Unity-iPhone target..." );
                string targetGuid = proj.GetUnityMainTargetGuid();
                EmbedHyprMXFramework( proj, projPath, targetGuid, CORE_HYPR_MX_FRAMEWORK_NAME );
                EmbedHyprMXFramework( proj, projPath, targetGuid, HYPR_PERMISSIONS_FRAMEWORK_NAME );
#else
            // Unity projects before 2019.3 do not contain runtime search paths needed for dynamic libraries.
            HLog.Log( HAdsIronSourceMediation.logPrefix , "Adding the runtime search path to Unity-iPhone target...");
            string targetGuid = proj.TargetGuidByName("Unity-iPhone");
            proj.SetBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "$(inherited) @executable_path/Frameworks");
#endif
                proj.WriteToFile( projPath );
            }
#endif
        }

#if UNITY_EDITOR_OSX
        static void EmbedHyprMXFramework( PBXProject proj, string projPath, string targetGuid, string framework )
        {
            string hyprMXframeworkPath = HYPR_MX_PODS_DIRECTORY + framework;
            string fileGuid = proj.AddFile( hyprMXframeworkPath, hyprMXframeworkPath, PBXSourceTree.Source );
            PBXProjectExtensions.AddFileToEmbedFrameworks( proj, targetGuid, fileGuid );
        }
#endif
    }
}