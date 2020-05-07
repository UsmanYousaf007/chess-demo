using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Messaging;
using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Utils
{
    internal static class Common
    {
        static readonly string LOG_PREFIX = "<color=\"#E40521\"><b>[{0}]</b></color> <color=\"#c77700\"><b>{1}</b></color> {2}";
        
        [Serializable]
        internal class Wrapper<T>
        {
            public List<T> Items = new List<T>();
        }

        internal static int GetTimestamp( int delay = 0 )
        {
            return ( int ) DateTime.UtcNow.AddSeconds( delay ).Subtract( new DateTime( 1970, 1, 1 ) ).TotalSeconds;
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
                EditorJsonUtility.FromJsonOverwrite( "{ \"Items\": " + json + "}", wrapper );
            }
            return wrapper.Items;
        }

        internal static string FromArrayToJson<T>( List<T> array )
        {
            var wrapper = new Wrapper<T> { Items = array };
            return EditorJsonUtility.ToJson( wrapper );
        }

        internal static void Log( string message )
        {
            if ( Core.Registry.IsSet( Models.Keys.PACKAGE_MANAGER_DEBUG_LOGS ) )
            {
                Debug.Log( string.Format( LOG_PREFIX, "HUF", "Common", message ) );
            }
        }
        
        internal static void Log( this Core.Command.Base command, string message )
        {
            if ( Core.Registry.IsSet( Models.Keys.PACKAGE_MANAGER_DEBUG_LOGS ) )
            {
                Debug.Log( string.Format( LOG_PREFIX, "HUF", command.GetType().Name, message ) );
            }
        }

        internal static void ShowDownloadProgress( string name, float progress = 0f )
        {
            EditorUtility.DisplayProgressBar( "Downloading", "Downloading " + name, progress );
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
    }
}
