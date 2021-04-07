#if UNITY_IOS && UNITY_2019_3_OR_NEWER
using HUF.Utils.BuildSupport.Editor.iOS;
using JetBrains.Annotations;
using UnityEditor.iOS.Xcode;
namespace HUF.InitFirebase.Editor
{
    //TODO: Remove after firebase gets updated to >=7.1.0
    [UsedImplicitly]
    public class iOSFirebaseLowerThan7_1Fixer : iOSProjectBaseFixer
    {
        const string SERVICES_FILE = "GoogleService-Info.plist";
        public override int callbackOrder => 0;
        protected override bool Process( PBXProject project )
        {
            string googleInfoPlistGuid = project.FindFileGuidByProjectPath( SERVICES_FILE );
            if ( string.IsNullOrEmpty( googleInfoPlistGuid ) )
                googleInfoPlistGuid = project.AddFile( SERVICES_FILE, SERVICES_FILE );
            project.AddFileToBuild( project.GetUnityMainTargetGuid(), googleInfoPlistGuid );
            return true;
        }
    }
}
#endif
