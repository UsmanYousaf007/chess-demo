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
        static StorageModel StorageModel => storageModel ?? ( storageModel = new StorageModel() );

        static HStorageTexture texture;
        static HStorageAudioClip audioClip;
        static HStorageAssetBundle assetBundle;
        static HStorageBytes bytes;

        /// <summary>
        /// Provides access to Storage Texture API.
        /// </summary>
        [PublicAPI]
        public static HStorageTexture Texture => texture ?? ( texture = new HStorageTexture( StorageModel ) );

        /// <summary>
        /// Provides access to Storage AudioClip API.
        /// </summary>
        [PublicAPI]
        public static HStorageAudioClip AudioClip => audioClip ?? ( audioClip = new HStorageAudioClip( StorageModel ) );

        /// <summary>
        /// Provides access to Storage Asset Bundles API.
        /// </summary>
        [PublicAPI]
        public static HStorageAssetBundle AssetBundle =>
            assetBundle ?? ( assetBundle = new HStorageAssetBundle( StorageModel ) );

        /// <summary>
        /// Provides access to Storage Bytes API.
        /// </summary>
        [PublicAPI]
        public static HStorageBytes Bytes => bytes ?? ( bytes = new HStorageBytes( StorageModel ) );

        /// <summary>
        /// Registers a download service.
        /// </summary>
        /// <param name="downloadService">A service to be registered</param>
        /// <returns>True, if the service was registered; false otherwise</returns>
        [PublicAPI]
        public static bool TryRegisterDownloadService( IDownloadService downloadService )
        {
            return LogServiceRegistration(
                StorageModel.TryRegisterService( new StorageDownloadModel( downloadService ) ),
                downloadService.GetType()
            );
        }

        /// <summary>
        /// Gives information if the file at the specified path has an update available. The handler value
        /// <see cref="MetadataResultContainer"/> IsUpdateAvailable is set to true if the file was not downloaded yet,
        /// or an update is available. Otherwise false.
        /// </summary>
        /// <param name="filePath">A path to the file</param>
        /// <param name="completeHandler">A metadata result handler</param>
        [PublicAPI]
        public static void IsUpdateAvailable( string filePath, UnityAction<MetadataResultContainer> completeHandler )
        {
            StorageModel.DownloadService?.GetUpdateInfo( filePath, completeHandler );
        }

        /// <summary>
        /// Tries to register an upload service.
        /// </summary>
        /// <param name="uploadService">A service to be registered</param>
        /// <returns>True, if the service was registered; false otherwise</returns>
        [PublicAPI]
        public static bool TryRegisterUploadService( IUploadService uploadService )
        {
            return LogServiceRegistration(
                StorageModel.TryRegisterService( new StorageUploadModel( uploadService ) ),
                uploadService.GetType()
            );
        }

        /// <summary>
        /// Starts uploading the file. The handler will be called after the upload process is completed.
        /// </summary>
        /// <param name="objectToUpload">An object to upload</param>
        /// <param name="filePath">A path that object will be saved to</param>
        /// <param name="uploadTaskResult">A upload result handler</param>
        [PublicAPI]
        public static void UploadFile( object objectToUpload,
            string filePath,
            UnityAction<StorageResultContainer> uploadTaskResult )
        {
            StorageModel.UploadService?.UploadFile( filePath, objectToUpload, uploadTaskResult );
        }

        /// <summary>
        /// Tries to register a removal service.
        /// </summary>
        /// <param name="removeService">A service to be registered</param>
        /// <returns>True, if the service was registered; false otherwise</returns>
        [PublicAPI]
        public static bool TryRegisterRemoveService( IRemoveService removeService )
        {
            return LogServiceRegistration(
                StorageModel.TryRegisterService( new StorageRemoveModel( removeService ) ),
                removeService.GetType()
            );
        }

        /// <summary>
        /// Removes the file at the given path. The handler will be called after the removal process is completed.
        /// </summary>
        /// <param name="filePath">A path to the file that will be removed</param>
        /// <param name="handleRemoveCompleted">A handler of the removal process</param>
        [PublicAPI]
        public static void RemoveFile( string filePath, UnityAction<StorageResultContainer> handleRemoveCompleted )
        {
            StorageModel.RemoveService?.RemoveFile( filePath, handleRemoveCompleted );
        }

        /// <summary>
        /// Disposes of the Storage services.
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