#if UNITY_IOS
using System.IO;
using HUF.AnalyticsAppsFlyer.Runtime.API;
using HUF.Utils.BuildSupport.Editor.iOS;
using HUF.Utils.Runtime.Logging;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public class AppsFlyerPostprocessBuild : iOSPlistBaseFixer
{
    const string APPS_FLYER_SHOULD_SWIZZLE = "AppsFlyerShouldSwizzle";

    readonly HLogPrefix logPrefix = new HLogPrefix( HAnalyticsAppsFlyer.logPrefix, nameof(AppsFlyerPostprocessBuild) );
    public override int callbackOrder => 10;

    protected override bool Process( PlistElementDict rootDict, string projectPath )
    {
        rootDict.SetBoolean( APPS_FLYER_SHOULD_SWIZZLE, true );
        HLog.Log( logPrefix, "Info.plist updated with AppsFlyerShouldSwizzle" );
        return true;
    }
}
#endif