using System;
using Firebase.Storage;
using HUF.InitFirebase.Runtime.API;
using HUF.InitFirebase.Runtime.Config;
using HUF.Storage.Runtime.API;
using HUF.Storage.Runtime.API.Services;
using HUF.StorageFirebase.Runtime.Implementation;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.StorageFirebase.Runtime.API
{
    public static class HStorageFirebase
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(HStorageFirebase) );

        static IDownloadService downloadService;

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
            if ( Config == null )
            {
                HLog.LogError( logPrefix, FirebaseErrorMessages.CONFIG_MISSING_ERROR );
                return;
            }

            if ( Config.AutoInit )
                Initialize();
        }

        /// <summary>
        /// Occurs when the storage is initialized.
        /// </summary>
        [PublicAPI]
        public static event Action OnInit;

        /// <summary>
        /// Is set to TRUE if service is fully initialized and FALSE otherwise
        /// </summary>
        [PublicAPI]
        public static bool IsInitialized => downloadService != null;

        /// <summary>
        /// Initializes the Storage Firebase service.
        /// </summary>
        [PublicAPI]
        public static void Initialize()
        {
            if ( !HInitFirebase.IsInitialized &&
                 ( !HConfigs.HasConfig<HFirebaseConfig>() || HConfigs.GetConfig<HFirebaseConfig>().AutoInit ) )
            {
                HInitFirebase.Init();
                HInitFirebase.OnInitializationSuccess += HandleFirebaseInitialized;
            }
            else
                Init();
        }

        static void HandleFirebaseInitialized()
        {
            HInitFirebase.OnInitializationSuccess -= HandleFirebaseInitialized;
            Init();
        }

        static void Init()
        {
            if ( Config == null )
            {
                HLog.LogError( logPrefix, FirebaseErrorMessages.CONFIG_MISSING_ERROR );
                return;
            }

            if ( downloadService != null )
            {
                HLog.LogError( logPrefix, FirebaseErrorMessages.STORAGE_ALREADY_INITIALIZED );
                return;
            }

            downloadService = new FirebaseStorageDownloadService( FirebaseStorage.DefaultInstance );

            HStorage.RegisterService( downloadService,
                new FirebaseStorageUploadService( FirebaseStorage.DefaultInstance ),
                new FirebaseStorageRemoveService( FirebaseStorage.DefaultInstance ),
                StorageService.Firebase,
                Config.IsMain );
            OnInit.Dispatch();
        }
    }
}