using System;
using HUF.Storage.Runtime.API.Models;
using HUF.Storage.Runtime.API.Services;
using HUF.Storage.Runtime.Implementation.Structs;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;

namespace HUF.Storage.Runtime.API
{
    public static class HStorage
    {
        public static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(HStorage) );

        
#pragma warning disable 0067
        /// <summary>
        /// Occurs when a storage service is initialized with storage enum
        /// </summary>
        [PublicAPI]
        public static event Action<StorageService> OnInit;
#pragma warning restore 0067

        static Implementation.StorageService storageService;

        /// <summary>
        /// Provides access to the storage texture API.
        /// </summary>
        [PublicAPI]
        public static HStorageTexture Texture => StorageService.Texture;

        /// <summary>
        /// Provides access to the storage audio clip API.
        /// </summary>
        [PublicAPI]
        public static HStorageAudioClip AudioClip => StorageService.AudioClip;

        /// <summary>
        /// Provides access to the storage asset bundles API.
        /// </summary>
        [PublicAPI]
        public static HStorageAssetBundle AssetBundle => StorageService.AssetBundle;

        /// <summary>
        /// Provides access to the storage bytes API.
        /// </summary>
        [PublicAPI]
        public static HStorageBytes Bytes => StorageService.Bytes;

        static Implementation.StorageService StorageService => storageService ?? ( storageService = new Implementation.StorageService() );

        /// <summary>
        /// Gives information if the file at the specific path has an update available.
        /// Complete handler value <see cref="MetadataResultContainer"/> IsUpdateAvailable is set to true if the file was not downloaded yet, or update is available.
        /// </summary>
        /// <param name="filePath">A path to the file</param>
        /// <param name="completeHandler">A handler of action completed</param>
        [PublicAPI]
        public static void IsUpdateAvailable( string filePath, Action<MetadataResultContainer> completeHandler )
        {
            StorageService.IsUpdateAvailable( filePath, completeHandler );
        }

        /// <summary>
        /// Gives information if the file at the specified path has an update available. The handler value
        /// <see cref="MetadataResultContainer"/> IsUpdateAvailable is set to true the file was not downloaded yet,
        /// or an update is available. Otherwise false.
        /// </summary>
        /// <param name="filePath">A path to the file</param>
        /// <param name="completeHandler">A metadata result handler</param>
        /// <param name="serviceType">The service to check</param>
        [PublicAPI]
        public static void IsUpdateAvailable( string filePath,
            Action<MetadataResultContainer> completeHandler,
            StorageService serviceType )
        {
            StorageService.IsUpdateAvailable( filePath, completeHandler, serviceType );
        }

        /// <summary>
        /// Starts a file upload. The callback will be called after the upload process is completed.
        /// </summary>
        /// <param name="objectToUpload">An object to upload</param>
        /// <param name="filePath">A path that the object will be saved to</param>
        /// <param name="uploadResultHandler">An upload result handler</param>
        [PublicAPI]
        public static void UploadFile( object objectToUpload,
            string filePath,
            Action<StorageResultContainer> uploadResultHandler )
        {
            StorageService.UploadFile( objectToUpload, filePath, uploadResultHandler );
        }

        /// <summary>
        /// Starts a file upload. The callback will be called after the upload process is completed.
        /// </summary>
        /// <param name="objectToUpload">An object to upload</param>
        /// <param name="filePath">A path that object will be saved to</param>
        /// <param name="uploadResultHandler">A upload result handler</param>
        /// <param name="serviceType">The service to use</param>
        [PublicAPI]
        public static void UploadFile( object objectToUpload,
            string filePath,
            Action<StorageResultContainer> uploadResultHandler,
            StorageService serviceType )
        {
            StorageService.UploadFile( objectToUpload, filePath, uploadResultHandler, serviceType );
        }

        /// <summary>
        /// Removes the file at the given path. The callback will be called after the removal process is completed.
        /// </summary>
        /// <param name="filePath">A path to the file that will be removed</param>
        /// <param name="removeResultHandler">A remove result handler</param>
        [PublicAPI]
        public static void RemoveFile( string filePath, Action<StorageResultContainer> removeResultHandler )
        {
            StorageService.RemoveFile( filePath, removeResultHandler );
        }

        /// <summary>
        /// Removes a file at the given path. The callback will be called after the removal process is completed.
        /// </summary>
        /// <param name="filePath">A path to the file that will be removed</param>
        /// <param name="removeResultHandler">A remove result handler</param>
        /// <param name="serviceType">The service to use</param>
        [PublicAPI]
        public static void RemoveFile( string filePath,
            Action<StorageResultContainer> removeResultHandler,
            StorageService serviceType )
        {
            StorageService.RemoveFile( filePath, removeResultHandler, serviceType );
        }

        /// <summary>
        /// Use to check if the storage service is initialized.
        /// </summary>
        /// <param name="serviceType">The service to use</param>
        /// <returns>The service initialization status</returns>
        public static bool IsServiceInitialized( StorageService serviceType )
        {
            return storageService.IsServiceInitialized( serviceType );
        }

        /// <summary>
        /// Use to check if there is any storage service is initialized.
        /// </summary>
        /// <returns>The service initialization status</returns>
        public static bool IsAnyServiceInitialized()
        {
            return storageService.MainService != null;
        }

        /// <summary>
        /// Register the storage service for future use. The service is automatically initialized after the registration.
        /// </summary>
        /// <param name="downloadService"> A download service</param>
        /// <param name="uploadService">A download service - can be null</param>
        /// <param name="removeService">A download service - can be null</param>
        /// <param name="serviceType">A service id</param>
        /// <param name="isMain">Should be used as the default service if there will be a request without the service id</param>
        [PublicAPI]
        public static void RegisterService( IDownloadService downloadService,
            IUploadService uploadService,
            IRemoveService removeService,
            StorageService serviceType,
            bool isMain )
        {
            StorageService.RegisterService( downloadService, uploadService, removeService, serviceType, isMain );
        }
    }
}