#if UNITY_IOS
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;

namespace HUF.Utils.Wrappers.BuildSupport.Editor
{
    public abstract class iOSDebugBuildFixer
    {
#if HUF_DEBUG_BUILD
        [PostProcessBuild(30)]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            string projectPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
            var projectInString = File.ReadAllText(projectPath);
            projectInString = projectInString.Replace(
                "\"CODE_SIGN_IDENTITY[sdk=iphoneos*]\" = \"iPhone Distribution\";",
                "\"CODE_SIGN_IDENTITY[sdk=iphoneos*]\" = \"iPhone Developer\";");
			
            projectInString = projectInString.Replace(
                "\"CODE_SIGN_IDENTITY\" = \"iPhone Distribution\";",
                "\"CODE_SIGN_IDENTITY\" = \"Apple Development\";");
			
            projectInString = projectInString.Replace(
                "CODE_SIGN_STYLE = Manual;","CODE_SIGN_STYLE = Automatic;");
			
            projectInString =
 projectInString.Replace("ProvisioningStyle = Manual;", $"DevelopmentTeam = {PlayerSettings.iOS.appleDeveloperTeamID};\nProvisioningStyle = Automatic;");
			
            File.WriteAllText(projectPath, projectInString);
	
        }
#endif

#if HUF_DEBUG_BUILD
        [MenuItem("HUF/Misc/Disable HUF Debug Build")]
        static void EnablePlayerPrefs()
        {
            var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup ( EditorUserBuildSettings.selectedBuildTargetGroup );
            var allDefines = definesString.Split ( ';' ).ToList();
            if (allDefines.Contains("HUF_DEBUG_BUILD"))
            {
                allDefines.Remove("HUF_DEBUG_BUILD");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(
                    EditorUserBuildSettings.selectedBuildTargetGroup,
                    string.Join(";", allDefines.ToArray()));
            }
        }
#else
        [MenuItem("HUF/Misc/Enable HUF Debug Build")]
        static void EnablePlayerPrefs()
        {
            var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup ( EditorUserBuildSettings.selectedBuildTargetGroup );
            var allDefines = definesString.Split ( ';' ).ToList();
            if (!allDefines.Contains("HUF_DEBUG_BUILD"))
            {
                allDefines.Add("HUF_DEBUG_BUILD");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(
                    EditorUserBuildSettings.selectedBuildTargetGroup,
                    string.Join(";", allDefines.ToArray()));
            }
        }
#endif
    }
}
#endif