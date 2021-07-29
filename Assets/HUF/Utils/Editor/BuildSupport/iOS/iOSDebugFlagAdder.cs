#if UNITY_IOS
using HUF.Utils.BuildSupport.Editor.iOS;
using UnityEditor.iOS.Xcode;

namespace HUF.Utils.Editor.BuildSupport.iOS
{
    public class iOSDebugFlagAdder : iOSProjectBaseFixer
    {
        public override int callbackOrder { get; }

        const string DEBUG_CONFIGURATION = "Debug";
        const string FLAGS_PROPERTY = "OTHER_CFLAGS";
        const string DEBUG_FLAG = "DEBUG";
        const string DEBUG_FLAG_DEFINE = "-D" + DEBUG_FLAG;

        protected override bool Process( PBXProject project )
        {
#if UNITY_2019_1_OR_NEWER
            string target = project.GetUnityMainTargetGuid();
#else
            string target = project.TargetGuidByName( PBXProject.GetUnityTargetName() );
#endif
            var config = project.BuildConfigByName( target, DEBUG_CONFIGURATION );
            var property = project.GetBuildPropertyForConfig( config, FLAGS_PROPERTY );

            if ( property.Contains( DEBUG_FLAG_DEFINE ) )
                return false;

            property += $" {DEBUG_FLAG_DEFINE}";
            project.SetBuildPropertyForConfig( config, FLAGS_PROPERTY, property );
            return true;
        }
    }
}
#endif
