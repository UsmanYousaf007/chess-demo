using System.Collections.Generic;
using System.IO;
using System.Linq;
using HUF.Utils.Runtime.Logging;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace HUF.Utils.Editor.AssetsBuilder
{
    public class HUFBuildAssetsResolver : IPreprocessBuildWithReport
    {
        const string PLUGIN_FOLDER = "Assets/Plugins/";
        const string ANDROID_PLUGINS_FOLDER = PLUGIN_FOLDER + "Android/";
        const string IOS_PLUGINS_FOLDER = PLUGIN_FOLDER + "iOS/";

        const string HUF_ANDROID_ASSET_FOLDER = "Assets/Android";
        const string HUF_IOS_ASSET_FOLDER = "Assets/iOS/";
        const string MANIFEST_COPY_SEARCH = "Manifest.huf";
        public const string MANIFEST_TEMPLATE_SEARCH = "ManifestTemplate.huf";

        public const string MANIFEST_FULL_NAME = "AndroidManifest.xml";
        const string PROJECT_PROPERTY_FULL_NAME = "project.properties";
        const string ANDROID_ADDITIONAL_MANIFEST_FOLDER_END_NAME = ".androidlib";

        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(HUFBuildAssetsResolver) );

        public int callbackOrder => -1000;

        public void OnPreprocessBuild( BuildReport report )
        {
            RunHUFBuildAssetsResolver();
        }
        
        [MenuItem( "HUF/Tools/Build Assets Resolver" )]
        public static void RunHUFBuildAssetsResolver()
        {
#if UNITY_ANDROID
            HUFAndroidBuildAssetsResolver();
#endif
        }

        static void HUFAndroidBuildAssetsResolver()
        {
            /*var mainManifest = AssetDatabase.LoadAssetAtPath<TextAsset>( ANDROID_PLUGINS_FOLDER + MANIFEST_FULL_NAME );

            if ( mainManifest == null )
            {
                HLog.LogError( logPrefix, $"You are missing AndroidManifest.xml in: {PLUGIN_FOLDER}" );
            }*/

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
                    FileUtil.CopyFileOrDirectory( $"{packageAndroidFolder}/{PROJECT_PROPERTY_FULL_NAME}",
                        $"{packageEndPath}/{PROJECT_PROPERTY_FULL_NAME}" );
                HLog.Log( logPrefix, $"Android Manifest Copy from {sourcePath} to {packageEndPath}" );
            }

            paths = GetHUFPackageAndroidAssetsFolder();

            foreach ( string sourcePath in paths )
            {
                if ( Directory.Exists( sourcePath + "res/" ) )
                    CopyAndroidResources( sourcePath + "res/", ANDROID_PLUGINS_FOLDER + "res/" );
                var dir = new DirectoryInfo( sourcePath );
                var directories = dir.GetDirectories();
                
                foreach ( var directory in directories )
                {
                    if ( directory.Name == "res" )
                        continue;

                    var directoryFinalPath = $"{ANDROID_PLUGINS_FOLDER}/{directory.Name}{ANDROID_ADDITIONAL_MANIFEST_FOLDER_END_NAME}";

                    if ( Directory.Exists( directoryFinalPath ) )
                    {
                        FileUtil.DeleteFileOrDirectory( directoryFinalPath );
                    }

                    FileUtil.CopyFileOrDirectory( $"{sourcePath}{directory.Name}", directoryFinalPath );
                }
            }
        }

        public static string GetAndroidManifestEndPath( string sourcePath )
        {
            var pathSplit = sourcePath.Split( '/' );

            return ANDROID_PLUGINS_FOLDER + pathSplit[pathSplit.Length - 4] +
                   ANDROID_ADDITIONAL_MANIFEST_FOLDER_END_NAME;
        }

        static void CopyAndroidResources( string source, string destination )
        {
            if ( Directory.Exists( destination ) )
            {
                var dir = new DirectoryInfo( source );
                var files = dir.GetFiles();

                foreach ( var file in files )
                {
                    if ( file.Name.Contains( ".meta" ) || File.Exists( $"{destination}/{file.Name}" ) )
                        continue;

                    FileUtil.CopyFileOrDirectory( $"{source}/{file.Name}", $"{destination}/{file.Name}" );
                }

                var directories = dir.GetDirectories();

                foreach ( var directory in directories )
                {
                    CopyAndroidResources( $"{source}{directory.Name}", $"{destination}{directory.Name}" );
                }
            }
            else
            {
                FileUtil.CopyFileOrDirectory( source, destination );
            }
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

        static IEnumerable<string> GetHUFPackageAndroidAssetsFolder()
        {
            var dir = new DirectoryInfo( Application.dataPath + "/HUF");
            var dirs = dir.GetDirectories();
            
            List<string> packageAssetsAndroidPath = new List<string>();

            foreach ( var directory in dirs )
            {
                var path = $"{directory}/{HUF_ANDROID_ASSET_FOLDER}";

                if ( Directory.Exists( path ) )
                {
                    packageAssetsAndroidPath.Add( path.Replace( "\\","/" ) +"/" );
                }
            }

            return packageAssetsAndroidPath;
        }
    }
}