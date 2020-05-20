using System;
using HUF.Storage.Runtime.API.Models;
using HUF.Storage.Runtime.API.Structs;
using HUF.Storage.Runtime.Implementation.Models;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEngine.Events;

namespace HUF.Storage.Runtime.API
{
    public static class HStorage
    {
        public static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(HStorage) );

        static StorageModel storageModel;
        static StorageModel StorageModel => storageModel ?? (storageModel = new StorageModel());

        static HStorageTexture texture;
        static HStorageAudioClip audioClip;
        static HStorageAssetBundle assetBundle;
        static HStorageBytes bytes;

        /// <summary>
        /// Provides methods to get Texture2D from Storage.
        /// </summary>
        [PublicAPI]
        public static HStorageTexture Texture => texture ?? (texture = new HStorageTexture(StorageModel));

        /// <summary>
        /// Provides methods to get AudioClip from Storage.
        /// </summary>
        [PublicAPI]
        public static HStorageAudioClip AudioClip => audioClip ?? (audioClip = new HStorageAudioClip(StorageModel));

        /// <summary>
        /// Provides methods to get Asset Bundles from Storage.
        /// </summary>
        [PublicAPI]
        public static HStorageAssetBundle AssetBundle =>
            assetBundle ?? (assetBundle = new HStorageAssetBundle(StorageModel));

        /// <summary>
        /// Provides methods to get byte[] from Storage.
        /// </summary>
        [PublicAPI]
        public static HStorageBytes Bytes => bytes ?? (bytes = new HStorageBytes(StorageModel));

        /// <summary>
        /// Registers download service.
        /// </summary>
        /// <param name="downloadService">Service to be registered</param>
        /// <returns>True, if service was registered; false otherwise</returns>
        [PublicAPI]
        public static bool TryRegisterDownloadService(IDownloadService downloadService)
        {
            return LogServiceRegistration(
                StorageModel.TryRegisterService( new StorageDownloadModel( downloadService ) ),
                downloadService.GetType()
            );
        }

        /// <summary>
        /// Gives information if file at specific path has update available. Complete handler value
        /// <see cref="MetadataResultContainer"/> IsUpdateAvailable is set to true if file was not downloaded yet,
        /// or update is available. Otherwise false.
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <param name="completeHandler">Handler of action completed</param>
        [PublicAPI]
        public static void IsUpdateAvailable(string filePath, UnityAction<MetadataResultContainer> completeHandler)
        {
            StorageModel.DownloadService?.GetUpdateInfo(filePath, completeHandler);
        }

        /// <summary>
        /// Tries to register upload service.
        /// </summary>
        /// <param name="uploadService">Service to be registered</param>
        /// <returns>True, if service was registered; false otherwise</returns>
        [PublicAPI]
        public static bool TryRegisterUploadService(IUploadService uploadService)
        {
            return LogServiceRegistration(
                StorageModel.TryRegisterService( new StorageUploadModel( uploadService ) ),
                uploadService.GetType()
            );
        }

        /// <summary>
        /// Starts uploading file. Complete handler is method that will handle upload process after its completion.
        /// </summary>
        /// <param name="objectToUpload">Object to upload</param>
        /// <param name="filePath">Path that object will be saved to</param>
        /// <param name="uploadTaskResult">Upload result handler</param>
        [PublicAPI]
        public static void UploadFile(object objectToUpload, string filePath,
            UnityAction<StorageResultContainer> uploadTaskResult)
        {
            StorageModel.UploadService?.UploadFile(filePath, objectToUpload, uploadTaskResult);
        }

        /// <summary>
        /// Tries to register removal service.
        /// </summary>
        /// <param name="removeService">Service to be registered</param>
        /// <returns>True, if service was registered; false otherwise</returns>
        [PublicAPI]
        public static bool TryRegisterRemoveService(IRemoveService removeService)
        {
            return LogServiceRegistration(
                StorageModel.TryRegisterService( new StorageRemoveModel( removeService ) ),
                removeService.GetType()
            );

        }

        /// <summary>
        /// Removes file at given path. Handler will be called after removal process is completed.
        /// </summary>
        /// <param name="filePath">Path to file that will be removed</param>
        /// <param name="handleRemoveCompleted">Handler of removal process</param>
        [PublicAPI]
        public static void RemoveFile(string filePath, UnityAction<StorageResultContainer> handleRemoveCompleted)
        {
            StorageModel.RemoveService?.RemoveFile(filePath, handleRemoveCompleted);
        }

        /// <summary>
        /// Disposes Storage service.
        /// </summary>
        [PublicAPI]
        public static void DisposeServices()
        {
            storageModel?.Dispose();
            storageModel = null;
        }

        static bool LogServiceRegistration( bool result, Type type )
        {
            if ( result )
                HLog.Log( logPrefix, $"Service {type} registered" );
            else
                HLog.LogError( logPrefix, $"Unable to register service {type}" );

            return result;
        }
    }
}