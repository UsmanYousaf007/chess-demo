using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using HUF.Utils.Runtime.Logging;
using HUFEXT.GenericGDPR.Runtime.Utils;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace HUFEXT.GenericGDPR.Editor
{
    public class GDPRValidator : IPreprocessBuildWithReport
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof( GDPRValidator ) );
        
        public int callbackOrder => 0;

#if HUF_TESTS
        [MenuItem( "HUF/GDPR/Validate" )]
        public static void Validate()
        {
            new GDPRValidator().OnPreprocessBuild( null );
            Debug.Log( GenerateHash() );
        }
#endif
        
        public void OnPreprocessBuild( BuildReport report )
        {
            var detected = GenerateHash();
            const string expected = "a5e77175e707a6b0b58fa9fa49ec1dbb1869ab1cb217cd133ffe44519ffc5df0";
            
            if ( detected != expected )
            {
                GenerateInvalidHashReport( detected, expected );
                HLog.LogWarning( logPrefix, "Policy text mismatch detected. Please contact HUF support." );
            }
        }
        
        static string GenerateHash()
        {
            var data = new StringBuilder();
            foreach ( var translation in GDPRTranslationsProvider.Translations )
            {
                data.Append( translation.policy )
                    .Append( translation.footer )
                    .Append( translation.toggle );
            }

            using ( var crypto = SHA256.Create() )
            {
                var bytes = crypto.ComputeHash( Encoding.UTF8.GetBytes( data.ToString() ) );
                data.Clear();
                foreach ( var character in bytes )
                {
                    data.Append( character.ToString( "x2" ) );
                }
                return data.ToString();
            }
        }

        static void GenerateInvalidHashReport( string detectedHash, string expectedHash )
        {
            var parameters = new Dictionary<string, string>()
            {
                { "huf_developer_id", "Unknown" },
                { "huf_package_name", PlayerSettings.applicationIdentifier },
                { "huf_version", PlayerSettings.bundleVersion },
                { "huf_build_time", DateTime.Now.ToString( CultureInfo.InvariantCulture ) },
                { "huf_unity", Application.unityVersion },
                { "huf_operating_system", SystemInfo.operatingSystem },
                { "huf_platform", EditorUserBuildSettings.activeBuildTarget.ToString() },
                { "huf_expected_hash", expectedHash },
                { "huf_detected_hash", detectedHash },
                { "huf_report_type", "INVALID_HASH" }
            };

            switch ( EditorUserBuildSettings.activeBuildTarget )
            {
                case BuildTarget.Android:
                {
                    parameters.Add( "huf_minimum_api_version", PlayerSettings.Android.minSdkVersion.ToString() );
                    break;
                }
                case BuildTarget.iOS:
                {
                    parameters.Add( "huf_minimum_api_version", PlayerSettings.iOS.targetOSVersionString );
                    break;
                }
            }

            HUF.Build.Report.Sender.SendReport( parameters );
        }
    }
}
