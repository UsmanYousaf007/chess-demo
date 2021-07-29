#if UNITY_2020_3_OR_NEWER
using System.IO;
using HUF.Utils.Editor.BuildSupport.AssetsBuilder;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace HUF.Utils.Editor.BuildSupport
{
    public class Unity2020BuildFixer
    {
        const string ANDROID_PLUGINS_FOLDER = HUFBuildAssetsResolver.PLUGIN_FOLDER + "Android";

        const string LAUNCHER_TEMPLATE_FILENAME = "launcherTemplate.gradle";
        const string MAIN_TEMPLATE_FILENAME = "mainTemplate.gradle";
        const string PHRASE_TO_REPLACE = "**STREAMING_ASSETS**]";
        const string REPLACE_VALUE = "] + unityStreamingAssets.tokenize(', ')";
        
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(Unity2020BuildFixer) );

        [MenuItem( "HUF/Unity/2020.3+/FixMyProject" )]
        public static void FixUnity2020Lts()
        {
            Unity2019BuildFixer.FixUnity2019_4_LTS();
            ReplaceFileContents(ANDROID_PLUGINS_FOLDER, MAIN_TEMPLATE_FILENAME);
            ReplaceFileContents(ANDROID_PLUGINS_FOLDER, LAUNCHER_TEMPLATE_FILENAME);

            HLog.Log( logPrefix, $"Fixing finished." );
            AssetDatabase.Refresh( ImportAssetOptions.ForceUpdate );
        }

        static void ReplaceFileContents(string folder, string filename)
        {
            var filePath = Path.Combine( folder, filename );

            if ( !File.Exists( filePath ) )
            {
                HLog.LogWarning( logPrefix, $"File {filePath} doesn't exist, skipping" );
                return;
            }

            var contents = File.ReadAllText( filePath );
            contents = contents.Replace( PHRASE_TO_REPLACE, REPLACE_VALUE );
            File.WriteAllText( filePath, contents );
        }
    }
}
#endif