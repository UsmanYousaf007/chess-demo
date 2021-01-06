using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Utils
{
    internal static class Common
    {
        const string HUF_PACKAGE_MANAGER = "HUF PACKAGE MANAGER";
        const string COMMON = "Common";
        const string LOG_PREFIX = "<color=\"#E40521\"><b>[{0}]</b></color> <color=\"#c77700\"><b>{1}</b></color> {2}";

        const string LOG_ERROR_PREFIX =
            "<color=\"#E40521\"><b>[{0}]</b></color> <color=\"#c77700\"><b>{1}</b></color>  <color=\"#ff2222\">{2}</color> ";

        const string DOWNLOADING = "Downloading";

        [Serializable]
        internal class Wrapper<T>
        {
            public List<T> Items = new List<T>();
        }

        internal static int GetTimestamp( int delay = 0 )
        {
            return (int)DateTime.UtcNow.AddSeconds( delay ).Subtract( new DateTime( 1970, 1, 1 ) ).TotalSeconds;
        }

        internal static List<T> FromJsonToArray<T>( string json )
        {
            var wrapper = new Wrapper<T>();

            if ( json.Contains( "Items" ) )
            {
                EditorJsonUtility.FromJsonOverwrite( json, wrapper );
            }
            else
            {
                EditorJsonUtility.FromJsonOverwrite( $"{{ \"Items\": {json}}}", wrapper );
            }

            return wrapper.Items;
        }

        internal static string FromListToJson<T>( List<T> array )
        {
            var wrapper = new Wrapper<T> {Items = array};
            return EditorJsonUtility.ToJson( wrapper );
        }

        internal static void Log( string message )
        {
            if ( Core.Registry.IsSet( Models.Keys.PACKAGE_MANAGER_DEBUG_LOGS ) )
            {
                Debug.Log( string.Format( LOG_PREFIX, HUF_PACKAGE_MANAGER, COMMON, message ) );
            }
        }

        internal static void Log( this Core.Command.Base command, string message )
        {
            if ( Core.Registry.IsSet( Models.Keys.PACKAGE_MANAGER_DEBUG_LOGS ) )
            {
                Debug.Log( string.Format( LOG_PREFIX, HUF_PACKAGE_MANAGER, command.GetType().Name, message ) );
            }
        }

        internal static void LogError( string message, string title = "" )
        {
            Debug.LogError( string.Format( LOG_ERROR_PREFIX, HUF_PACKAGE_MANAGER, title == string.Empty ? COMMON : title, message ) );
        }

        internal static void ShowDownloadProgress( string name, float progress = 0f )
        {
            EditorUtility.DisplayProgressBar( DOWNLOADING, $"Downloading {name}", progress );
        }

        internal static string GetPackagePath( string packageName )
        {
            return Path.Combine( Models.Keys.CACHE_DIRECTORY,
                $"{packageName}{Models.Keys.Filesystem.UNITY_PACKAGE_EXTENSION}" );
        }

        internal static string GetMetaPath( string path )
        {
            return $"{path}{Models.Keys.Filesystem.META_EXTENSION}";
        }

        internal static void RebuildDefines()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach ( var assembly in assemblies )
            {
                var types = assembly.GetTypes();

                foreach ( var type in types )
                {
                    if ( type.Name == "DefineCollector" )
                    {
                        type.GetMethod( "RebuildDefines", BindingFlags.Public | BindingFlags.Static )
                            .Invoke( null, null );
                    }
                }
            }
        }
    }
}