﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HUFEXT.PackageManager.Editor.Implementation.Local.Services;
using HUFEXT.PackageManager.Editor.Implementation.Remote.Auth;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Events;
using Report = HUF.Build.Report.Sender;

namespace HUFEXT.PackageManager.Editor.Utils
{
    public class BuildReportPreprocess : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;
        
        private static class Keys
        {
            private const string PREFIX = "huf_";
            internal const string DEV_ID = PREFIX + "developer_id";
            internal const string PACKAGE_NAME = PREFIX + "package_name";
            internal const string VERSION = PREFIX + "version";
            internal const string BUILD_TIME = PREFIX + "build_time";
            internal const string UNITY_VERSION = PREFIX + "unity";
            internal const string OS = PREFIX + "operating_system";
            internal const string PLATFORM = PREFIX + "platform";
            internal const string API_VERSION = PREFIX + "minimum_api_version";
            internal const string HUF_PACKAGE = PREFIX + "package_";
            internal const string UNITY_PACKAGE = "unity_package_";
        }

        public enum ReportStatus
        {
            Success = 0,
            Failed
        }

        private static Dictionary<string, string> buildParameters;
        private static ListRequest packagesRequest;

        static UnityAction<Dictionary<string, string>> OnReportGenerationCompleted;

        public static ReportStatus GenerateBuildInfo( UnityAction<Dictionary<string, string>> response )
        {
            var status = ReportStatus.Success;
            var token = Token.LoadExistingToken();
            if ( token == null )
            {
                return ReportStatus.Failed;
            }

            buildParameters = new Dictionary<string, string>()
            {
                { Keys.DEV_ID, token.DeveloperID },
                { Keys.PACKAGE_NAME, PlayerSettings.applicationIdentifier },
                { Keys.VERSION, PlayerSettings.bundleVersion },
                { Keys.BUILD_TIME, DateTime.Now.ToString( CultureInfo.InvariantCulture ) },
                { Keys.UNITY_VERSION, Application.unityVersion },
                { Keys.OS, SystemInfo.operatingSystem },
                { Keys.PLATFORM, EditorUserBuildSettings.activeBuildTarget.ToString() },
            };

            switch ( EditorUserBuildSettings.activeBuildTarget )
            {
                case BuildTarget.Android:
                {
                    buildParameters.Add( Keys.API_VERSION, PlayerSettings.Android.minSdkVersion.ToString() );
                    break;
                }
                case BuildTarget.iOS:
                {
                    buildParameters.Add( Keys.API_VERSION, PlayerSettings.iOS.targetOSVersionString );
                    break;
                }
            }

            new LocalPackagesService().RequestPackagesList( string.Empty, ( localPackages ) =>
            {
                if ( !token.IsValid && localPackages.Count > 0)
                {
                    status = ReportStatus.Failed;
                }

                foreach ( var package in localPackages )
                {
                    var name = string.Empty;
                    var arr = package.name.Split( '.' );
                    if ( arr.Length > 0 )
                    {
                        name = arr.Last();
                    }
                    buildParameters[Keys.HUF_PACKAGE + name] = package.version;
                }
            } );

            OnReportGenerationCompleted = response;

            packagesRequest = Client.List( true );
            EditorApplication.update += FetchUnityPackages;

            return status;
        }

        static void FetchUnityPackages()
        {
            if ( packagesRequest.IsCompleted )
            {
                foreach ( var package in packagesRequest.Result )
                {
                    var name = string.Empty;
                    var arr = package.name.Split( '.' );
                    if ( arr.Length > 0 )
                    {
                        name = arr.Last();
                    }
                    buildParameters[Keys.UNITY_PACKAGE + name] = package.version;
                }
            }

            if ( packagesRequest.Status != StatusCode.InProgress )
            {
                OnReportGenerationCompleted?.Invoke( buildParameters );
                OnReportGenerationCompleted = null;
                EditorApplication.update -= FetchUnityPackages;
            }
        }

        public static string SerializeReport( Dictionary<string, string> dict )
        {
            var entries = dict.Select(d => $"    \"{d.Key}\": \"{d.Value}\"" );
            return "{\n" + string.Join(",\n", entries) + "\n}";
        }
        
        public void OnPreprocessBuild( UnityEditor.Build.Reporting.BuildReport report )
        {
            GenerateBuildInfo( Report.SendReport );
        }
    }
}