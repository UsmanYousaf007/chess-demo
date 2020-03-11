using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.RemoteConfig;
using Firebase.Extensions;
using HUF.InitFirebase;
using HUF.InitFirebase.API;
using HUF.RemoteConfigs.Implementation;
using HUF.Utils.Configs.API;
using HUF.Utils.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.RemoteConfigsFirebase.Implementation
{
    public class FirebaseRemoteConfigsService : BaseRemoteService
    {
        readonly string logPrefix;
        public override bool IsInitialized => HInitFirebase.IsInitialized;
        
        FirebaseRemoteConfigsConfig config;
        FirebaseRemoteConfigsConfig Config
        {
            get
            {
                if (config == null)
                    config = HConfigs.GetConfig<FirebaseRemoteConfigsConfig>();
                return config;
            }
        }

        Dictionary<string, string> fetchedDict = new Dictionary<string, string>();

        readonly FirebaseRemoteConfigsCacheService remoteConfigsCacheService;

        public override event UnityAction OnInitComplete;
        public override event UnityAction OnFetchComplete;
        public override event UnityAction OnFetchFailed;

        public FirebaseRemoteConfigsService()
        {
            logPrefix = GetType().Name;
            remoteConfigsCacheService = new FirebaseRemoteConfigsCacheService();
            
            if (HInitFirebase.IsInitialized)
            {
                FinishInitialization();
            }
            else
            {
                HInitFirebase.OnInitializationFailure += OnFirebaseInitFail;
                HInitFirebase.OnInitializationSuccess += OnFirebaseInitSuccess;
                HInitFirebase.Init();
                CreateFetchedDictFromCache();
            }
        }

        void SaveCache()
        {
            remoteConfigsCacheService.ClearCache();
            foreach (var fetchedConfig in fetchedDict)
            {
                remoteConfigsCacheService.CacheConfig(fetchedConfig.Key, fetchedConfig.Value);
            }
        }

        void OnFirebaseInitFail()
        {
            Debug.LogWarning($"[{logPrefix}] Firebase Init failed. Something went wrong. " +
                             "Please address console logs for more info.");
        }

        void OnFirebaseInitSuccess()
        {
            HInitFirebase.OnInitializationFailure -= OnFirebaseInitFail;
            HInitFirebase.OnInitializationSuccess -= OnFirebaseInitSuccess;

            FinishInitialization();
        }

        void FinishInitialization()
        {
            CreateFetchedDict();
            OnInitComplete.Dispatch();
        }

        public override void Fetch()
        {
            if (!IsInitialized)
            {
                Debug.LogWarning($"[{logPrefix}] Firebase RemoteConfig is not initialized yet.");
                return;
            }
            
            Debug.Log($"[{logPrefix}] Fetch started.");
            var cacheExpirationTimeSpan = TimeSpan.FromSeconds(Config.CacheExpirationInSeconds);
            FirebaseRemoteConfig.FetchAsync(cacheExpirationTimeSpan).ContinueWithOnMainThread(HandleFetchCompleted);
        }

        void HandleFetchCompleted(Task fetchTask)
        {
            if (TryActivateFetchedConfig())
            {
                Debug.LogFormat($"[{logPrefix}] Fetch completed.");
                FinalizeFetchCompleted();
            }
            else
            {
                string failReason;
                if (fetchTask.Exception != null)
                {
                    failReason = fetchTask.Exception.GetFullErrorMessage();
                }
                else if (fetchTask.IsCanceled)
                {
                    failReason = "Fetch task was cancelled";
                } 
                else if (fetchTask.IsFaulted)
                {
                    failReason = "Unknown error";
                }
                else
                {
                    failReason = "There is no error but nothing was fetched. " +
                                 "Probably 'Fetch' was called before the previously cached data expired. " +
                                 "Check 'CacheExpirationInSeconds' field in 'FirebaseRemoteConfigsConfig'";
                }
                Debug.LogWarning($"[{logPrefix}] Fetch failed. {failReason}");

                OnFetchFailed();
            }
        }

        void FinalizeFetchCompleted()
        {
            OnFetchComplete.Dispatch();
            SaveCache();
        }

        bool TryActivateFetchedConfig()
        {
            if (!FirebaseRemoteConfig.ActivateFetched())
                return false;
            
            CreateFetchedDict();
            return true;
        }

        void CreateFetchedDictFromCache()
        {
            fetchedDict.Clear();
            fetchedDict = remoteConfigsCacheService.ReadAllConfigs();
        }

        void CreateFetchedDict()
        {
            fetchedDict.Clear();
            foreach (var key in FirebaseRemoteConfig.Keys)
            {
                var value = FirebaseRemoteConfig.GetValue(key).StringValue;
                fetchedDict.Add(key, value);
            }
        }

        public override Dictionary<string, string> GetConfigJSONs()
        {
            return fetchedDict;
        }

        public override string GetConfigJSON(string configId)
        {
            return fetchedDict.ContainsKey(configId) ? fetchedDict[configId] : string.Empty;
        }

        public override void ApplyConfig<T>(ref T config)
        {
            if (fetchedDict != null && fetchedDict.ContainsKey(config.ConfigId))
            {
                var json = fetchedDict[config.ConfigId];
                config.ApplyJson(json);
            }
        }
    }
}