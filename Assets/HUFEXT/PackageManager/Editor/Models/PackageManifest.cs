using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HUFEXT.PackageManager.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Models
{
    [Serializable]
    public enum PackageStatus
    {
        Unknown = 0, // Default status for new package instance.
        Unavailable, // This status is used if package is on remote server but manager can't download it.
        NotInstalled, // Package is found on remote server but is not installed in project.
        Installed, // Package is installed and up to date.
        ForceUpdate, // This package must be updated immediately.
        UpdateAvailable, // There is new version of package on remote server, but user can decide when he want make update.
        Migration, // Status for packages that have different names but same path.
        Conflict, // Status for packages that have same name but different paths.
        Embedded, // Status for packages without manifest and version data (Used for submodules in old format).
        Git, // Status for repositories with package structure.
        GitUpdate, // Status for repositories with not updated package version.
        GitError, // Status for repositories with old package version.
    }

    public static class Rollout
    {
        public const string NOT_HUF_LABEL = "Not HUF";
        public const string VCS_LABEL = "Version Control";
        public const string EXPERIMENTAL_LABEL = "Experimental";
        public const string DEVELOPMENT_LABEL = "Development";
        public const string NOT_INSTALLED_LABEL = "Not Installed";
        public const string UNITY_LABEL = "Unity Packages";
        public const string UNDEFINED_LABEL = "Undefined";
    }

    [Serializable]
    public class PackageManifest
    {
        [Serializable]
        public class Author
        {
            public string name = "HUF Development Team";

            public Author() { }

            public Author( string name )
            {
                this.name = name;
            }
        }

        [Serializable]
        public class Metadata
        {
            public PackageConfig config = new PackageConfig();
            public PackageStatus status = PackageStatus.Unknown;
            public string scope = "public";
            public string channel = string.Empty;
            public string message = string.Empty;
            public string version = string.Empty;
            public string prerelease = string.Empty;
            public string date = string.Empty;
            public string commit = string.Empty;
            public string rollout = string.Empty;
            public string build = string.Empty;
            public string path = string.Empty;
            public bool isLocal = false;
            public bool isUnity = false;
            public List<string> dependencies = new List<string>();
            public List<string> optionalDependencies = new List<string>();
            public List<string> exclude = new List<string>();
            public List<string> details = new List<string>();
            public List<string> scopes = new List<string>();
        }

        public string name = string.Empty;
        public string displayName = string.Empty;
        public string description = "No detailed information's about package found.";
        public string version = "0.0.0-unknown";
        public string unity = string.Empty;
        public Author author = new Author();
        public Metadata huf = new Metadata();
        bool checkedIfCurrentUnitySupportsThisPackage = false;
        bool supportsCurrentUnityVersion = true;

        public bool IsInstalled => huf.status == PackageStatus.Installed ||
                                   huf.status == PackageStatus.UpdateAvailable ||
                                   huf.status == PackageStatus.ForceUpdate;

        public bool IsUpdate => huf.status == PackageStatus.UpdateAvailable ||
                                huf.status == PackageStatus.ForceUpdate;

        public bool IsRepository => huf.status == PackageStatus.Git ||
                                    huf.status == PackageStatus.GitUpdate ||
                                    huf.status == PackageStatus.GitError;

        public bool IsStable => string.IsNullOrEmpty( huf.prerelease );
        public bool IsHufPackage => name.Contains( ".huuuge." );

        public bool HasExternalDependencies =>
            huf.dependencies.Select( p => new Models.Dependency( p ) ).Any( d => !d.IsHufPackage );

        public string LocalChangelog
        {
            get
            {
                var changelogPath = $"{huf.path}/CHANGELOG.md";

                if ( File.Exists( changelogPath ) )
                {
                    return File.ReadAllText( changelogPath );
                }

                return string.Empty;
            }
        }

        public string RemoteChangelog { get; set; }

        public bool SupportsCurrentUnityVersion
        {
            get
            {
                if ( !checkedIfCurrentUnitySupportsThisPackage )
                    CheckIfCurrentUnitySupportsThisPackage();
                return supportsCurrentUnityVersion;
            }
        }

        public static PackageManifest ParseManifest( string file, bool loadFromMemory = false )
        {
            var manifest = new PackageManifest();
            EditorJsonUtility.FromJsonOverwrite( loadFromMemory ? file : File.ReadAllText( file ), manifest );
            manifest.ParseVersion();
            return manifest;
        }

        public override string ToString()
        {
            return EditorJsonUtility.ToJson( this );
        }

        public static void SaveManifest( string file, PackageManifest manifest )
        {
            File.WriteAllText( file, EditorJsonUtility.ToJson( manifest, true ) );
        }

        public static void SaveConfig( string file, PackageConfig config )
        {
            File.WriteAllText( file, EditorJsonUtility.ToJson( config, true ) );
        }

        public bool TryGetChangelog( out string changelog )
        {
            changelog = string.Empty;

            if ( !string.IsNullOrEmpty( RemoteChangelog ) )
            {
                changelog = RemoteChangelog;
                return true;
            }

            var changelogPath = $"{huf.path}/CHANGELOG.md";

            if ( File.Exists( changelogPath ) )
            {
                changelog = File.ReadAllText( changelogPath );
                return true;
            }

            return false;
        }

        public PackageManifest LatestPackageVersion()
        {
            var latestPackage = Core.Packages.Remote.Find( package => package.name == name );

            if ( latestPackage == null )
            {
                if ( IsHufPackage )
                    Utils.Common.LogError( "Manifest wasn't found in the remote package: " + name,
                        nameof(PackageManifest) );
                return this;
            }
            else
                return latestPackage;
        }

        public Dependency ToDependency()
        {
            return new Dependency( name, version );
        }

        public void ParseVersion()
        {
            if ( string.IsNullOrEmpty( version ) )
            {
                version = "0.0.0-unknown";
            }

            var parts = version.Split( '-' );

            switch ( parts.Length )
            {
                case 1:
                {
                    huf.version = parts[0];
                    break;
                }
                case 2:
                {
                    huf.version = parts[0];
                    huf.prerelease = parts[1];
                    break;
                }
                default: return;
            }
        }

        public void CheckIfCurrentUnitySupportsThisPackage()
        {
            try
            {
                var stringBuilder = new StringBuilder();

                foreach ( var ch in Application.unityVersion )
                {
                    if ( char.IsDigit( ch ) || ch == '.' )
                        stringBuilder.Append( ch );
                    else
                        break;
                }

                var unityVersion = stringBuilder.ToString();
                string packageUnityVersion = unity.Replace( " ", "" );

                if ( string.IsNullOrEmpty( packageUnityVersion ) )
                    return;

                string[] versionRanges = packageUnityVersion.Split( ',' );
                bool isUnityVersionSupported = false;

                foreach ( var versionRange in versionRanges )
                {
                    var versions = versionRange.Split( '-' );

                    if ( versions.Length == 1 )
                    {
                        if ( VersionComparer.Compare( unityVersion, versions[0], true, true ) >= 0 )
                        {
                            isUnityVersionSupported = true;
                            break;
                        }
                    }
                    else if ( VersionComparer.Compare( unityVersion, versions[0], true, true ) >= 0 &&
                              VersionComparer.Compare( unityVersion, versions[1], true, true ) <= 0 )
                    {
                        isUnityVersionSupported = true;
                        break;
                    }
                }

                if ( !isUnityVersionSupported )
                {
                    supportsCurrentUnityVersion = false;
                }
            }
            catch ( Exception e )
            {
                Debug.LogError( e );
            }
        }

        public bool IsVersionHigherOrEqualTo( Dependency dependency, bool ignoreTags = true ) =>
            IsVersionHigherOrEqualTo( dependency.version, ignoreTags );

        public bool IsVersionHigherOrEqualTo( PackageManifest packageManifest, bool ignoreTags = true ) =>
            IsVersionHigherOrEqualTo( packageManifest.version, ignoreTags );

        public bool IsVersionHigherOrEqualTo( string version, bool ignoreTags = true ) =>
            Utils.VersionComparer.Compare( this.version, version, ignoreTags ) >= 0;

        public bool IsVersionHigherTo( Dependency dependency, bool ignoreTags = true ) =>
            IsVersionHigherTo( dependency.version, ignoreTags );

        public bool IsVersionHigherTo( PackageManifest packageManifest, bool ignoreTags = true ) =>
            IsVersionHigherTo( packageManifest.version, ignoreTags );

        public bool IsVersionHigherTo( string version, bool ignoreTags = true ) =>
            Utils.VersionComparer.Compare( this.version, version, ignoreTags ) > 0;
    }
}