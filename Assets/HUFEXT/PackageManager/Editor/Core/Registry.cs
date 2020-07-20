using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Core
{
    public enum CachePolicy
    {
        File               = 0,
        Prefs              = 1,
        EditorPrefs        = 4,
        EncodedFile        = 2,
        EncodedPrefs       = 3,
        EncodedEditorPrefs = 5
    }

    // Keys are used to keep data in prefs.
    // Flags are used for indicate status of something (for example if force close flag is set, editor window will be closed).
    internal static class Registry
    {
        [Serializable]
        public class CacheData : Utils.Common.Wrapper<string> {}
        
        static CacheData CachedData
        {
            get
            {
                var data = new CacheData();
                if ( PlayerPrefs.HasKey( Models.Keys.CACHE_DATA_KEY ) )
                {
                    EditorJsonUtility.FromJsonOverwrite( PlayerPrefs.GetString( Models.Keys.CACHE_DATA_KEY ), data );
                }
                return data;
            }

            set
            {
                PlayerPrefs.SetString( Models.Keys.CACHE_DATA_KEY, EditorJsonUtility.ToJson( value ) );
                PlayerPrefs.Save();
            }
        }

        static void RegisterPath( string path )
        {
            var data = CachedData;
            if ( !data.Items.Contains( path ) )
            {
                data.Items.Add( path );
                CachedData = data;
            }
        }

        public static bool IsRegistered( string path )
        {
            return CachedData.Items.Exists( ( item ) => item == path );
        }

        static void UnregisterPath( string path )
        {
            var data = CachedData;
            if ( data.Items.Remove( path ) )
            {
                CachedData = data;
            }
        }

        static string Encode( string data )
        {
            return Convert.ToBase64String( System.Text.Encoding.UTF8.GetBytes( data ) );
        }

        static string Decode( string data )
        {
            try
            {
                return System.Text.Encoding.UTF8.GetString( Convert.FromBase64String( data ) );
            }
            catch ( Exception )
            {
                return string.Empty;
            }
        }

        static void RemoveCacheDirectory()
        {
            if ( Directory.Exists( Models.Keys.CACHE_DIRECTORY ) &&
                 new DirectoryInfo( Models.Keys.CACHE_DIRECTORY ).GetFiles().Length == 0)
            {
                FileUtil.DeleteFileOrDirectory( Models.Keys.CACHE_DIRECTORY );
            }
        }

        static void RemoveInternal( string path )
        {
            if ( PlayerPrefs.HasKey( path ) )
            {
                PlayerPrefs.DeleteKey( path );
            }
            else if ( EditorPrefs.HasKey( path ) )
            {
                EditorPrefs.DeleteKey( path );
            }
            else if ( File.Exists( path ) )
            {
                File.Delete( path );
                if ( File.Exists( $"{path}.meta" ) )
                {
                    File.Delete( $"{path}.meta" );
                }
                RemoveCacheDirectory();
            }
        }

        public static void Clear()
        {
            if ( !PlayerPrefs.HasKey( Models.Keys.CACHE_DATA_KEY ) )
            {
                return;
            }

            foreach ( var item in CachedData.Items )
            {
                RemoveInternal( item );
            }
            PlayerPrefs.DeleteKey( Models.Keys.CACHE_DATA_KEY );
        }
        
        static bool SaveAsPrefs<T>( string path, T obj, bool encode, bool useEditor = false ) where T : class
        {
            var data = encode ? Encode( EditorJsonUtility.ToJson( obj ) ) : EditorJsonUtility.ToJson( obj );
            if ( useEditor )
            {
                EditorPrefs.SetString( path, data );
            }
            else
            {
                PlayerPrefs.SetString( path, data );
                PlayerPrefs.Save();
            }
            RegisterPath( path );
            return useEditor ? EditorPrefs.HasKey( path ) : PlayerPrefs.HasKey( path );
        }

        static bool LoadFromPrefs<T>( string path, T obj, bool decode, bool useEditor = false ) where T : class
        {
            var data = useEditor ? EditorPrefs.GetString( path, string.Empty ) : 
                           PlayerPrefs.GetString( path, string.Empty );
            
            if ( !string.IsNullOrEmpty( data ) )
            {
                var plainData = decode ? Decode( data ) : data;
                EditorJsonUtility.FromJsonOverwrite( plainData, obj );
                return true;
            }
            return false;
        }

        static bool SaveAsFile<T>( string path, T obj, bool encode ) where T : class
        {
            var cacheDirectory = Path.GetDirectoryName( path );
            if ( cacheDirectory != null && !Directory.Exists( cacheDirectory ) )
            {
                Directory.CreateDirectory( cacheDirectory );
            }

            var data = encode ? Encode( EditorJsonUtility.ToJson( obj ) ) : EditorJsonUtility.ToJson( obj );
            File.WriteAllText( path, data );
            RegisterPath( path );
            return File.Exists( path );
        }

        static bool LoadFromFile<T>( string path, T obj, bool decode ) where T : class
        {
            if ( File.Exists( path ) )
            {
                var data = File.ReadAllText( path );
                if ( !string.IsNullOrEmpty( data ) )
                {
                    var plainData = decode ? Decode( data ) : data;
                    EditorJsonUtility.FromJsonOverwrite( plainData, obj );
                    return true;
                }
            }
            return false;
        }

        public static bool Save<T>( string path, T obj, CachePolicy policy ) where T : class
        {
            switch ( policy )
            {
                case CachePolicy.File: return SaveAsFile( path, obj, false );
                case CachePolicy.Prefs: return SaveAsPrefs( path, obj, false );
                case CachePolicy.EditorPrefs: return SaveAsPrefs( path, obj, false, true );
                case CachePolicy.EncodedFile: return SaveAsFile( path, obj, true );
                case CachePolicy.EncodedPrefs: return SaveAsPrefs( path, obj, true );
                case CachePolicy.EncodedEditorPrefs: return SaveAsPrefs( path, obj, true, true );
                default: return false;
            }
        }

        public static bool Save( string path, int value )
        {
            PlayerPrefs.SetInt( path, value );
            PlayerPrefs.Save();
            RegisterPath( path );
            return PlayerPrefs.HasKey( path );
        }
        
        public static bool Save( string path, string value )
        {
            PlayerPrefs.SetString( path, value );
            PlayerPrefs.Save();
            RegisterPath( path );
            return PlayerPrefs.HasKey( path );
        }
        
        public static bool Load<T>( string path, T obj, CachePolicy policy ) where T : class
        {
            switch ( policy )
            {
                case CachePolicy.File: return LoadFromFile( path, obj, false );
                case CachePolicy.Prefs: return LoadFromPrefs( path, obj, false );
                case CachePolicy.EditorPrefs: return LoadFromPrefs( path, obj, false, true );
                case CachePolicy.EncodedFile: return LoadFromFile( path, obj, true );
                case CachePolicy.EncodedPrefs: return LoadFromPrefs( path, obj, true );
                case CachePolicy.EncodedEditorPrefs: return LoadFromPrefs( path, obj, true, true );
                default: return false;
            }
        }

        public static bool Load( string path, out int value )
        {
            value = PlayerPrefs.GetInt( path, 0 );
            return PlayerPrefs.HasKey( path );
        }
        
        public static bool Load( string path, out string value )
        {
            value = PlayerPrefs.GetString( path, string.Empty );
            return PlayerPrefs.HasKey( path );
        }

        public static T Get<T>( string path, CachePolicy policy ) where T : class, new()
        {
            var obj = new T();
            Load( path, obj, policy );
            return obj;
        }
        
        public static void Remove( string path )
        {
            RemoveInternal( path );
            UnregisterPath( path );
        }

        public static void ClearCache()
        {     
            if ( Directory.Exists( Models.Keys.CACHE_DIRECTORY ) )
            {
                Directory.Delete( Models.Keys.CACHE_DIRECTORY, true );
            }
        }
        
        public static bool SaveInCache<T>( string path, T obj, bool encode = false ) where T : class 
        {
            return Save( $"{Models.Keys.CACHE_DIRECTORY}/{path}", obj, encode ? CachePolicy.EncodedFile : CachePolicy.File );
        }

        public static bool LoadFromCache<T>( T obj, string path, bool decode = false ) where T : class
        {
            return Load( $"{Models.Keys.CACHE_DIRECTORY}/{path}", obj, decode ? CachePolicy.EncodedFile : CachePolicy.File );
        }
        
        public static void RemoveFromCache( string path )
        {
            Remove( $"{Models.Keys.CACHE_DIRECTORY}/{path}" );
        }

        public static void Push( string key )
        {
            PlayerPrefs.SetInt( key, 1 );
            PlayerPrefs.Save();
            RegisterPath( key );
        }

        public static bool Pop( string key )
        {
            if ( !PlayerPrefs.HasKey( key ) )
            {
                return false;
            }
            
            Remove( key );
            return true;
        }

        public static bool IsSet( string key ) => PlayerPrefs.HasKey( key );
    }
}