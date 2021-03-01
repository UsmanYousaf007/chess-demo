#if UNITY_IOS
using System.IO;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace HUF.Utils.Editor.BuildSupport.iOS
{
    [UsedImplicitly]
    public class DisablingAlwaysEmbedSwiftStandardLibrariesiOS
    {
        const string ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES = nameof(ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES);

        [PostProcessBuild( PostProcessBuildNumbers.PODFILE_GENERATION - 1 )]
        public static void PostProcessBuildAttribute( BuildTarget target, string pathToBuildProject )
        {
            if ( target == BuildTarget.iOS )
            {
                string projectPath = PBXProject.GetPBXProjectPath( pathToBuildProject );
                var projectInString = File.ReadAllText( projectPath );

                projectInString = projectInString.Replace( $"{ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES} = YES;",
                    $"{ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES} = NO;" );
                File.WriteAllText( projectPath, projectInString );
                var pbxProject = new PBXProject();
                pbxProject.ReadFromFile( projectPath );
#if UNITY_2019_3_OR_NEWER
                pbxProject.SetBuildProperty( pbxProject.GetUnityFrameworkTargetGuid(),
                    "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES",
                    "NO" );
                var targetGuid = pbxProject.GetUnityMainTargetGuid();
#else
                var targetName = PBXProject.GetUnityTargetName();
                var targetGuid = pbxProject.TargetGuidByName( targetName );
#endif
                pbxProject.SetBuildProperty( targetGuid, ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES, "NO" );
                pbxProject.SetBuildProperty( pbxProject.ProjectGuid(), ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES, "YES" );
                pbxProject.WriteToFile( projectPath );
            }
        }
    }
}

#endif