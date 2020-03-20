using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HUFEXT.PackageManager.Editor.API.Data;
using HUFEXT.PackageManager.Editor.Utils;
using HUFEXT.PackageManager.Editor.Views;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace HUFEXT.PackageManager.Editor.API.Installers
{
    [InitializeOnLoad]
    public class PackageInstaller
    {
        private const string PROGRESS_BAR_TITLE = "HUF Package Installer";
        private const string DEPENDENCIES_PATH_PATTERN = "^Assets\\/HUF*\\/{0}\\/dependencies.(txt|huf)$";
        private static readonly string PROJECT_PATH = Directory.GetParent( Application.dataPath ).FullName;

        public static bool IsLocked => PlayerPrefs.HasKey( Registry.Keys.PACKAGE_INSTALLER_LOCK );

        public static event UnityAction OnPackageInstalled;

        static PackageInstaller()
        {
            AssetDatabase.importPackageCompleted += OnImportCompleted;
            AssetDatabase.importPackageCancelled += ( x ) => Unlock();
            AssetDatabase.importPackageFailed += ( x, y ) => Unlock();
        }

        ~PackageInstaller()
        {
            AssetDatabase.importPackageCompleted -= OnImportCompleted;
        }

        public static void Lock()
        {
            PlayerPrefs.SetInt( Registry.Keys.PACKAGE_INSTALLER_LOCK, 1 );
        }

        private static void Unlock()
        {
            PlayerPrefs.DeleteKey( Registry.Keys.PACKAGE_INSTALLER_LOCK );
        }

        public static void InstallPackage( PackageManifest manifest )
        {
            Lock();
            var path = $"{Registry.Cache.CACHE_DIRECTORY}/{manifest.name}.unitypackage";

            if ( !File.Exists( path ) )
            {
                Cancel( $"[PackageManager] Unable to install package {path}." );
                return;
            }

            UpdateProgress( 0.0f, $"Removing package {manifest.name}..." );
            if ( !manifest.huf.config.overwritePackage && !string.IsNullOrEmpty( manifest.huf.path ) )
            {
                RemovePackage( manifest.huf.path, false, ".*\\.asset" );

                if ( manifest.huf.status == PackageStatus.Migration )
                {
                    // If we migrating to new package, we want to delete all files from old package.
                    ForceRemoveDirectory( $"{PROJECT_PATH}/{manifest.huf.path}" );
                }
            }
            UpdateProgress( 0.5f, $"Importing package {manifest.name}..." );
            AssetDatabase.ImportPackage( path, false );
        }

        private static void OnImportCompleted( string packageName )
        {
            var path = $"{Registry.Cache.CACHE_DIRECTORY}/{packageName}.unitypackage";
            if ( RemoveFileOrDirectory( path ) )
            {
                TryCleanFolder( Registry.Cache.CACHE_DIRECTORY );
            }
            AssetDatabase.Refresh();
            PackageManagerWindow.SetDirtyFlag();
            EditorUtility.ClearProgressBar();
            Unlock();
            OnPackageInstalled?.Invoke();
        }

        public static void RemovePackage( string path )
        {
            try
            {
                RemovePackage( path, true, ".*\\.asset" );
            }
            catch ( Exception )
            {
                EditorUtility.ClearProgressBar();
            }
            
            PackageManagerWindow.SetDirtyFlag();
        }

        private static void RemovePackage( string path, bool refreshOnComplete, params string[] excludedFilters )
        {
            if (string.IsNullOrEmpty( path ) || !Directory.Exists( path ))
            {
                return;
            }

            UpdateProgress( 0f, "Fetching dependencies..." );
            var packageDirectoryName = new DirectoryInfo( path ).Name;
            var allDependencies = FindProjectDependencies( packageDirectoryName );
            var packageDependencies = FindPackageDependencies( path, excludedFilters );

            if (packageDependencies != null)
            {
                for (var index = 0; index < packageDependencies.Length; index++)
                {
                    var filePath = packageDependencies[index];
                    if (!string.IsNullOrEmpty( filePath ) && !allDependencies.Contains( filePath ))
                    {
                        RemoveFileOrDirectory( $"{PROJECT_PATH}/{filePath}" );
                    }

                    UpdateProgress( index / (float) packageDependencies.Length,
                                    $"Removing file {filePath}" );
                }

                foreach (var filePath in packageDependencies)
                {
                    if (filePath == "-n")
                    {
                        continue;
                    }

                    var pathToFile = $"{PROJECT_PATH}/{filePath}";
                    var directoryPath = pathToFile.Substring( 0, pathToFile.LastIndexOf( '/' ) );
                    if ( Directory.Exists( directoryPath ) )
                    {
                        TryCleanFoldersHierarchy( directoryPath );
                    }
                }
            }

            if (refreshOnComplete)
            {
                AssetDatabase.Refresh();
            }

            EditorUtility.ClearProgressBar();
        }

        private static HashSet<string> FindProjectDependencies( string key )
        {
            var dependenciesGuids = AssetDatabase.FindAssets( "dependencies" );
            var otherDependencies = new HashSet<string>();
            foreach (var guid in dependenciesGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath( guid );
                if (Regex.IsMatch( path, string.Format( DEPENDENCIES_PATH_PATTERN, ".+" ), RegexOptions.Compiled ) &&
                     !Regex.IsMatch( path, string.Format( DEPENDENCIES_PATH_PATTERN, key ), RegexOptions.Compiled ))
                {
                    // Remove "Assets" string from path.
                    var absolutePath = Application.dataPath + path.Remove( 0, 6 );
                    foreach (var line in File.ReadLines( absolutePath ))
                    {
                        otherDependencies.Add( line );
                    }
                }
            }
            return otherDependencies;
        }

        private static string[] FindPackageDependencies( string path, params string[] excludedFilters )
        {
            var mainDirPath = $"{PROJECT_PATH}/{path}";
            var dependencies = $"{mainDirPath}/dependencies.txt";

            if ( !File.Exists( dependencies ) )
            {
                dependencies = $"{mainDirPath}/dependencies.huf";
            }
            
            if ( File.Exists( dependencies ) )
            {
                var filesToRemove = File.ReadAllLines( dependencies );
                if (excludedFilters.Length > 0)
                {
                    var excludedPattern = string.Join( "|", excludedFilters );
                    filesToRemove = filesToRemove
                                    .Where( q => !Regex.IsMatch( q, excludedPattern, RegexOptions.Compiled ) )
                                    .ToArray();
                }
                return filesToRemove;
            }
            else
            {
                if ( EditorUtility.DisplayDialog( "Are you sure? " + mainDirPath,
                                                  "Unable to find dependencies list. Package Manager will remove only main package directory.", 
                                                  "Yes, remove this package",
                                                  "Cancel" ) )
                {
                    ForceRemoveDirectory( mainDirPath );
                    EditorUtility.ClearProgressBar();
                }
            }

            return null;
        }

        private static void TryCleanFoldersHierarchy( string path )
        {
            if ( TryCleanFolder( path ) )
            {
                var index = path.LastIndexOf( '/' );
                if ( index >= 0 )
                {
                    var nextPath = path.Substring( 0, index );
                    if ( !nextPath.EndsWith( "HUF/Assets" ) )
                    {
                        TryCleanFoldersHierarchy( nextPath );
                    }
                }
            }
        }

        private static bool TryCleanFolder( string path )
        {
            var files = new DirectoryInfo( path ).GetFiles();
            if ( files.Length == 0 && RemoveFileOrDirectory( path ) )
            {
                AssetDatabase.Refresh();
                return true;
            }
            return false;
        }

        private static bool RemoveFileOrDirectory( string path )
        {
            if( FileUtil.DeleteFileOrDirectory( path ) )
            {
                FileUtil.DeleteFileOrDirectory( $"{path}.meta" );
                return true;
            }
            return false;
        }

        private static void UpdateProgress( float progress, string message = "Installing package" )
        {
            EditorUtility.DisplayProgressBar( PROGRESS_BAR_TITLE, message, progress );
        }

        private static void Cancel( string message )
        {
            Debug.LogError( message );
            EditorUtility.ClearProgressBar();
        }

        private static void ForceRemoveDirectory( string path )
        {
            if ( Directory.Exists( path ) )
            {
                Directory.Delete( path, true );
                AssetDatabase.Refresh();
            }
        }
    }
}
