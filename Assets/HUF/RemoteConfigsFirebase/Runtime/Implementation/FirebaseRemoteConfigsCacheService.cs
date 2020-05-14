using System;
using System.Collections.Generic;
using System.Text;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using HUF.Utils.Runtime.PlayerPrefs;

namespace HUF.RemoteConfigsFirebase.Runtime.Implementation
{
    public class FirebaseRemoteConfigsCacheService
    {
        const string KEY_PREFIX = "RemoteConfigsFirebase";
        const string CACHED_KEYS_KEY = KEY_PREFIX + ".CachedConfigsKeys";
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(FirebaseRemoteConfigsCacheService) );

        readonly StringArrayPP cachedKeys;
        readonly Dictionary<string, StringPP> cachedConfigs;

        public FirebaseRemoteConfigsCacheService()
        {
            HLog.Log( logPrefix, $"Initializing firebase remote config cache service" );
            cachedKeys = new StringArrayPP( CACHED_KEYS_KEY, ',' );
            cachedConfigs = new Dictionary<string, StringPP>();
        }

        public void ClearCache()
        {
            foreach ( var config in cachedConfigs )
            {
                config.Value.Value = null;
            }

            cachedConfigs.Clear();
            cachedKeys.Value = null;
        }

        public void CacheConfig( string key, string value )
        {
            var cachedKey = $"{KEY_PREFIX}.{key}";

            if ( cachedConfigs.ContainsKey( cachedKey ) )
            {
                cachedConfigs[cachedKey].Value = EncodeBase64( value );
            }
            else
            {
                cachedConfigs.Add( cachedKey, new StringPP( cachedKey ) );
                cachedConfigs[cachedKey].Value = EncodeBase64( value );
            }

            cachedKeys.AddUnique( cachedKey );
        }

        public Dictionary<string, string> ReadAllConfigs()
        {
            var configs = new Dictionary<string, string>();

            if ( cachedKeys.Value == null || cachedKeys.Value.Length == 0 )
            {
                HLog.Log( logPrefix, $"No cached configs found" );
                return configs;
            }

            HLog.Log( logPrefix, $"Found {cachedKeys.Value.Length} cached configs" );

            foreach ( var cachedKey in cachedKeys.Value )
            {
                try
                {
                    StringPP config;

                    if ( cachedConfigs.ContainsKey( cachedKey ) )
                    {
                        config = cachedConfigs[cachedKey];
                    }
                    else
                    {
                        config = new StringPP( cachedKey );
                        cachedConfigs.Add( cachedKey, config );
                    }

                    if ( !config.Value.IsNullOrEmpty() )
                    {
                        configs.Add( cachedKey.Substring( $"{KEY_PREFIX}.".Length ), DecodeBase64( config.Value ) );
                    }
                }
                catch ( Exception exception )
                {
                    HLog.LogWarning( logPrefix, $"Failed to get configs from cache: {exception.Message}" );
                }
            }

            return configs;
        }

        string EncodeBase64( string target )
        {
            byte[] bytesToEncode = Encoding.UTF8.GetBytes( target );
            return Convert.ToBase64String( bytesToEncode );
        }

        string DecodeBase64( string target )
        {
            byte[] decodedBytes = Convert.FromBase64String( target );
            return Encoding.UTF8.GetString( decodedBytes );
        }
    }
}