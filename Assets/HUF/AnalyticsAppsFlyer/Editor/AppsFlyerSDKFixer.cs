using System;
using System.IO;
using System.Linq;
using HUF.AnalyticsAppsFlyer.Runtime.API;
using HUF.Utils.Runtime.Logging;
using UnityEditor;
using UnityEngine;

namespace HUF.AnalyticsAppsFlyer.Editor
{
    public class AppsFlyerSDKFixer : AssetPostprocessor
    {
        const string CONTROLLER_PATH = "AnalyticsAppsFlyer/SDK/AppsFlyer/Plugins/iOS/AppsFlyerAppController.mm";
        const string IMPL_APP_CONTROLLER_SUBCLASS = nameof(IMPL_APP_CONTROLLER_SUBCLASS);
        const string BAD_STRING = "\n" + IMPL_APP_CONTROLLER_SUBCLASS;
        const string BAD_STRING_REPLACEMENT = "\n//" + IMPL_APP_CONTROLLER_SUBCLASS;

        const string NEW_CONTENT = "\n+(void)load\n{\n[AppsFlyerAppController plugin];\n}\n// Singleton accessor.\n" +
                                   "+ (AppsFlyerAppController *)plugin\n{\nstatic AppsFlyerAppController *sharedInstance = nil;\n" +
                                   "static dispatch_once_t onceToken;\ndispatch_once(&onceToken, ^{\n" +
                                   "sharedInstance = [[AppsFlyerAppController alloc] init];\n});\nreturn sharedInstance;\n}";

        const string AT_SIGN_END = "@end";
        const string ASSETS = "Assets";

        static readonly HLogPrefix logPrefix =
            new HLogPrefix( HAnalyticsAppsFlyer.logPrefix, nameof(AppsFlyerSDKFixer) );

        static void OnPostprocessAllAssets( string[] imported,
            string[] deleted,
            string[] moved,
            string[] movedFromPaths )
        {
            string filePath = imported.FirstOrDefault( asset => asset.Contains( CONTROLLER_PATH ) );
            var file = new FileInfo( Application.dataPath.Replace( ASSETS, "" ) + filePath );

            if ( file.Exists )
            {
                var text = File.ReadAllText( file.FullName );

                if ( text.Contains( BAD_STRING ) )
                {
                    text = text.Replace( BAD_STRING, BAD_STRING_REPLACEMENT );
                    var id = text.LastIndexOf( AT_SIGN_END, StringComparison.InvariantCulture );
                    text = text.Insert( id - 1, NEW_CONTENT );
                    File.WriteAllText( file.FullName, text );
                    HLog.Log( logPrefix, "SDK Fixed: " + file.FullName );
                }
                else
                {
                    HLog.Log( logPrefix, "SDK Fixer skipped" );
                }
            }
        }
    }
}