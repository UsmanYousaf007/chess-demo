using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace HUFEXT.PackageManager.Editor.Models
{
    [Serializable]
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
        Embedded,        // Status for packages without manifest and version data (Used for submodules in old format).
        Development,     // Status for packages in development.
        Git,             // Status for repositories with package structure.
        GitUpdate,       // Status for repositories with not updated package version.
        GitError         // Status for repositories with old package version.
    }

    public static class Rollout
    {
        public const string NOT_HUF_LABEL       = "Not HUF";
        public const string VCS_LABEL           = "Version Control";
        public const string EXPERIMENTAL_LABEL  = "Experimental";
        public const string DEVELOPMENT_LABEL   = "Development";
        public const string NOT_INSTALLED_LABEL = "Not Installed";
        public const string UNITY_LABEL         = "Unity Packages";
        public const string UNDEFINED_LABEL     = "Undefined";
    }
    
    [Serializable]
    public class PackageManifest
    {
        public static readonly string PREVIEW_TAG = "preview";
        public static readonly string EXPERIMENTAL_TAG = "experimental";
        
        [Serializable]
        public class Author
        {
            public string name = "HUF Development Team";
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
            public bool isPreview = false;
            public bool isUnity = false;
            public List<string> dependencies = new List<string>();
            public List<string> exclude = new List<string>();
            public List<string> details = new List<string>();
        }
        
        public string name = string.Empty;
        public string displayName = string.Empty;
        public string description = "No detailed information's about package found.";
        public string version = "0.0.0-unknown";
        public Author author = new Author();
        public Metadata huf = new Metadata();

        public bool IsInstalled => huf.status == PackageStatus.Installed || 
                                   huf.status == PackageStatus.UpdateAvailable || 
                                   huf.status == PackageStatus.ForceUpdate;

        public bool IsUpdate => huf.status == PackageStatus.UpdateAvailable ||
                                huf.status == PackageStatus.ForceUpdate;
        
        public bool IsRepository => huf.status == PackageStatus.Git || 
                                    huf.status == PackageStatus.GitUpdate || 
                                    huf.status == PackageStatus.GitError;
        
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
        
        public void ParseVersion()
        {
            if( string.IsNullOrEmpty( version ) )
            {
                version = "0.0.0-unknown";
            }

            var parts = version.Split( '-' );
            switch( parts.Length )
            {
                case 1: { huf.version = parts[0]; break; }
                case 2:
                {
                    huf.version = parts[0];
                    huf.prerelease = parts[1];
                    huf.isPreview = huf.prerelease == PREVIEW_TAG;
                    break;
                }
                default: return;
            }
        }
    }
}