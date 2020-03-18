using HUFEXT.PackageManager.Editor.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;

/*  HUF Package Manifest
 *      -> Correct version string format: (MAJOR.MINOR.PATCH-PRERELEASE)
 *      -> Examples: 
 *          a) 1.9.15-preview
 *          b) 1.0.0
 */
namespace HUFEXT.PackageManager.Editor.API.Data
{
    [System.Serializable]
    public enum PackageStatus
    {
        Unknown = 0,     // Default status for new manifest instance.
        Unavailable,     // This status is used if package is on remote server but manager can't download it.
        NotInstalled,    // Package is found on remote server but is not installed in project.
        Installed,       // Package is installed and up to date.
        ForceUpdate,     // This package must be updated immediately.
        UpdateAvailable, // There is new version of package on remote server, but user can decide when he want make update.
        Migration,       // Status for packages that have different names but same path.
        Conflict,        // Status for packages that have same name but different paths.
        Embedded         // Status for packages without manifest and version data (Used for submodules in old format).
    }

    [System.Serializable]
    public class PackageManifest
    {
        private enum VersionCompareStatus
        {
            Unknown,
            Equal,
            Older,
            Newer,
            OlderThanPreview
        }

        [System.Serializable]
        public class Author
        {
            public string name = "HUF Development Team";
        }

        [System.Serializable]
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
            public string build = string.Empty;
            public string path = string.Empty;
            public bool isLocal = false;
            public bool isPreview = false;
        }

        //const string SEMVER_REGEX = @"^(?'major'0|(?:[1-9]\d*))\.(?'minor'0|(?:[1-9]\d*))\.(?'patch'0|(?:[1-9]\d*))(?:-(?'preview'(?:0|(?:[1-9A-Za-z-][0-9A-Za-z-]*))(?:\.(?:0|(?:[1-9A-Za-z-][0-9A-Za-z-]*)))*))?(?:\+(?'build'(?:0|(?:[1-9A-Za-z-][0-9A-Za-z-]*))(?:\.(?:0|(?:[1-9A-Za-z-][0-9A-Za-z-]*)))*))?$";

        public string name = string.Empty;
        public string displayName = string.Empty;
        public string description = "No detailed information's about package found.";
        public string version = "0.0.0-unknown";
        public Author author = new Author();
        public Metadata huf = new Metadata();

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

        public void ParseVersion()
        {
            if( string.IsNullOrEmpty( version ) )
            {
                version = "0.0.0-unknown";
            }

            var splitted = version.Split( '-' );
            switch( splitted.Length )
            {
                case 1:
                {
                    huf.version = splitted[0];
                    break;
                }

                case 2:
                {
                    huf.version = splitted[0];
                    huf.prerelease = splitted[1];
                    huf.isPreview = huf.prerelease == "preview";
                    break;
                }

                default: return;
            }
        }

        public void ParseConfig( PackageConfig config )
        {
            huf.config = config;
        }

        public bool IsEqual( string other )
        {
            return version == other;
        }

        public bool IsNewer( string other )
        {
            return Compare( version, other ) == VersionCompareStatus.Newer;
        }

        public bool IsOlder( string other )
        {
            return Compare( version, other ) == VersionCompareStatus.Older;
        }

        // Package can be compared in 2 ways: Ignore Preview label or not
        //  -> Ignore preview: remove preview label and compare only version numbers
        //  -> Include preview: preview packages are always older than stable, if stable version is equal to preview is marked as newer.

        // Example for comparision with preview label:
        // 1. Compare: 0.9.0-preview to 1.0.0 -> Package is older
        // 2. Compare: 0.9.0 to 1.0.0-preview -> Package is newer
        // 3. Compare: 1.0.0-preview to 1.0.0 -> Package is older
        // 4. Compare: 1.0.0-preview to 0.9.0 -> Package is newer
        // 5. Compare: 1.0.0 to 1.0.0-preview -> Package is newer

        private static VersionCompareStatus Compare( string a, string b )
        {
            if( string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b) )
            {
                return VersionCompareStatus.Unknown;
            }

            bool aIsPreview = a.Contains( "preview" );
            bool bIsPreview = b.Contains( "preview" );

            a = a.Split( '-' )[0];
            b = b.Split( '-' )[0];
            
            var result = VersionCompareStatus.Unknown;
            var an = new List<int>( Array.ConvertAll( a.Split( '.' ), int.Parse ) );
            var bn = new List<int>( Array.ConvertAll( b.Split( '.' ), int.Parse ) );

            if ( bn.Count > an.Count )
            {
                var diff = bn.Count - an.Count;
                for (int i = 0; i < diff; ++i)
                {
                    an.Add( 0 );
                }
            }

            for (var i = 0; i < an.Count; ++i)
            {
                if( an[i] == bn[i] )
                {
                    result = VersionCompareStatus.Equal;
                }
                else if ( an[i] > bn[i] )
                {
                    result = VersionCompareStatus.Newer;
                    break;
                }
                else if ( an[i] < bn[i] )
                {
                    result = VersionCompareStatus.Older;
                    break;
                }
            }

            if( ( aIsPreview && bIsPreview ) || ( !aIsPreview && !bIsPreview ) )
            {
                return result;
            }

            if( aIsPreview )
            {
                return ( result == VersionCompareStatus.Newer ) ? result : VersionCompareStatus.Older;
            }
            else if ( bIsPreview )
            {
                return VersionCompareStatus.Newer;
            }

            return VersionCompareStatus.Unknown;
        }
    }
}