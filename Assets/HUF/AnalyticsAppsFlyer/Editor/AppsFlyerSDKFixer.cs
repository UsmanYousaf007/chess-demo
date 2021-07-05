using System;
using System.IO;
using System.Linq;
using HUF.AnalyticsAppsFlyer.Runtime.API;
using HUF.Utils.Runtime.Extensions;
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

        const string NEW_PLUGIN_CONTENT =
            "\n+(void)load\n{\n[AppsFlyerAppController plugin];\n}\n// Singleton accessor.\n" +
            "+ (AppsFlyerAppController *)plugin\n{\nstatic AppsFlyerAppController *sharedInstance = nil;\n" +
            "static dispatch_once_t onceToken;\ndispatch_once(&onceToken, ^{\n" +
            "sharedInstance = [[AppsFlyerAppController alloc] init];\n});\nreturn sharedInstance;\n}";

        const string IMPORT = "\n#import";
        const string NEW_IMPORT = "#import <AppTrackingTransparency/ATTrackingManager.h>";
        const string DID_FINISH_LAUNCHING_LINE = "- (void)didFinishLaunching:(NSNotification*)notification {";

        const string NEW_DID_FINISH_LAUNCHING_LINE_PART =
            "[ATTrackingManager requestTrackingAuthorizationWithCompletionHandler:";

        const string NEW_DID_FINISH_LAUNCHING_CONTENT = DID_FINISH_LAUNCHING_LINE +
                                                        "\n    if (@available(iOS 14, *)) {\n          [ATTrackingManager requestTrackingAuthorizationWithCompletionHandler:^(ATTrackingManagerAuthorizationStatus status){ }];\n    }";

        const string AT_SIGN_END = "@end";
        const string ASSETS = "Assets";

        static void OnPostprocessAllAssets( string[] imported,
            string[] deleted,
            string[] moved,
            string[] movedFromPaths )
        {
            string filePath = imported.FirstOrDefault( asset => asset.Contains( CONTROLLER_PATH ) );
            var file = new FileInfo( Application.dataPath.Replace( ASSETS, "" ) + filePath );
            bool didFixerChangeSomething = false;

            if ( file.Exists )
            {
                var text = File.ReadAllText( file.FullName );

                if ( !text.Contains( $"\n{NEW_IMPORT}" ) && text.Contains( IMPORT ) )
                {
                    text = text.ReplaceFirst( IMPORT, $"\n{NEW_IMPORT}{IMPORT}" );
                    didFixerChangeSomething = true;
                }

                if ( text.Contains( DID_FINISH_LAUNCHING_LINE ) )
                {
                    if ( !text.Contains( NEW_DID_FINISH_LAUNCHING_LINE_PART ) ) //checking for NEW_DID_FINISH_LAUNCHING_CONTENT didn't always work
                    {
                        text = text.ReplaceFirst( DID_FINISH_LAUNCHING_LINE, NEW_DID_FINISH_LAUNCHING_CONTENT );
                        didFixerChangeSomething = true;
                    }
                }
#if HUF_TESTS
                else
                    HLog.LogError( $"{CONTROLLER_PATH} doesn't contain line: {DID_FINISH_LAUNCHING_LINE}" );
#endif

                if ( text.Contains( BAD_STRING ) )
                {
                    text = text.Replace( BAD_STRING, BAD_STRING_REPLACEMENT );
                    var id = text.LastIndexOf( AT_SIGN_END, StringComparison.InvariantCulture );
                    text = text.Insert( id - 1, NEW_PLUGIN_CONTENT );
                    didFixerChangeSomething = true;
                }

                if ( didFixerChangeSomething )
                {
                    File.WriteAllText( file.FullName, text );
                    HLog.Log( "SDK Fixed: " + file.FullName );
                }
                else
                    HLog.Log( "SDK Fixer skipped" );
            }
        }
    }
}