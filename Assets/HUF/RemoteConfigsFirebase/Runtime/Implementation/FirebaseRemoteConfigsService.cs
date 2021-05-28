using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using HUF.InitFirebase.Runtime;
using HUF.InitFirebase.Runtime.API;
using HUF.InitFirebase.Runtime.Config;
using HUF.RemoteConfigs.Runtime.API;
using HUF.RemoteConfigs.Runtime.Implementation;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;

namespace HUF.RemoteConfigsFirebase.Runtime.Implementation
{
    public class FirebaseRemoteConfigsService : BaseRemoteService
    {
        const string SERVICE_UID = "Firebase";
        const RemoteConfigService SERVICE_TYPE = RemoteConfigService.Firebase;
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(FirebaseRemoteConfigsService) );
        public override bool HasCachedData => fetchedDict != null && fetchedDict.Count > 0;
        public override bool IsInitialized => HInitFirebase.IsInitialized;
        public override bool SupportsCaching => true;
        public override string UID => SERVICE_UID;

        FirebaseRemoteConfigsConfig config;

        FirebaseRemoteConfigsConfig Config
        {
            get
            {
                if ( config == null )
                    config = HConfigs.GetConfig<FirebaseRemoteConfigsConfig>();
                return config;
            }
        }

        Dictionary<string, string> fetchedDict = new Dictionary<string, string>();

        readonly FirebaseRemoteConfigsCacheService remoteConfigsCacheService;

        public override event Action<RemoteConfigService> OnInitialized;
        public override event Action<RemoteConfigService> OnFetchComplete;
        public override event Action<RemoteConfigService> OnFetchFailed;

        public FirebaseRemoteConfigsService()
        {
            remoteConfigsCacheService = new FirebaseRemoteConfigsCacheService();

            if ( HInitFirebase.IsInitialized )
            {
                FinishInitialization();
            }
            else
            {
                HInitFirebase.OnInitializationFailure += OnFirebaseInitFail;
                HInitFirebase.OnInitializationSuccess += OnFirebaseInitSuccess;

                if ( !HConfigs.HasConfig<HFirebaseConfig>() || HConfigs.GetConfig<HFirebaseConfig>().AutoInit )
                    HInitFirebase.Init();
                CreateFetchedDictFromCache();
            }
        }

        public override Dictionary<string, string> GetConfigJSONs()
        {
            return fetchedDict;
        }

        public override string GetConfigJSON( string configId )
        {
            return fetchedDict.ContainsKey( configId ) ? fetchedDict[configId] : string.Empty;
        }

        public override void ApplyConfig<T>( ref T config )
        {
            if ( fetchedDict != null && fetchedDict.ContainsKey( config.ConfigId ) )
            {
                var json = fetchedDict[config.ConfigId];
                config.ApplyJson( json );
            }
        }

        public override void Fetch()
        {
            if ( !IsInitialized )
            {
                HLog.LogWarning( logPrefix, $"Firebase RemoteConfig is not initialized yet." );
                return;
            }

            HLog.Log( logPrefix, $"Fetch started." );
            var cacheExpirationTimeSpan = TimeSpan.FromSeconds( Config.CacheExpirationInSeconds );
            FirebaseRemoteConfig.DefaultInstance.FetchAsync( cacheExpirationTimeSpan ).ContinueWithOnMainThread( HandleFetchCompleted );
        }

        void DetachCallbacks()
        {
            HInitFirebase.OnInitializationFailure -= OnFirebaseInitFail;
            HInitFirebase.OnInitializationSuccess -= OnFirebaseInitSuccess;
        }

        void SaveCache()
        {
            remoteConfigsCacheService.ClearCache();

            foreach ( var fetchedConfig in fetchedDict )
            {
                remoteConfigsCacheService.CacheConfig( fetchedConfig.Key, fetchedConfig.Value );
            }
        }

        void OnFirebaseInitFail()
        {
            DetachCallbacks();

            HLog.LogWarning( logPrefix,
                $"Firebase Init failed. Something went wrong. " +
                "Please address console logs for more info." );
        }

        void OnFirebaseInitSuccess()
        {
            DetachCallbacks();
            FinishInitialization();
        }

        void FinishInitialization()
        {
            CreateFetchedDict();
            HLog.Log( logPrefix, "Service initialized" );
            OnInitialized.Dispatch( SERVICE_TYPE );
        }

        void HandleFetchCompleted( Task fetchTask )
        {
            FirebaseRemoteConfig.DefaultInstance.ActivateAsync().ContinueWithOnMainThread( HandleActivateConfig );

            if ( FirebaseRemoteConfig.DefaultInstance.Info.LastFetchStatus == LastFetchStatus.Success )
            {
                HLog.Log( logPrefix, $"Firebase cache used. Nothing new was fetched" );
                OnFetchComplete.Dispatch( SERVICE_TYPE );
                return;
            }

            string failReason;

            if ( fetchTask.Exception != null )
            {
                failReason = fetchTask.Exception.GetFullErrorMessage();
            }
            else if ( fetchTask.IsCanceled )
            {
                failReason = "Fetch task was cancelled.";
            }
            else
            {
                failReason = FirebaseRemoteConfig.DefaultInstance.Info.LastFetchFailureReason.ToString();

                switch ( FirebaseRemoteConfig.DefaultInstance.Info.LastFetchFailureReason )
                {
                    case FetchFailureReason.Invalid:
                        failReason = "There is no error but nothing was fetched. " +
                                     "Probably 'Fetch' was called before the previously cached data expired. " +
                                     "Check 'CacheExpirationInSeconds' field in 'FirebaseRemoteConfigsConfig'.";
                        break;
                    case FetchFailureReason.Throttled:
                        failReason =
                            $"Throttled by the server until {FirebaseRemoteConfig.DefaultInstance.Info.ThrottledEndTime}. You are sending too many fetch requests in too short a time.";
                        break;
                    case FetchFailureReason.Error:
                        failReason = "There was an unknown error on the Server.";
                        break;
                }
            }

            HLog.LogWarning( logPrefix, $"Fetch failed. {failReason}" );
            OnFetchFailed.Dispatch( SERVICE_TYPE );
        }

        void FinalizeFetchCompleted()
        {
            OnFetchComplete.Dispatch( SERVICE_TYPE );
            SaveCache();
        }

        void HandleActivateConfig(Task<bool> activateTask)
        {
            if (activateTask.Result == false)
                return;

            HLog.Log( logPrefix, $"Fetch completed." );
            FinalizeFetchCompleted();
            CreateFetchedDict();
        }

        void CreateFetchedDictFromCache()
        {
            fetchedDict.Clear();
            fetchedDict = remoteConfigsCacheService.ReadAllConfigs();
        }

        void CreateFetchedDict()
        {
            fetchedDict.Clear();

            foreach ( var key in FirebaseRemoteConfig.DefaultInstance.Keys )
            {
                var value = FirebaseRemoteConfig.DefaultInstance.GetValue( key ).StringValue;
                fetchedDict.Add( key, value );
            }
        }
    }
}