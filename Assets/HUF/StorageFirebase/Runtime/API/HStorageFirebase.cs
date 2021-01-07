using Firebase.Storage;
using HUF.InitFirebase.Runtime.API;
using HUF.InitFirebase.Runtime.Config;
using HUF.Storage.Runtime.API;
using HUF.StorageFirebase.Runtime.Implementation;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.StorageFirebase.Runtime.API
{
    public static class HStorageFirebase
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(HStorageFirebase) );

        static FirebaseStorageConfig config;

        static FirebaseStorageConfig Config
        {
            get
            {
                if ( config != null )
                    return config;

                if ( !HConfigs.HasConfig<FirebaseStorageConfig>() )
                    return null;

                return config = HConfigs.GetConfig<FirebaseStorageConfig>();
            }
        }

        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
        static void AutoInit()
        {
            if ( !HConfigs.HasConfig<FirebaseStorageConfig>() )
            {
                HLog.LogError( logPrefix, FirebaseErrorMessages.CONFIG_MISSING_ERROR );
                return;
            }

            TryInitServices();
        }

        static void TryInitServices()
        {
            if ( HInitFirebase.IsInitialized )
            {
                InitServices();
            }
            else
            {
                HInitFirebase.OnInitializationSuccess += OnInitFirebaseSuccess;

                if ( !HConfigs.HasConfig<HFirebaseConfig>() || HConfigs.GetConfig<HFirebaseConfig>().AutoInit )
                    HInitFirebase.Init();
            }
        }

        static void InitServices()
        {
            if ( Config.AutoInitDownloadService )
                TryInitDownloadService();

            if ( Config.AutoInitUploadService )
                TryInitUploadService();

            if ( Config.AutoInitRemoveService )
                TryInitRemoveService();
        }

        static void OnInitFirebaseSuccess()
        {
            HInitFirebase.OnInitializationSuccess -= OnInitFirebaseSuccess;
            InitServices();
        }

        /// <summary>
        /// Tries to register a download service.
        /// </summary>
        /// <returns>True if the service was registered successfully, false otherwise</returns>
        [PublicAPI]
        public static bool TryInitDownloadService()
        {
            return HInitFirebase.IsInitialized && HStorage.TryRegisterDownloadService(
                new FirebaseStorageDownloadService( FirebaseStorage.DefaultInstance ) );
        }

        /// <summary>
        /// Tries to register an upload service.
        /// </summary>
        /// <returns>True if the service was registered successfully, false otherwise</returns>
        [PublicAPI]
        public static bool TryInitUploadService()
        {
            return HInitFirebase.IsInitialized && HStorage.TryRegisterUploadService(
                new FirebaseStorageUploadService( FirebaseStorage.DefaultInstance ) );
        }

        /// <summary>
        /// Tries to register a remove service.
        /// </summary>
        /// <returns>True if the service was registered successfully, false otherwise</returns>
        [PublicAPI]
        public static bool TryInitRemoveService()
        {
            return HInitFirebase.IsInitialized && HStorage.TryRegisterRemoveService(
                new FirebaseStorageRemoveService( FirebaseStorage.DefaultInstance ) );

            ;
        }
    }
}