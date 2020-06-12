using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace HUF.Utils.Editor.ProGuard
{
    public class ProGuardBuilder : IPreprocessBuildWithReport
    {
        const string RESULT_FILE = "Assets/Plugins/Android/proguard-user.txt";
        const string GENERATED_CONTENT_PREFIX_START = "### HUF generated start ###################################";
        const string GENERATED_CONTENT_PREFIX_END = "### HUF generated end ###################################";

        public int callbackOrder => -1001;

        public void OnPreprocessBuild( BuildReport report )
        {
            Resolve();
        }

        [MenuItem( "HUF/Tools/Proguard Merger" )]
        static void Resolve()
        {
            var resultFile = AssetDatabase.LoadAssetAtPath<TextAsset>( RESULT_FILE );

            if ( resultFile == null )
            {
                Debug.LogWarning( $"There is no proguard file in location: {RESULT_FILE}" );
                return;
            }

            var paths = GetProguardFilesPaths();
            var stringBuilder = new StringBuilder();
            var prefixContent = GetPrefixContentFromProguardFile( resultFile );
            var postfixContent = GetPostfixContentFromProguardFile( resultFile );
            stringBuilder.Append( prefixContent );
            stringBuilder.Append( $"\n{GENERATED_CONTENT_PREFIX_START}\n" );

            foreach ( string path in paths )
            {
                var file = AssetDatabase.LoadAssetAtPath<TextAsset>( path );
                Debug.Assert( file != null, $"Cannot find file at path: {path}" );
                stringBuilder.Append( $"\n#start of {path}\n" );
                stringBuilder.Append( file.text );
                stringBuilder.Append( $"\n#end of {path}\n" );
            }

            stringBuilder.Append( $"\n{GENERATED_CONTENT_PREFIX_END}\n" );
            stringBuilder.Append( postfixContent );
            File.WriteAllText( AssetDatabase.GetAssetPath( resultFile ), stringBuilder.ToString() );
            EditorUtility.SetDirty( resultFile );
        }

        static string GetPrefixContentFromProguardFile( TextAsset file )
        {
            int index = file.text.IndexOf( GENERATED_CONTENT_PREFIX_START, StringComparison.CurrentCultureIgnoreCase );

            if ( index < 0 )
            {
                return file.text;
            }

            return file.text.Substring( 0, index - 1 );
        }

        static string GetPostfixContentFromProguardFile( TextAsset file )
        {
            int index = file.text.IndexOf( GENERATED_CONTENT_PREFIX_END, StringComparison.CurrentCultureIgnoreCase );

            if ( index < 0 )
            {
                return string.Empty;
            }

            index++;

            index += GENERATED_CONTENT_PREFIX_END.Length;
            return file.text.Substring( index, file.text.Length - index );
        }

        static IEnumerable<string> GetProguardFilesPaths()
        {
            var proguardFiles = AssetDatabase.FindAssets( "proguard t:TextAsset" );

            return proguardFiles.Select( AssetDatabase.GUIDToAssetPath )
                .Where( s => s.EndsWith( "txt" ) && !s.Contains( RESULT_FILE ) );
        }
    }
}