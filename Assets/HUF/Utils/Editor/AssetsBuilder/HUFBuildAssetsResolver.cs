using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;

namespace HUF.Utils.Editor.BuildSupport.AssetsBuilder
{
    public class HUFBuildAssetsResolver : IPreprocessBuildWithReport
    {
        public static event Action OnBuildError;

        public const string PLUGIN_FOLDER = "Assets/Plugins/";
        public const string ANDROID_PLUGINS_FOLDER = PLUGIN_FOLDER + "Android/";
        public const string IOS_PLUGINS_FOLDER = PLUGIN_FOLDER + "iOS/";

        public const string HUF_ANDROID_ASSET_FOLDER = "Plugins/Android";
        public const string HUF_IOS_ASSET_FOLDER = "Plugins/iOS";
        public const string MANIFEST_COPY_SEARCH = "Manifest.huf";
        public const string MANIFEST_TEMPLATE_SEARCH = "ManifestTemplate.huf";

        public const string MANIFEST_FULL_NAME = "AndroidManifest.xml";
        public const string PROJECT_PROPERTY_FULL_NAME = "project.properties";
        public const string ANDROID_ADDITIONAL_MANIFEST_FOLDER_END_NAME = ".androidlib";
        public const string META_FILES = ".meta";

        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(HUFBuildAssetsResolver) );

        public int callbackOrder => -1000;

        static void OnLogMessageReceived( string condition, string stacktrace, LogType type )
        {
            if (condition.Contains( "Build completed" ) && type == LogType.Error )
            {
                OnBuildError.Dispatch();
                Application.logMessageReceived -= OnLogMessageReceived;
            }
        }

        public void OnPreprocessBuild( BuildReport report )
        {
            Application.logMessageReceived += OnLogMessageReceived;
            RunHUFBuildAssetsResolver();
        }

        [PostProcessBuild( 1000 )]
        public static void PostProcessBuildAttribute( BuildTarget target, string pathToBuiltProject )
        {
            RunHUFBuildAssetsReverter();
            Application.logMessageReceived -= OnLogMessageReceived;
        }

        [MenuItem( "HUF/Utils/Builds/Build Assets Resolver" )]
        public static void RunHUFBuildAssetsResolver()
        {
#if UNITY_ANDROID
            HUFAndroidBuildAssetsResolver();
#endif
        }

        [MenuItem( "HUF/Utils/Builds/Build Assets Revert" )]
        public static void RunHUFBuildAssetsReverter()
        {
#if UNITY_ANDROID
            HUFAndroidBuildsAssetsReverter();
#endif
        }
#if UNITY_ANDROID
        static void HUFAndroidBuildsAssetsReverter()
        {
            var paths = GetHUFManifests( MANIFEST_COPY_SEARCH );

            foreach ( string sourcePath in paths )
            {
                var pathSplit = sourcePath.Split( '/' );
                //var packageAndroidFolder = sourcePath.Replace( pathSplit[pathSplit.Length - 1], "" );
                var packageEndPath = GetAndroidManifestEndPath( sourcePath );
                FileUtil.DeleteFileOrDirectory( packageEndPath );
                FileUtil.DeleteFileOrDirectory( packageEndPath + META_FILES );
            }

            paths = GetHUFManifests( MANIFEST_TEMPLATE_SEARCH );

            foreach ( string sourcePath in paths )
            {
                var pathSplit = sourcePath.Split( '/' );
                var packageEndPath = GetAndroidManifestEndPath( sourcePath );
                FileUtil.DeleteFileOrDirectory( packageEndPath );
                FileUtil.DeleteFileOrDirectory( packageEndPath + META_FILES );
            }

            paths = GetHUFPackageToCopyFolder();

            foreach ( string source in paths )
            {
                var sourcePath = source + "ToCopy/";

                if ( !Directory.Exists( sourcePath ) )
                    continue;

                DeleteAllDirectorys( sourcePath );
            }

            /*if ( Directory.Exists( $"{ANDROID_PLUGINS_FOLDER}Firebase" ) )
                DeleteAllDirectorys( $"{ANDROID_PLUGINS_FOLDER}Firebase", false );*/
        }
        
        static void HUFAndroidBuildAssetsResolver()
        {
            var paths = GetHUFManifests( MANIFEST_COPY_SEARCH );

            foreach ( string sourcePath in paths )
            {
                var pathSplit = sourcePath.Split( '/' );
                var packageAndroidFolder = sourcePath.Replace( pathSplit[pathSplit.Length - 1], "" );
                var packageEndPath = GetAndroidManifestEndPath( sourcePath );
                FileUtil.DeleteFileOrDirectory( packageEndPath );
                Directory.CreateDirectory( packageEndPath );

                if ( !File.Exists( $"{packageEndPath}/{MANIFEST_FULL_NAME}" ) )
                    FileUtil.CopyFileOrDirectory( sourcePath, $"{packageEndPath}/{MANIFEST_FULL_NAME}" );

                if ( !File.Exists( $"{packageAndroidFolder}/{PROJECT_PROPERTY_FULL_NAME}" ) )
                    CreateProjectPropertyFile( packageEndPath );
                else
                    CopyFiles( $"{packageAndroidFolder}/{PROJECT_PROPERTY_FULL_NAME}",
                        $"{packageEndPath}/{PROJECT_PROPERTY_FULL_NAME}" );
            }

            paths = GetHUFPackageToCopyFolder();

            foreach ( string source in paths )
            {
                var sourcePath = source + "ToCopy/";

                if ( !Directory.Exists( sourcePath ) )
                    continue;

                if ( Directory.Exists( sourcePath + "res/" ) )
                    CopyFiles( sourcePath + "res/", ANDROID_PLUGINS_FOLDER + "res/" );
                var dir = new DirectoryInfo( sourcePath );
                var directories = dir.GetDirectories();

                foreach ( var directory in directories )
                {
                    if ( directory.Name == "res" )
                        continue;

                    var directoryFinalPath = $"{ANDROID_PLUGINS_FOLDER}{directory.Name}" +
                                             $"{( directory.Name != "Firebase" ? ANDROID_ADDITIONAL_MANIFEST_FOLDER_END_NAME : String.Empty )}";

                    if ( Directory.Exists( directoryFinalPath ) )
                    {
                        FileUtil.DeleteFileOrDirectory( directoryFinalPath );
                    }

                    CopyFiles( $"{sourcePath}{directory.Name}", directoryFinalPath );
                }
            }

            var firebaseCopyPath = FindSubfolderPath( "Assets/Firebase", "ToCopy" );

            if ( !firebaseCopyPath.IsNullOrEmpty() )
            {
                CopyFiles( firebaseCopyPath, ANDROID_PLUGINS_FOLDER );
            }
        }

        public static string GetAndroidManifestEndPath( string sourcePath )
        {
            var pathSplit = sourcePath.Split( '/' );

            return ANDROID_PLUGINS_FOLDER + pathSplit[pathSplit.Length - 4] +
                   ANDROID_ADDITIONAL_MANIFEST_FOLDER_END_NAME;
        }
        
        public static void CreateProjectPropertyFile( string endPath )
        {
            StreamWriter writer = new StreamWriter( endPath + "/" + PROJECT_PROPERTY_FULL_NAME, false );
            writer.WriteLine( "android.library=true" );
            writer.Close();
        }

        public static IEnumerable<string> GetHUFManifests( string manifestToSearch, string packageFolderName = "" )
        {
            var manifestFiles = AssetDatabase.FindAssets( "Manifest" );

            return manifestFiles.Select( AssetDatabase.GUIDToAssetPath )
                .Where( s =>
                    s.EndsWith( manifestToSearch ) &&
                    s.Contains( packageFolderName + "/" + HUF_ANDROID_ASSET_FOLDER ) );
        }
#endif
        
        static void DeleteAllDirectorys( string sourcePath, bool addAndroidLibraryName = true )
        {
            var dir = new DirectoryInfo( sourcePath );
            var directories = dir.GetDirectories();

            foreach ( var directory in directories )
            {
                if ( directory.Name == "res" )
                    continue;

                var directoryFinalPath = $"{ANDROID_PLUGINS_FOLDER}{directory.Name}" +
                                         $"{( addAndroidLibraryName ? ANDROID_ADDITIONAL_MANIFEST_FOLDER_END_NAME : String.Empty )}";

                if ( Directory.Exists( directoryFinalPath ) )
                {
                    FileUtil.DeleteFileOrDirectory( directoryFinalPath );
                    FileUtil.DeleteFileOrDirectory( directoryFinalPath + META_FILES );
                }
            }
        }

        static void CopyFiles( string source, string destination )
        {
            if ( !Directory.Exists( destination ) )
            {
                Directory.CreateDirectory( destination );
            }

            var dir = new DirectoryInfo( source );
            var files = dir.GetFiles();

            foreach ( var file in files )
            {
                if ( file.Name.Contains( META_FILES ) || File.Exists( $"{destination}/{file.Name}" ) )
                    continue;

                FileUtil.CopyFileOrDirectory( $"{source}/{file.Name}", $"{destination}/{file.Name}" );
            }

            var directories = dir.GetDirectories();

            foreach ( var directory in directories )
            {
                CopyFiles( $"{source}/{directory.Name}", $"{destination}/{directory.Name}" );
            }
        }

        static IEnumerable<string> GetHUFPackageToCopyFolder()
        {
            var dir = new DirectoryInfo( Application.dataPath + "/HUF" );
            var dirs = dir.GetDirectories();
            List<string> packageAssetsAndroidPath = new List<string>();

            foreach ( var directory in dirs )
            {
                var path = $"{directory}/{HUF_ANDROID_ASSET_FOLDER}";

                if ( Directory.Exists( path ) )
                {
                    packageAssetsAndroidPath.Add( path.Replace( "\\", "/" ) + "/" );
                }
            }

            return packageAssetsAndroidPath;
        }

        static string FindSubfolderPath( string parentFolder, string searchFolder )
        {
            string[] folders = AssetDatabase.GetSubFolders( parentFolder );

            string resultFolder =
                folders.FirstOrDefault( folder => ( new DirectoryInfo( folder ) ).Name == searchFolder );

            if ( string.IsNullOrEmpty( resultFolder ) && folders.Length > 0 )
            {
                string temp;

                for ( int i = 0; i < folders.Length; i++ )
                {
                    temp = FindSubfolderPath( folders[i], searchFolder );

                    if ( !string.IsNullOrEmpty( temp ) )
                    {
                        return temp;
                    }
                }
            }

            return resultFolder;
        }
    }
}