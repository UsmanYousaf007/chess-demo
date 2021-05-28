#if HUFEXT_PACKAGE_MANAGER
using System.IO;
using System.Text.RegularExpressions;
using HUF.Utils.Runtime.Extensions;
using HUFEXT.PackageManager.Editor.Commands.Data;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using HUFEXT.PackageManager.Editor.Core;
using HUFEXT.PackageManager.Editor.Models;

public class FirebaseVersionConsolidator : IPreprocessBuildWithReport
{
    const string FIREBASE_SDK_UNITY_PLUGIN = "Firebase SDK Unity plugin";
    const string COM_HUUUGE_PLUGINS_FIREBASE = "com.huuuge.plugins.firebase";
    const string DOT_FIREBASE = ".firebase";
    const string ZERO_VERSION = "0.0.0";

    static readonly Regex findVersionInChangelogRegex = new Regex( @"\d*[.]\d*[.]\d*" );

    public int callbackOrder => int.MinValue;

    public virtual void OnPreprocessBuild( BuildReport report )
    {
        Execute();
    }

    [MenuItem( "HUF/Check Firebase SDKs versions" )]
    static void MenuItem()
    {
        Execute();
    }

    static void Execute()
    {
        Command.Execute( new GetLocalPackagesCommand()
        {
            OnComplete = ( result, serializedData ) =>
            {
                var localPackages = HUFEXT.PackageManager.Editor.Core.Packages.Local;

                var corePackage =
                    localPackages.Find( p => p.name == COM_HUUUGE_PLUGINS_FIREBASE );

                if ( corePackage == null )
                    return;

                var corePackageSDKVersion = GetFirebaseSDKVersionFromChangelog( corePackage );
                var packagesToUpdateText = string.Empty;

                foreach ( var package in localPackages )
                {
                    if ( package.name.EndsWith( DOT_FIREBASE ) )
                    {
                        var packageSDKVersion = GetFirebaseSDKVersionFromChangelog( package );

                        if ( packageSDKVersion != corePackageSDKVersion )
                            packagesToUpdateText += $"{package.displayName}\n";
                    }
                }

                if ( packagesToUpdateText.IsNullOrEmpty() )
                    return;

                EditorUtility.DisplayDialog( "IMPORTANT",
                    $"Firebase SDK packages not matching version {corePackage.version} were found:\n\n{packagesToUpdateText}\n" +
                    "Please open HUF Package Manager and install Firebase packages with the same SDK version",
                    "OK" );
                return;
            }
        } );
    }

    static string GetFirebaseSDKVersionFromChangelog( PackageManifest package )
    {
        var changelogPath = $"{package.huf.path}/CHANGELOG.md";

        foreach ( var line in File.ReadLines( changelogPath ) )
        {
            if ( line.Contains( FIREBASE_SDK_UNITY_PLUGIN ) )
            {
                var match = findVersionInChangelogRegex.Match( line );

                if ( match.Success )
                {
                    return match.Value;
                }

                break;
            }
        }

        return ZERO_VERSION;
    }
}
#endif