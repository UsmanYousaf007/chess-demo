#define HUF_STORAGE_FIREBASE

using Firebase.Storage;
using HUF.InitFirebase.API;
using HUF.Storage.API;
using HUF.StorageFirebase.Implementation;
using HUF.Utils.Configs.API;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.StorageFirebase.API
{
    public static class HStorageFirebase
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void AutoInit()
        {
            if (!HConfigs.HasConfig<FirebaseStorageConfig>())
            {
                Debug.LogError(FirebaseErrorMessages.CONFIG_MISSING_ERROR);
                return;
            }

            TryInitServices();
        }

        static void TryInitServices()
        {
            if (HInitFirebase.IsInitialized)
            {
                InitServices();
            }
            else
            {
                HInitFirebase.OnInitializationSuccess += OnInitFirebaseSuccess;
                HInitFirebase.Init();
            }
        }

        static void InitServices()
        {
            var config = HConfigs.GetConfig<FirebaseStorageConfig>();
            if (config.AutoInitDownloadService)
                TryInitDownloadService();
            if (config.AutoInitUploadService)
                TryInitUploadService();
            if (config.AutoInitRemoveService)
                TryInitRemoveService();
        }

        static void OnInitFirebaseSuccess()
        {
            HInitFirebase.OnInitializationSuccess -= OnInitFirebaseSuccess;
            InitServices();
        }

        /// <summary>
        /// Tries to register Download Service. 
        /// </summary>
        /// <returns>True if service registered successfully, false otherwise</returns>
        [PublicAPI]
        public static bool TryInitDownloadService()
        {
            return HInitFirebase.IsInitialized && HStorage.TryRegisterDownloadService(
                       new FirebaseStorageDownloadService(FirebaseStorage.DefaultInstance));
        }

        /// <summary>
        /// Tries to register Upload Service. 
        /// </summary>
        /// <returns>True if service registered successfully, false otherwise</returns>
        [PublicAPI]
        public static bool TryInitUploadService()
        {
            return HInitFirebase.IsInitialized && HStorage.TryRegisterUploadService(
                       new FirebaseStorageUploadService(FirebaseStorage.DefaultInstance));
        }

        /// <summary>
        /// Tries to register Remove Service. 
        /// </summary>
        /// <returns>True if service registered successfully, false otherwise</returns>
        [PublicAPI]
        public static bool TryInitRemoveService()
        {
            return HInitFirebase.IsInitialized && HStorage.TryRegisterRemoveService(
                       new FirebaseStorageRemoveService(FirebaseStorage.DefaultInstance));;
        }
    }
}