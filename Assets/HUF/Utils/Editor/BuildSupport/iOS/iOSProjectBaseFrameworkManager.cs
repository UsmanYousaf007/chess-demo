#if UNITY_IOS
using System.Collections.Generic;
using UnityEditor.iOS.Xcode;

namespace HUF.Utils.BuildSupport.Editor.iOS
{
    public abstract class iOSProjectBaseFrameworkManager : iOSProjectBaseFixer
    {
        protected abstract IEnumerable<string> FrameworksToAdd { get; }
#if UNITY_2019_3_OR_NEWER
        protected virtual IEnumerable<string> FrameworksToAddToMainTarget { get; } = new string[0];
#endif

        protected override bool Process(PBXProject project)
        {
#if UNITY_2019_3_OR_NEWER
            var mainTargetGuid = project.GetUnityMainTargetGuid();
            foreach (var framework in FrameworksToAddToMainTarget)
            {
                TryAddFramework(project, mainTargetGuid, framework);
            }
            var targetGuid = project.GetUnityFrameworkTargetGuid();
#else
            string targetName = PBXProject.GetUnityTargetName();
            var targetGuid = project.TargetGuidByName(targetName);
#endif
            foreach (var framework in FrameworksToAdd)
            {
                TryAddFramework(project, targetGuid, framework);
            }

            return true;
        }

        static void TryAddFramework(PBXProject project, string targetGuid, string framework)
        {
            if(!project.ContainsFramework(targetGuid, framework))
                project.AddFrameworkToProject(targetGuid, framework, false);
        }
    }
}
#endif