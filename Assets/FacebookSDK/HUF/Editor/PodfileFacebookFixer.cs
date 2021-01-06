#if UNITY_IOS && !HUF_INIT_FIREBASE
using System.IO;
using System.Linq;
using HUF.Utils.Editor.BuildSupport;
using HUF.Utils.Runtime.Logging;
using UnityEditor;
using UnityEditor.Callbacks;

namespace FacebookSDK.HUF.Editor
{
    public static class PodfileFacebookFixer
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix(nameof(PodfileFacebookFixer));
        const string PATTERN_TO_FIND = "platform :ios";
        const string LINE_TO_ADD = "use_frameworks!";

        [PostProcessBuild(PostProcessBuildNumbers.PODFILE_GENERATION + 1)]
        static void PostProcessIOSBuild(BuildTarget target, string buildPath)
        {
            HLog.Log(logPrefix, "Start");
            string podfilePath = Path.Combine(buildPath, "Podfile");

            if (!File.Exists(podfilePath))
            {
                HLog.LogError(logPrefix, "Cannot find Podfile file!");
                return;
            }


            var textLines = File.ReadAllLines(podfilePath).ToList();
            string el = textLines.First(s => s.Contains(PATTERN_TO_FIND));

            if (el == null)
            {
                HLog.LogError(logPrefix, "Cannot find pattern in Podfile!");
                return;
            }

            int index = textLines.IndexOf(el);
            textLines.Insert(index+1,LINE_TO_ADD);
            File.WriteAllLines(podfilePath,textLines);
            HLog.Log(logPrefix, "Success!");
        }
    }
}
#endif
