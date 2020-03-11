using System.Collections.Generic;
using Firebase.Storage;
using HUF.InitFirebase.API;
using HUF.Storage.API;
using HUF.Storage.API.Structs;
using HUF.StorageFirebase.API;
using HUF.StorageFirebase.Implementation.ActionHandlers;
using HUF.Utils.Configs.API;
using HUF.Utils.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.StorageFirebase.Implementation
{
    public class FirebaseStorageDownloadService : IDownloadService
    {
        FirebaseStorage firebaseStorage;
        FirebaseStorageConfig firebaseStorageConfig;
        
        FirebaseStorageConfig FirebaseStorageConfig
        {
            get
            {
                if (firebaseStorageConfig == null)
                {
                    firebaseStorageConfig = HConfigs.GetConfig<FirebaseStorageConfig>();
                }
                return firebaseStorageConfig;
            }
        }

        readonly Dictionary<string, object> downloadHandles;

        public FirebaseStorageDownloadService(FirebaseStorage storageReference)
        {
            downloadHandles = new Dictionary<string, object>();
            
            if (HInitFirebase.IsInitialized)
                firebaseStorage = storageReference;
            else
                HInitFirebase.OnInitializationSuccess += HandleInitSuccess;
        }

        void HandleInitSuccess()
        {
            HInitFirebase.OnInitializationSuccess -= HandleInitSuccess;
            firebaseStorage = FirebaseStorage.DefaultInstance;
        }

        public void GetFileBytes(
            string filePath, 
            UnityAction<ObjectResultContainer<byte[]>> completeHandler,
            bool forceDownload = false)
        {
            if (firebaseStorage == null)
            {
                completeHandler.Dispatch(new ObjectResultContainer<byte[]>(
                    new StorageResultContainer(filePath, FirebaseErrorMessages.FIREBASE_NOT_INITIALIZED)));
            }
            else
            {
                completeHandler += HandleCompleteReceived;
                var bytesFileReference = GetStorageReferenceForPath(filePath);
                var bytesDownloadHandler = new BytesDownloadHandler(filePath, completeHandler);
                downloadHandles.Add(filePath, bytesDownloadHandler);
                bytesDownloadHandler.DownloadFile(bytesFileReference);
            }
        }

        public void GetTexture(
            string filePath, 
            UnityAction<ObjectResultContainer<Texture2D>> completeHandler,
            bool forceDownload = false)
        {
            if (firebaseStorage == null)
            {
                completeHandler.Dispatch(new ObjectResultContainer<Texture2D>(
                    new StorageResultContainer(filePath, FirebaseErrorMessages.FIREBASE_NOT_INITIALIZED)));
            }
            else
            {
                completeHandler += HandleCompleteReceived;
                var textureReference = GetStorageReferenceForPath(filePath);
                var textureDownloadHandler = new TextureDownloadHandler(filePath, completeHandler);
                downloadHandles.Add(filePath, textureDownloadHandler);
                textureDownloadHandler.DownloadFile(textureReference);
            }
        }

        public void GetAudioClip(
            string filePath, 
            UnityAction<ObjectResultContainer<AudioClip>> completeHandler,
            bool forceDownload = false)
        {
            if (firebaseStorage == null)
            {
                completeHandler.Dispatch(new ObjectResultContainer<AudioClip>(
                    new StorageResultContainer(filePath, FirebaseErrorMessages.FIREBASE_NOT_INITIALIZED)));
            }
            else
            {
                completeHandler += HandleCompleteReceived;
                var audioClipReference = GetStorageReferenceForPath(filePath);
                var audioDownloadHandler = new AudioClipDownloadHandler(filePath, completeHandler);
                downloadHandles.Add(filePath, audioDownloadHandler);
                audioDownloadHandler.DownloadFile(audioClipReference);
            }
        }

        public void GetAssetBundle(
            string filePath, 
            UnityAction<ObjectResultContainer<AssetBundle>> completeHandler,
            bool forceDownload = false)
        {
            if (firebaseStorage == null)
            {
                completeHandler.Dispatch(new ObjectResultContainer<AssetBundle>(
                    new StorageResultContainer(filePath, FirebaseErrorMessages.FIREBASE_NOT_INITIALIZED)));
            }

            completeHandler += HandleCompleteReceived;
            var assetBundleReference = GetStorageReferenceForPath(filePath);
            var assetBundleDownloadHandler = new AssetBundleDownloadHandler(filePath, completeHandler);
            downloadHandles.Add(filePath, assetBundleDownloadHandler);
            assetBundleDownloadHandler.DownloadFile(assetBundleReference, forceDownload);
        }

        public void GetUpdateInfo(string filePath, UnityAction<MetadataResultContainer> completeHandler)
        {
            if (firebaseStorage == null || FirebaseStorageConfig == null)
            {
                var errorMessage = firebaseStorage == null
                    ? FirebaseErrorMessages.FIREBASE_NOT_INITIALIZED
                    : FirebaseErrorMessages.CONFIG_MISSING_ERROR;
                completeHandler.Dispatch(
                    new MetadataResultContainer(new StorageResultContainer(filePath, errorMessage)));
            }
            else
            {
                var fileReference = firebaseStorage.GetReferenceFromUrl(
                    StorageUtils.GetDatabaseUrlPath(FirebaseStorageConfig.DatabaseAddress, filePath));

                SetupMetadataDownloadHandler(filePath, fileReference);
            }
        }

        void HandleCompleteReceived<T>(ObjectResultContainer<T> resultContainer) where T : class
        {
            var filePath = resultContainer.StorageResultContainer.PathToFile;
            if (downloadHandles.ContainsKey(filePath))
            {
                downloadHandles.Remove(filePath);
            }
        }

        void SetupMetadataDownloadHandler(string filePath, StorageReference assetBundleReference)
        {
            var metadataKey = GetMetadataKey(filePath);
            var metadataDownloadHandler = new MetadataDownloadHandler(filePath, HandleMetadataDownloadComplete);
            downloadHandles.Add(metadataKey, metadataDownloadHandler);
            metadataDownloadHandler.GetMetadata(assetBundleReference);
        }

        void HandleMetadataDownloadComplete(MetadataResultContainer metadataResultContainer)
        {
            var filePath = metadataResultContainer.StorageResultContainer.PathToFile;
            var metadataKey = GetMetadataKey(filePath);
            if (downloadHandles.ContainsKey(metadataKey))
            {
                downloadHandles.Remove(metadataKey);
            }
        }

        string GetMetadataKey(string filePath)
        {
            return $"{filePath}.meta";
        }
        
        StorageReference GetStorageReferenceForPath(string filePath)
        {
            if (FirebaseStorageConfig != null)
            {
                var databaseUrl = StorageUtils.GetDatabaseUrlPath(FirebaseStorageConfig.DatabaseAddress, filePath);
                return firebaseStorage.GetReferenceFromUrl(databaseUrl);
            }
            return null;
        }

        public void Dispose()
        {
            firebaseStorage = null;
            downloadHandles.Clear();
        }
    }
}