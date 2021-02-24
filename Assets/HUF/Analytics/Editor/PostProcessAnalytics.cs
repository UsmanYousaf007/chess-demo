#if UNITY_IOS
using HUF.Utils.BuildSupport.Editor.iOS;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEditor.iOS.Xcode;

namespace HUF.Analytics.Editor
{
    [UsedImplicitly]
    public class PostProcessAnalytics : iOSPlistBaseFixer
    {
        const string PLIST_ATT_DESC_KEY = "NSUserTrackingUsageDescription";
        const string PLIST_ATT_DESC_VALUE = "Your data will be used to deliver personalised ads to you.";

        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(PostProcessAnalytics) );

        public override int callbackOrder => 999;

        protected override bool Process( PlistElementDict rootDict, string projectPath )
        {
            if ( !rootDict.values.ContainsKey( PLIST_ATT_DESC_KEY ) )
            {
                rootDict.SetString( PLIST_ATT_DESC_KEY, PLIST_ATT_DESC_VALUE );
                HLog.Log( logPrefix, $"Added {PLIST_ATT_DESC_KEY} key to PList" );
            }

            return true;
        }
    }
}
#endif