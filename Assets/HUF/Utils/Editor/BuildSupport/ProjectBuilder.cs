using System;
using System.Collections.Generic;
using System.IO;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using UnityEngine;

namespace HUF.Utils.Editor.BuildSupport
{
    public class ProjectBuilder : IPostprocessBuildWithReport
    {
        public const string CMD_TARGET = "-T_"; //Target - iOS, Android
        public const string CMD_BUILD_TYPE = "-BT_"; //Build Type - Debug, Production
        public const string CMD_DIRECTORY = "-D_"; //Directory path (+ android file name)
        public const string CMD_BUILD_NUMBER = "-BN_"; //Build Number (bundle Version)
        public const string CMD_BUILD_NUMBER_ADDITIVE = "-BNA_"; //Build Number Additive - add to current
        public const string CMD_BUILD_VERSION = "-V_"; //Build Version (1.2.30)
        public const string CMD_KEYSTORE_PASS = "-KP_"; //Keystore Pass
        public const string CMD_KEYSTORE_PATH = "-KPT_"; //Keystore Path
        public const string CMD_KEYALIAS_NAME = "-KA_"; //KeyAlias Name
        public const string CMD_KEYALIAS_PASS = "-KAP_"; //KeyAlias Pass
        public const string CMD_IOS_TEAM_ID = "-TID_"; //iOS Team id
        public const string CMD_IOS_PP_ID = "-PPID_"; //iOS Provisioning Profile Id
        public const string CMD_IOS_PP_TYPE = "-PPT_"; //Provisioning Profile Type Automatic-default, Distribution, Development
        public const string CMD_OBB = "-OBB_1"; //build Android obb
        public const string CMD_AAB = "-AAB"; //build Android abb


        static readonly string[] androidDebug = {"-T_Android", "-BT_Debug"};
        static readonly string[] androidProduction = {"-T_Android", "-BT_Production"};
        static readonly string[] iOSDebug = {"-T_iOS", "-BT_Debug"};
        static readonly string[] iOSProduction = {"-T_iOS", "-BT_Production"};

        public int callbackOrder => 999;

        const string MENU_PATH = "HUF/Tools/Build/";
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(ProjectBuilder) );

        [MenuItem( MENU_PATH + "Android Build Debug" )]
        static void BuildAndroidDebug()
        {
            StartBuild( androidDebug );
        }

        [MenuItem( MENU_PATH + "Android Build Production" )]
        static void BuildAndroidProd()
        {
            StartBuild( androidProduction );
        }

        [MenuItem( MENU_PATH + "iOS Build Debug" )]
        static void BuildIOSDebug()
        {
            StartBuild( iOSDebug );
        }

        [MenuItem( MENU_PATH + "iOS Build Production" )]
        static void BuildIOSProd()
        {
            StartBuild( iOSProduction );
        }

        public static void StartBuild()
        {
            StartBuild( null );
        }

        public static void StartBuild( string[] commandLineArgs )
        {
            HLog.LogAlways( logPrefix, "###########################################" );
            HLog.LogAlways( logPrefix, "           START BUILD PROCESS" );
            HLog.LogAlways( logPrefix, "###########################################" );

            if ( commandLineArgs == null )
            {
                commandLineArgs = Environment.GetCommandLineArgs();
            }

            ProcessCommandLinesArgs( commandLineArgs,
                out BuildTarget platformTarget,
                out bool isDebugBuild,
                out string directoryPath,
                out int buildNumber,
                out bool buildNumberAdditive,
                out string buildVersion );

            buildNumber = ProcessBuildNumber( buildNumber, platformTarget, buildNumberAdditive );

            directoryPath = ManageEndDirectoryAndFile( directoryPath, platformTarget, isDebugBuild, buildNumber );

            EditorUserBuildSettings.development = isDebugBuild;

            if ( buildVersion != null )
            {
                PlayerSettings.bundleVersion = buildVersion;
            }

            AssetDatabase.SaveAssets();

            var options = new BuildPlayerOptions()
            {
                target = platformTarget,
                scenes = GetCurrentScenes(),
                locationPathName = directoryPath,
                targetGroup = platformTarget == BuildTarget.Android ? BuildTargetGroup.Android : BuildTargetGroup.iOS,
                options = isDebugBuild ? BuildOptions.Development : BuildOptions.None,
            };

            var report = BuildPipeline.BuildPlayer( options );

            if ( report.summary.result != BuildResult.Succeeded )
            {
                var buildReportInfo = $"==========================\n";

                for ( int i = 0; i < report.steps.Length; i++ )
                {
                    var step = report.steps[i];
                    buildReportInfo += $"{step.name}:\n";

                    for ( int j = 0; j < step.messages.Length; j++ )
                    {
                        if ( step.messages[j].type == LogType.Error || step.messages[j].type == LogType.Exception )
                            buildReportInfo += $"{step.messages[j].type}: {step.messages[j].content}\n";
                    }

                    buildReportInfo += "\n\n";
                }

                buildReportInfo += "==========================";
                throw new Exception( "[ERROR] Build Failed! Reason: " + buildReportInfo );
            }

            HLog.LogAlways( logPrefix, "###########################################" );
            HLog.LogAlways( logPrefix, "           BUILD PROCESS COMPLETE" );
            HLog.LogAlways( logPrefix, "###########################################" );
        }

        static string ManageEndDirectoryAndFile( string directoryPath,
            BuildTarget platformTarget,
            bool isDebugBuild,
            int buildNumber )
        {
            if ( directoryPath.StartsWith( Application.dataPath ) )
            {
                if ( platformTarget == BuildTarget.iOS )
                {
                    directoryPath += $"XcodeProject_{( isDebugBuild ? "Debug" : "Prod" )}_{buildNumber}/";
                }
                else
                {
                    directoryPath += $"AndroidBuild_{( isDebugBuild ? "Debug" : "Prod" )}_{buildNumber}." +
                                     $"{( PlayerSettings.Android.useAPKExpansionFiles ? "obb" : "apk" )}";
                }
            }

            var folderPath = directoryPath;
            var splitedPath = directoryPath.Split( '/' );

            if ( splitedPath.Length > 1 )
                folderPath = directoryPath.Replace( splitedPath[splitedPath.Length - 1], "" );

            if ( Directory.Exists( folderPath ) )
            {
                Directory.Delete( folderPath, true );
            }

            if ( File.Exists( folderPath ) )
            {
                File.Delete( folderPath );
            }

            Directory.CreateDirectory( folderPath );
            return directoryPath;
        }

        static int ProcessBuildNumber( int buildNumber, BuildTarget platformTarget, bool buildNumberAdditive )
        {
            if ( buildNumber >= 0 )
            {
                if ( platformTarget == BuildTarget.Android )
                {
                    if ( buildNumberAdditive )
                        buildNumber += PlayerSettings.Android.bundleVersionCode;
                    PlayerSettings.Android.bundleVersionCode = buildNumber;
                }
                else
                {
                    if ( buildNumberAdditive )
                    {
                        int.TryParse( PlayerSettings.iOS.buildNumber, out int buildParseNumber );
                        buildNumber += buildParseNumber;
                    }

                    PlayerSettings.iOS.buildNumber = buildNumber.ToString();
                }
            }
            else
            {
                if ( platformTarget == BuildTarget.Android )
                {
                    buildNumber = PlayerSettings.Android.bundleVersionCode;
                }
                else
                {
                    int.TryParse( PlayerSettings.iOS.buildNumber, out buildNumber );
                }
            }

            return buildNumber;
        }

        static void ProcessCommandLinesArgs( string[] commandLineArgs,
            out BuildTarget platformTarget,
            out bool isDebugBuild,
            out string directoryPath,
            out int buildNumber,
            out bool buildNumberAdditive,
            out string buildVersion )
        {
            platformTarget = BuildTarget.Android;
            isDebugBuild = false;
            directoryPath = Application.dataPath + "/../Builds/";
            buildNumber = -1;
            buildNumberAdditive = false;
            buildVersion = null;
            string keystorePass = null;
            string keystorePath = null;
            string keyaliasName = null;
            string keyaliasPass = null;
            string iOSTeamId = null;
            string provisioningProfileId = null;
            var provisioningProfileType = ProvisioningProfileType.Automatic;

            foreach ( string arg in commandLineArgs )
            {
                if ( arg.StartsWith( CMD_TARGET )  && arg.Length > CMD_TARGET.Length)
                {
                    platformTarget = arg.ToLower().Contains( "ios" ) ? BuildTarget.iOS : BuildTarget.Android;
                }
                else if ( arg.StartsWith( CMD_BUILD_TYPE )  && arg.Length > CMD_BUILD_TYPE.Length)
                {
                    isDebugBuild = arg.ToLower().Contains( "debug" );
                }
                else if ( arg.StartsWith( CMD_DIRECTORY ) && arg.Length > CMD_DIRECTORY.Length)
                {
                    directoryPath = arg.Replace( CMD_DIRECTORY, string.Empty );
                }
                else if ( arg.StartsWith( CMD_BUILD_NUMBER ) && arg.Length > CMD_BUILD_NUMBER.Length )
                {
                    int.TryParse( arg.Replace( CMD_BUILD_NUMBER, string.Empty ), out buildNumber );
                }
                else if ( arg.StartsWith( CMD_BUILD_NUMBER_ADDITIVE )  && arg.Length > CMD_BUILD_NUMBER_ADDITIVE.Length )
                {
                    int.TryParse( arg.Replace( CMD_BUILD_NUMBER_ADDITIVE, string.Empty ), out buildNumber );
                    buildNumberAdditive = true;
                }
                else if ( arg.StartsWith( CMD_BUILD_VERSION ) && arg.Length > CMD_BUILD_VERSION.Length)
                {
                    buildVersion = arg.Replace( CMD_BUILD_VERSION, string.Empty );
                }
                else if ( arg.StartsWith( CMD_KEYSTORE_PASS ) && arg.Length > CMD_KEYSTORE_PASS.Length)
                {
                    keystorePass = arg.Replace( CMD_KEYSTORE_PASS, string.Empty );
                }
                else if ( arg.StartsWith( CMD_KEYSTORE_PATH ) && arg.Length > CMD_KEYSTORE_PATH.Length)
                {
                    keystorePath = arg.Replace( CMD_KEYSTORE_PATH, string.Empty );
                }
                else if ( arg.StartsWith( CMD_KEYALIAS_NAME ) && arg.Length > CMD_KEYALIAS_NAME.Length)
                {
                    keyaliasName = arg.Replace( CMD_KEYALIAS_NAME, string.Empty );
                }
                else if ( arg.StartsWith( CMD_KEYALIAS_PASS ) && arg.Length > CMD_KEYALIAS_PASS.Length)
                {
                    keyaliasPass = arg.Replace( CMD_KEYALIAS_PASS, string.Empty );
                }
                else if ( arg == CMD_OBB )
                {
                    PlayerSettings.Android.useAPKExpansionFiles = true;
                }
                else if ( arg == CMD_AAB )
                {
                    EditorUserBuildSettings.buildAppBundle = true;
                }
                else if ( arg.StartsWith( CMD_IOS_TEAM_ID ) && arg.Length > CMD_IOS_TEAM_ID.Length)
                {
                    iOSTeamId = arg.Replace( CMD_IOS_TEAM_ID, string.Empty );
                }
                else if ( arg.StartsWith( CMD_IOS_PP_ID ) && arg.Length > CMD_IOS_PP_ID.Length)
                {
                    provisioningProfileId = arg.Replace( CMD_IOS_PP_ID, string.Empty );
                }
                else if ( arg.StartsWith( CMD_IOS_PP_TYPE ) && arg.Length > CMD_IOS_PP_TYPE.Length)
                {
                    switch ( arg.Replace( CMD_IOS_PP_TYPE, string.Empty ) )
                    {
                        case "Automatic":
                            provisioningProfileType = ProvisioningProfileType.Automatic;
                            break;
                        case "Development":
                            provisioningProfileType = ProvisioningProfileType.Development;
                            break;
                        case "Distribution":
                            provisioningProfileType = ProvisioningProfileType.Distribution;
                            break;
                    }
                }
            }

#if UNITY_2019_1_OR_NEWER
            if ( PlayerSettings.Android.useCustomKeystore &&
                 PlayerSettings.Android.keystorePass == string.Empty && keystorePass == null )
                PlayerSettings.Android.useCustomKeystore = false;
            else if ( PlayerSettings.Android.useCustomKeystore == false && !keystorePass.IsNullOrEmpty())
                PlayerSettings.Android.useCustomKeystore = true;
#else
            if ( PlayerSettings.Android.useAPKExpansionFiles &&
                 PlayerSettings.Android.keystorePass == string.Empty && keystorePass == null )
                PlayerSettings.Android.useAPKExpansionFiles = false;
            else if ( PlayerSettings.Android.useAPKExpansionFiles == false && !keystorePass.IsNullOrEmpty())
                PlayerSettings.Android.useAPKExpansionFiles = true;
#endif


            if ( !keystorePass.IsNullOrEmpty() )
                PlayerSettings.Android.keystorePass = keystorePass;

            if ( !keystorePath.IsNullOrEmpty() )
                PlayerSettings.Android.keystoreName = keystorePath;

            if ( !keyaliasName.IsNullOrEmpty())
                PlayerSettings.Android.keyaliasName = keyaliasName;

            if ( !keyaliasPass.IsNullOrEmpty())
                PlayerSettings.Android.keyaliasPass = keyaliasPass;

            if ( iOSTeamId != null && provisioningProfileId != null )
            {
                PlayerSettings.iOS.appleDeveloperTeamID = iOSTeamId;
                PlayerSettings.iOS.appleEnableAutomaticSigning = false;
                PlayerSettings.iOS.iOSManualProvisioningProfileID = provisioningProfileId;
                PlayerSettings.iOS.iOSManualProvisioningProfileType = provisioningProfileType;
            }
        }

        static string[] GetCurrentScenes()
        {
            var scenesToBuild = new List<string>();

            foreach ( var scene in EditorBuildSettings.scenes )
            {
                if ( scene.enabled )
                {
                    scenesToBuild.Add( scene.path );
                }
            }

            return scenesToBuild.ToArray();
        }

        public void OnPostprocessBuild( BuildReport report )
        {
            var pathToBuiltProject = report.summary.outputPath;
            var target = report.summary.platform;

            if ( target != BuildTarget.iOS )
            {
                return;
            }

#if UNITY_IOS
            //Debug.LogFormat("Postprocessing build at \"{0}\" for target {1}", pathToBuiltProject, target);
            PBXProject project = new PBXProject();
            string pbxFilename = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
            project.ReadFromFile( pbxFilename );
#if UNITY_2019_3_OR_NEWER
            string targetGUID = project.GetUnityMainTargetGuid();
#else
        string targetName = PBXProject.GetUnityTargetName();
        string targetGUID = project.TargetGuidByName(targetName);
#endif
            var token = project.GetBuildPropertyForAnyConfig( targetGUID, "USYM_UPLOAD_AUTH_TOKEN" );

            if ( string.IsNullOrEmpty( token ) )
            {
                token = "if this did not help ask Robert";
            }

            project.SetBuildProperty( targetGUID, "USYM_UPLOAD_AUTH_TOKEN", token );
            project.WriteToFile( pbxFilename );
#endif
        }
    }
}