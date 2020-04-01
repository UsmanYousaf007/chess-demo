using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using HUF.Utils.Runtime.Logging;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace HUFEXT.GenericGDPR.Editor
{
    public class GDPRValidator : IPreprocessBuildWithReport
    {
        readonly HLogPrefix prefix = new HLogPrefix( nameof( GDPRValidator ) );
        
        public int callbackOrder => 0;

        //[MenuItem( "HUF/GDPR/Validate" )]
        public static void Validate()
        {
            new GDPRValidator().OnPreprocessBuild( null );
        }
        
        public void OnPreprocessBuild( BuildReport report )
        {
            var detected = GenerateHash();
            const string expected = "917278a3be363c4155abd55557e09c27f1c1a6caf12d74656ee0c95e94bb2d18";
            
            if ( detected != expected )
            {
                GenerateInvalidHashReport( detected, expected );
                HLog.LogError( prefix, "Policy text mismatch detected. Please contact HUF support." );
            }
        }
        
        string GenerateHash()
        {
            using ( var crypto = SHA256.Create() )
            {
                using ( var stream = File.OpenRead( "Assets/HUFEXT/GenericGDPR/Runtime/Utils/GDPRTranslationsProvider.cs" ) )
                {
                    var bytes = crypto.ComputeHash( stream );
                    var builder = new StringBuilder();  
                    for ( int i = 0; i < bytes.Length; i++ )
                    {
                        builder.Append( bytes[i].ToString( "x2" ) );
                    }
                    return builder.ToString();
                }
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
