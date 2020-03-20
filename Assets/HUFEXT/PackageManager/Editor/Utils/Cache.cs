using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Utils
{
    public static class Cache
    {
        public enum Policy
        {
            File = 0,
            PlayerPrefs,
            EncodedFile,
            EncodedPrefs
        }

        [Serializable]
        private class Entry
        {
            public string path;
            public Policy policy;
        }

        [Serializable]
        private class CacheEntries
        {
            public List<Entry> entries = new List<Entry>();
        }

        public static bool Save<T>( string path, T obj, Policy policy = Policy.PlayerPrefs )
        {
            switch (policy)
            {
                case Policy.File: return SaveAsFile( path, obj, false );
                case Policy.PlayerPrefs: return SaveAsPrefs( path, obj, false );
                case Policy.EncodedFile: return SaveAsFile( path, obj, true );
                case Policy.EncodedPrefs: return SaveAsPrefs( path, obj, true );
            }
            return false;
        }

        public static bool SaveInCache<T>( string path, T obj, Policy policy = Policy.PlayerPrefs )
        {
            return Save( $"{Registry.Cache.CACHE_DIRECTORY}/{path}", obj, policy );
        }

        public static bool Load<T>( T obj, string path, Policy policy = Policy.PlayerPrefs )
        {
            switch (policy)
            {
                case Policy.File: return LoadFromFile( obj, path, false );
                case Policy.PlayerPrefs: return LoadFromPrefs( obj, path, false );
                case Policy.EncodedFile: return LoadFromFile( obj, path, true );
                case Policy.EncodedPrefs: return LoadFromPrefs( obj, path, true );
            }
            return false;
        }

        public static bool LoadFromCache<T>( T obj, string path, Policy policy = Policy.PlayerPrefs )
        {
            return Load( obj, $"{Registry.Cache.CACHE_DIRECTORY}/{path}", policy );
        }

        public static void Remove( string path, Policy policy = Policy.PlayerPrefs )
        {
            RemoveCache( path, policy );
            RemoveCacheEntry( path, policy );
        }

        public static void RemoveFromCache( string path, Policy policy = Policy.PlayerPrefs )
        {
            Remove( $"{Registry.Cache.CACHE_DIRECTORY}/{path}", policy );
        }

        static bool SaveAsPrefs<T>( string path, T obj, bool encode )
        {
            var data = encode ? Encode( EditorJsonUtility.ToJson( obj ) ) : EditorJsonUtility.ToJson( obj );
            PlayerPrefs.SetString( path, data );
            CacheEntry( path, encode ? Policy.EncodedPrefs : Policy.PlayerPrefs );
            return PlayerPrefs.HasKey( path );
        }
        
        static bool SaveAsFile<T>( string path, T obj, bool encode )
        {
            var cacheDirectory = Path.GetDirectoryName( path );
            if ( cacheDirectory != null && !Directory.Exists( cacheDirectory ) )
            {
                Directory.CreateDirectory( cacheDirectory );
            }

            var data = encode ? Encode( EditorJsonUtility.ToJson( obj ) ) : EditorJsonUtility.ToJson( obj );
            File.WriteAllText( path, data );
            CacheEntry( path, encode ? Policy.EncodedFile : Policy.File );
            return File.Exists( path );
        }

        static bool LoadFromPrefs<T>( T obj, string path, bool decode )
        {
            if ( !PlayerPrefs.HasKey( path ) )
            {
                return false;
            }
            
            var data = PlayerPrefs.GetString( path );
            if ( !string.IsNullOrEmpty( data ) )
            {
                var plainData = decode ? Decode( data ) : data;
                EditorJsonUtility.FromJsonOverwrite( plainData, obj );
                return true;
            }

            return false;
        }
        
        static bool LoadFromFile<T>( T obj, string path, bool decode )
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

        static CacheEntries GetEntries()
        {
            var entries = new CacheEntries();
            if (PlayerPrefs.HasKey( Registry.Keys.CACHE_ENTRIES_KEY ))
            {
                EditorJsonUtility.FromJsonOverwrite( PlayerPrefs.GetString( Registry.Keys.CACHE_ENTRIES_KEY ),
                                                     entries );
            }

            return entries;
        }

        static void CacheEntry( string path, Policy policy )
        {
            var entries = GetEntries();
            var entry = new Entry() { path = path, policy = policy };

            if ( !entries.entries.Contains( entry ))
            {
                entries.entries.Add( new Entry() { path = path, policy = policy } );
                var json = EditorJsonUtility.ToJson( entries );
                PlayerPrefs.SetString( Registry.Keys.CACHE_ENTRIES_KEY, EditorJsonUtility.ToJson( entries ) );
            }
        }

        static void RemoveCacheEntry( string path, Policy policy )
        {
            if ( !PlayerPrefs.HasKey( Registry.Keys.CACHE_ENTRIES_KEY ) )
            {
                return;
            }
            var entries = GetEntries();
            entries.entries.RemoveAll( ( x ) => x.path == path && x.policy == policy );

            if ( Directory.Exists( Registry.Cache.CACHE_DIRECTORY ) )
            {
                var files = new DirectoryInfo( Registry.Cache.CACHE_DIRECTORY ).GetFiles();
                if ( files.Length == 0 )
                {
                    FileUtil.DeleteFileOrDirectory( Registry.Cache.CACHE_DIRECTORY );
                }
            }

            PlayerPrefs.SetString( Registry.Keys.CACHE_ENTRIES_KEY, EditorJsonUtility.ToJson( entries ) );
        }

        static void RemoveCache( string path, Policy policy )
        {
            switch ( policy )
            {
                case Policy.File:
                case Policy.EncodedFile:
                {
                    if ( File.Exists( path ) )
                    {
                        File.Delete( path );
                        File.Delete( $"{path}.meta" );
                    }
                    break;
                }

                case Policy.PlayerPrefs:
                case Policy.EncodedPrefs:
                {
                    PlayerPrefs.DeleteKey( path );
                    break;
                }
            }
        }

        static string Encode( string data )
        {
            var encoded = System.Text.Encoding.UTF8.GetBytes( data );
            return Convert.ToBase64String( encoded );
        }

        static string Decode( string data )
        {
            if (IsBase64String( data ))
            {
                var decoded = Convert.FromBase64String( data );
                return System.Text.Encoding.UTF8.GetString( decoded );
            }
            else return string.Empty;
        }

        static bool IsBase64String( this string s )
        {
            s = s.Trim();
            return ( s.Length % 4 == 0 ) && Regex.IsMatch( s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None );
        }

        public static void Clear()
        {
            if (!PlayerPrefs.HasKey( Registry.Keys.CACHE_ENTRIES_KEY ))
            {
                return;
            }

            var entries = new CacheEntries();
            EditorJsonUtility.FromJsonOverwrite( PlayerPrefs.GetString( Registry.Keys.CACHE_ENTRIES_KEY ),
                                                 entries );

            foreach (var entry in entries.entries)
            {
                RemoveCache( entry.path, entry.policy );
            }

            PlayerPrefs.DeleteKey( Registry.Keys.CACHE_ENTRIES_KEY );
        }
    }
}
