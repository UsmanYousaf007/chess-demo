using System.Collections.Generic;
using HUF.Utils.Runtime.Logging;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Presets;
#endif

namespace HUF.Utils.Runtime.Configs.Implementation
{
    public static class ConfigUtils
    {
        const string HUF_FOLDER_PREFIX = "Assets/HUF";

        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(ConfigUtils) );

#if UNITY_EDITOR
        public static void OverlayPreset( Object target )
        {
            string className = target.GetType().Name;
            var presets = AssetDatabase.FindAssets( $"{className} t:preset" );

            if ( presets.Length == 0 )
            {
                HLog.Log( logPrefix, $"No presets found for {className}." );
                return;
            }

            string path = AssetDatabase.GUIDToAssetPath( presets[0] );

            if ( presets.Length > 1 && !path.StartsWith( HUF_FOLDER_PREFIX ) )
            {
                for ( int i = 1; i < presets.Length; i++ )
                {
                    string tempPath = AssetDatabase.GUIDToAssetPath( presets[i] );

                    if ( !tempPath.StartsWith( HUF_FOLDER_PREFIX ) )
                        continue;

                    path = tempPath;
                    break;
                }
            }

            var preset = AssetDatabase.LoadAssetAtPath<Preset>( path );

            if ( preset.CanBeAppliedTo( target ) )
            {
                List<string> properties = new List<string>();

                foreach ( var modification in preset.PropertyModifications )
                {
                    if ( string.IsNullOrEmpty( modification.value ) )
                        continue;

                    properties.Add( modification.propertyPath );
                }

                preset.ApplyTo( target, properties.ToArray() );
                HLog.Log( logPrefix, $"Applied preset to {className}: {path}" );
            }
            else
                HLog.LogError( logPrefix, $"Conflicting preset at {path}. Unable to apply." );
        }
#endif
    }
}
