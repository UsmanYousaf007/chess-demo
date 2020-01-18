using JetBrains.Annotations;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace HUF.Utils.BuildSupport.Editor.iOS
{
    [UsedImplicitly]
    public class iOSMissingPurposeStringFixer : iOSPlistBaseFixer
    {
        const string MISSING_PURPOSE_CONFIG = "Missing PurposeStringConfig config. If app is not deployed on iOS or " +
                                              "is not using sensitive data ignore this message. Otherwise refer to " +
                                              "https://developer.apple.com/documentation/uikit/core_app/protecting_the_user_s_privacy";

        public override int callbackOrder => 16;

        protected override bool Process(PlistElementDict rootDict, string projectPath)
        {
            var config = Resources.Load<PurposeStringConfig>("PurposeStringConfig");
            if (config == null)
            {
                Debug.LogWarning(MISSING_PURPOSE_CONFIG);
                return false;
            }

            foreach (var purposeStringData in config.PurposeStringData)
            {
                rootDict.SetString(purposeStringData.PurposeName, purposeStringData.PurposeReason);
                Debug.LogFormat("Written {0} into key {1}", purposeStringData.PurposeReason,
                    purposeStringData.PurposeName);
            }

            return true;
        }
    }
}