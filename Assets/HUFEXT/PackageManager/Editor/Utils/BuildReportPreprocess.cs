using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Events;

namespace HUFEXT.PackageManager.Editor.Utils
{
    public class BuildReportPreprocess : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public enum ReportStatus
        {
            Success = 0,
            Failed
        }

        static Dictionary<string, string> buildParameters;
        static UnityEditor.PackageManager.Requests.ListRequest packagesRequest;

        static UnityAction<Dictionary<string, string>> OnReportGenerationCompleted;

        public static ReportStatus GenerateBuildInfo( UnityAction<Dictionary<string, string>> response )
        {
            var status = ReportStatus.Success;

            if ( !Models.Token.Exists )
            {
                return ReportStatus.Failed;
            }

            buildParameters = new Dictionary<string, string>()
            {
                {Models.Keys.BuildEventKey.DEV_ID, Models.Token.ID},
                {Models.Keys.BuildEventKey.PACKAGE_NAME, PlayerSettings.applicationIdentifier},
                {Models.Keys.BuildEventKey.VERSION, PlayerSettings.bundleVersion},
                {Models.Keys.BuildEventKey.BUILD_TIME, DateTime.Now.ToString( CultureInfo.InvariantCulture )},
                {Models.Keys.BuildEventKey.UNITY_VERSION, Application.unityVersion},
                {Models.Keys.BuildEventKey.OS, SystemInfo.operatingSystem},
                {Models.Keys.BuildEventKey.PLATFORM, EditorUserBuildSettings.activeBuildTarget.ToString()},
            };

            switch ( EditorUserBuildSettings.activeBuildTarget )
            {
                case BuildTarget.Android:
                {
                    buildParameters.Add( Models.Keys.BuildEventKey.API_VERSION,
                        PlayerSettings.Android.minSdkVersion.ToString() );
                    break;
                }
                case BuildTarget.iOS:
                {
                    buildParameters.Add( Models.Keys.BuildEventKey.API_VERSION,
                        PlayerSettings.iOS.targetOSVersionString );
                    break;
                }
            }

            foreach ( var package in Core.Packages.Local )
            {
                var name = string.Empty;
                var arr = package.name.Split( '.' );

                if ( arr.Length > 0 )
                {
                    name = arr.Last();
                }

                buildParameters[Models.Keys.BuildEventKey.HUF_PACKAGE + name] = package.version;
            }

            OnReportGenerationCompleted = response;
            
#if UNITY_2019_1_OR_NEWER
            packagesRequest = UnityEditor.PackageManager.Client.List( false, true );
#else
            packagesRequest = UnityEditor.PackageManager.Client.List( false );
#endif
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

                    buildParameters[Models.Keys.BuildEventKey.UNITY_PACKAGE + name] = package.version;
                }
            }

            if ( packagesRequest.Status != UnityEditor.PackageManager.StatusCode.InProgress )
            {
                OnReportGenerationCompleted?.Invoke( buildParameters );
                OnReportGenerationCompleted = null;
                EditorApplication.update -= FetchUnityPackages;
            }
        }

        public static string SerializeReport( Dictionary<string, string> dict )
        {
            var entries = dict.Select( d => $"    \"{d.Key}\": \"{d.Value}\"" );
            return $"{{\n{string.Join( ",\n", entries )}\n}}";
        }

        public void OnPreprocessBuild( UnityEditor.Build.Reporting.BuildReport report )
        {
            GenerateBuildInfo( Reporter.Send );
        }

#if HPM_DEV_MODE
        [MenuItem("HUF/Debug/Send Test HBI report")]
        public static void SendTestReport()
        {
            new BuildReportPreprocess().OnPreprocessBuild( null );
        }
#endif
    }
}