using System;
using System.Collections.Generic;
using Firebase.Storage;
using HUF.InitFirebase.Runtime.API;
using HUF.Storage.Runtime.API.Services;
using HUF.Storage.Runtime.Implementation;
using HUF.Storage.Runtime.Implementation.Structs;
using HUF.StorageFirebase.Runtime.API;
using HUF.StorageFirebase.Runtime.Implementation.ActionHandlers;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using UnityEngine;
using UnityEngine.Events;
using StorageService = HUF.Storage.Runtime.API.StorageService;

namespace HUF.StorageFirebase.Runtime.Implementation
{
    public class FirebaseStorageDownloadService : IDownloadService
    {
        public event Action<StorageService> OnInit;

        readonly Dictionary<string, object> downloadHandles;

        FirebaseStorage firebaseStorage;
        FirebaseStorageConfig firebaseStorageConfig;

        public FirebaseStorageDownloadService( FirebaseStorage storageReference )
        {
            downloadHandles = new Dictionary<string, object>();

            if ( HInitFirebase.IsInitialized )
                Initialize();
            else
                HInitFirebase.OnInitializationSuccess += HandleInitSuccess;
        }

        FirebaseStorageConfig FirebaseStorageConfig
        {
            get
            {
                if ( firebaseStorageConfig == null )
                {
                    firebaseStorageConfig = HConfigs.GetConfig<FirebaseStorageConfig>();
                }

                return firebaseStorageConfig;
            }
        }

        public bool IsInitialized => firebaseStorage != null;

        public void GetFileBytes(
            string filePath,
            Action<ObjectResultContainer<byte[]>> completeHandler )
        {
            if ( firebaseStorage == null )
            {
                completeHandler.Dispatch( new ObjectResultContainer<byte[]>(
                    new StorageResultContainer( filePath, FirebaseErrorMessages.FIREBASE_NOT_INITIALIZED ) ) );
            }
            else
            {
                completeHandler += HandleCompleteReceived;
                var bytesFileReference = GetStorageReferenceForPath( filePath );
                var bytesDownloadHandler = new BytesDownloadHandler( filePath, completeHandler );
                downloadHandles.Add( filePath, bytesDownloadHandler );
                bytesDownloadHandler.DownloadFile( bytesFileReference );
            }
        }

        public void GetTexture(
            string filePath,
            Action<ObjectResultContainer<Texture2D>> completeHandler )
        {
            if ( firebaseStorage == null )
            {
                completeHandler.Dispatch( new ObjectResultContainer<Texture2D>(
                    new StorageResultContainer( filePath, FirebaseErrorMessages.FIREBASE_NOT_INITIALIZED ) ) );
            }
            else
            {
                completeHandler += HandleCompleteReceived;
                var textureReference = GetStorageReferenceForPath( filePath );
                var textureDownloadHandler = new TextureDownloadHandler( filePath, completeHandler );
                downloadHandles.Add( filePath, textureDownloadHandler );
                textureDownloadHandler.DownloadFile( textureReference );
            }
        }

        public void GetAudioClip(
            string filePath,
            Action<ObjectResultContainer<AudioClip>> completeHandler )
        {
            if ( firebaseStorage == null )
            {
                completeHandler.Dispatch( new ObjectResultContainer<AudioClip>(
                    new StorageResultContainer( filePath, FirebaseErrorMessages.FIREBASE_NOT_INITIALIZED ) ) );
            }
            else
            {
                completeHandler += HandleCompleteReceived;
                var audioClipReference = GetStorageReferenceForPath( filePath );
                var audioDownloadHandler = new AudioClipDownloadHandler( filePath, completeHandler );
                downloadHandles.Add( filePath, audioDownloadHandler );
                audioDownloadHandler.DownloadFile( audioClipReference );
            }
        }

        public void GetAssetBundle(
            string filePath,
            Action<ObjectResultContainer<AssetBundle>> completeHandler )
        {
            if ( firebaseStorage == null )
            {
                completeHandler.Dispatch( new ObjectResultContainer<AssetBundle>(
                    new StorageResultContainer( filePath, FirebaseErrorMessages.FIREBASE_NOT_INITIALIZED ) ) );
            }

            completeHandler += HandleCompleteReceived;
            var assetBundleReference = GetStorageReferenceForPath( filePath );
            var assetBundleDownloadHandler = new AssetBundleDownloadHandler( filePath, completeHandler );
            downloadHandles.Add( filePath, assetBundleDownloadHandler );
            assetBundleDownloadHandler.DownloadFile( assetBundleReference );
        }

        public void GetUpdateInfo( string filePath, Action<MetadataResultContainer> completeHandler )
        {
            if ( firebaseStorage == null || FirebaseStorageConfig == null )
            {
                var errorMessage = firebaseStorage == null
                    ? FirebaseErrorMessages.FIREBASE_NOT_INITIALIZED
                    : FirebaseErrorMessages.CONFIG_MISSING_ERROR;

                completeHandler.Dispatch(
                    new MetadataResultContainer( new StorageResultContainer( filePath, errorMessage ) ) );
            }
            else
            {
                var fileReference = firebaseStorage.GetReferenceFromUrl(
                    StorageUtils.GetDatabaseUrlPath( FirebaseStorageConfig.DatabaseAddress, filePath ) );
                SetupMetadataDownloadHandler( filePath, fileReference );
            }
        }

        public string GetRemotePath( string filePath )
        {
            return filePath;
        }

        void HandleCompleteReceived<T>( ObjectResultContainer<T> resultContainer ) where T : class
        {
            var filePath = resultContainer.StorageResultContainer.PathToFile;

            if ( downloadHandles.ContainsKey( filePath ) )
            {
                downloadHandles.Remove( filePath );
            }
        }

        void SetupMetadataDownloadHandler( string filePath, StorageReference assetBundleReference )
        {
            var metadataKey = GetMetadataKey( filePath );
            var metadataDownloadHandler = new MetadataDownloadHandler( filePath, HandleMetadataDownloadComplete );
            downloadHandles.Add( metadataKey, metadataDownloadHandler );
            metadataDownloadHandler.GetMetadata( assetBundleReference );
        }

        void HandleMetadataDownloadComplete( MetadataResultContainer metadataResultContainer )
        {
            var filePath = metadataResultContainer.StorageResultContainer.PathToFile;
            var metadataKey = GetMetadataKey( filePath );

            if ( downloadHandles.ContainsKey( metadataKey ) )
            {
                downloadHandles.Remove( metadataKey );
            }
        }

        string GetMetadataKey( string filePath )
        {
            return $"{filePath}.meta";
        }

        StorageReference GetStorageReferenceForPath( string filePath )
        {
            if ( FirebaseStorageConfig != null )
            {
                var databaseUrl = StorageUtils.GetDatabaseUrlPath( FirebaseStorageConfig.DatabaseAddress, filePath );
                return firebaseStorage.GetReferenceFromUrl( databaseUrl );
            }

            return null;
        }

        void HandleInitSuccess()
        {
            HInitFirebase.OnInitializationSuccess -= HandleInitSuccess;
            Initialize();
        }

        void Initialize()
        {
            firebaseStorage = FirebaseStorage.DefaultInstance;
            OnInit.Dispatch( StorageService.Firebase );
        }
    }
}