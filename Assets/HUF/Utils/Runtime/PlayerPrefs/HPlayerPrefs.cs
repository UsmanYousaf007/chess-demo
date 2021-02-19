using JetBrains.Annotations;
using UnityEngine;
using Prefs = UnityEngine.PlayerPrefs;

namespace HUF.Utils.Runtime.PlayerPrefs
{
    public static class HPlayerPrefs
    {
        /// <summary>
        /// Returns if PlayerPrefs has a given key.
        /// </summary>
        /// <param name="key">A key.</param>
        [PublicAPI]
        public static bool HasKey( string key )
        {
            return Prefs.HasKey( key );
        }

        /// <summary>
        /// Gets boolean value of a given key in PlayerPrefs.
        /// </summary>
        /// <param name="key">A key.</param>
        /// <param name="defaultValue">A value returned if PlayerPrefs does not have a given key.</param>
        [PublicAPI]
        public static bool GetBool( string key, bool defaultValue = false )
        {
            return Prefs.GetInt( key, defaultValue ? 1 : 0 ) > 0;
        }

        /// <summary>
        /// Sets boolean value of a given key in PlayerPrefs.
        /// </summary>
        /// <param name="key">A key.</param>
        /// <param name="value">A value of a given key.</param>
        [PublicAPI]
        public static void SetBool( string key, bool value )
        {
            Prefs.SetInt( key, value ? 1 : 0 );

            if ( !Application.isEditor )
                Prefs.Save();
        }

        /// <summary>
        /// Gets integer value of a given key in PlayerPrefs.
        /// </summary>
        /// <param name="key">A key.</param>
        /// <param name="defaultValue">A value returned if PlayerPrefs does not have a given key.</param>
        [PublicAPI]
        public static int GetInt( string key, int defaultValue = 0 )
        {
            return Prefs.GetInt( key, defaultValue );
        }

        /// <summary>
        /// Sets integer value of a given key in PlayerPrefs.
        /// </summary>
        /// <param name="key">A key.</param>
        /// <param name="value">A value of a given key.</param>
        [PublicAPI]
        public static void SetInt( string key, int value )
        {
            Prefs.SetInt( key, value );

            if ( !Application.isEditor )
                Prefs.Save();
        }

        /// <summary>
        /// Gets unsigned integer value of a given key in PlayerPrefs.
        /// </summary>
        /// <param name="key">A key.</param>
        /// <param name="defaultValue">A value returned if PlayerPrefs does not have a given key.</param>
        [PublicAPI]
        public static uint GetUInt( string key, uint defaultValue = 0 )
        {
            if ( HasKey( key ) )
            {
                var stringValue = Prefs.GetString( key );

                if ( uint.TryParse( stringValue, out var value ) )
                    return value;
            }

            return defaultValue;
        }

        /// <summary>
        /// Sets unsigned integer value of a given key in PlayerPrefs.
        /// </summary>
        /// <param name="key">A key.</param>
        /// <param name="value">A value of a given key.</param>
        [PublicAPI]
        public static void SetUInt( string key, uint value )
        {
            Prefs.SetString( key, value.ToString( "0" ) );

            if ( !Application.isEditor )
                Prefs.Save();
        }

        /// <summary>
        /// Gets long value of a given key in PlayerPrefs.
        /// </summary>
        /// <param name="key">A key.</param>
        /// <param name="defaultValue">A value returned if PlayerPrefs does not have a given key.</param>
        [PublicAPI]
        public static long GetLong( string key, long defaultValue = 0 )
        {
            if ( HasKey( key ) )
            {
                var stringValue = Prefs.GetString( key );

                if ( long.TryParse( stringValue, out var value ) )
                    return value;
            }

            return defaultValue;
        }

        /// <summary>
        /// Sets unsigned integer value of a given key in PlayerPrefs.
        /// </summary>
        /// <param name="key">A key.</param>
        /// <param name="value">A value of a given key.</param>
        [PublicAPI]
        public static void SetLong( string key, long value )
        {
            Prefs.SetString( key, value.ToString( "0" ) );

            if ( !Application.isEditor )
                Prefs.Save();
        }

        /// <summary>
        /// Gets float value of a given key in PlayerPrefs.
        /// </summary>
        /// <param name="key">A key.</param>
        /// <param name="defaultValue">A value returned if PlayerPrefs does not have a given key.</param>
        [PublicAPI]
        public static float GetFloat( string key, float defaultValue = 0.0f )
        {
            return Prefs.GetFloat( key, defaultValue );
        }

        /// <summary>
        /// Sets float value of a given key in PlayerPrefs.
        /// </summary>
        /// <param name="key">A key.</param>
        /// <param name="value">A value of a given key.</param>
        [PublicAPI]
        public static void SetFloat( string key, float value )
        {
            Prefs.SetFloat( key, value );

            if ( !Application.isEditor )
                Prefs.Save();
        }

        /// <summary>
        /// Gets string value of a given key in PlayerPrefs.
        /// </summary>
        /// <param name="key">A key.</param>
        /// <param name="defaultValue">A value returned if PlayerPrefs does not have a given key.</param>
        [PublicAPI]
        public static string GetString( string key, string defaultValue = "" )
        {
            return Prefs.GetString( key, defaultValue );
        }

        /// <summary>
        /// Sets string value of a given key in PlayerPrefs.
        /// </summary>
        /// <param name="key">A key.</param>
        /// <param name="value">A value of a given key.</param>
        [PublicAPI]
        public static void SetString( string key, string value )
        {
            Prefs.SetString( key, value );

            if ( !Application.isEditor )
                Prefs.Save();
        }

        /// <summary>
        /// Deletes value of a given key in PlayerPrefs.
        /// </summary>
        /// <param name="key">A key.</param>
        [PublicAPI]
        public static void DeleteKey( string key )
        {
            Prefs.DeleteKey( key );

            if ( !Application.isEditor )
                Prefs.Save();
        }

        /// <summary>
        /// Deletes all values in PlayerPrefs.
        /// </summary>
        [PublicAPI]
        public static void DeleteAll()
        {
            Prefs.DeleteAll();

            if ( !Application.isEditor )
                Prefs.Save();
        }
    }
}