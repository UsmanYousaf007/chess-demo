using System;
using System.Collections.Generic;
using System.IO;
using HUF.Storage.Runtime.API;
using HUF.Storage.Runtime.API.Models;
using HUF.Storage.Runtime.API.Services;
using HUF.Storage.Runtime.Implementation.Structs;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using UnityEngine;

namespace HUF.Storage.Runtime.Implementation
{
    public class StorageService
    {
        public event Action<API.StorageService> OnInit;
        public static readonly HLogPrefix logPrefix = new HLogPrefix( HStorage.logPrefix, nameof(HStorage) );

        readonly LocalStorageService localStorageService;
        readonly HashSet<string> downloadsInProgressSet;
        readonly Dictionary<string, Delegate> multipleDownloadHelper = new Dictionary<string, Delegate>();

        API.StorageService? mainService = null;
        readonly HStorageTexture texture;
        readonly HStorageAudioClip audioClip;
        readonly HStorageAssetBundle assetBundle;
        readonly HStorageBytes bytes;
        readonly IDownloadService[] downloadService;
        readonly IUploadService[] uploadService;
        readonly IRemoveService[] removeService;

        public StorageService()
        {
            texture = new HStorageTexture( this );
            audioClip = new HStorageAudioClip( this );
            assetBundle = new HStorageAssetBundle( this );
            bytes = new HStorageBytes( this );
            localStorageService = new LocalStorageService();
            downloadsInProgressSet = new HashSet<string>();
            var count = Enum.GetNames( typeof(API.StorageService) ).Length;
            downloadService = new IDownloadService[count];
            uploadService = new IUploadService[count];
            removeService = new IRemoveService[count];
        }

        public API.StorageService? MainService => mainService;
        public HStorageTexture Texture => texture;
        public HStorageAudioClip AudioClip => audioClip;
        public HStorageAssetBundle AssetBundle => assetBundle;
        public HStorageBytes Bytes => bytes;
        internal IDownloadService[] DownloadService => downloadService;
        internal IUploadService[] UploadService => uploadService;
        internal IRemoveService[] RemoveService => removeService;

        public void RegisterService( IDownloadService inDownloadService,
            IUploadService inUploadService,
            IRemoveService inRemoveService,
            API.StorageService serviceType,
            bool isMain )
        {
            if ( isMain || mainService == null )
                mainService = serviceType;

            if ( inDownloadService != null )
            {
                if ( inDownloadService.IsInitialized )
                {
                    OnInit.Dispatch( serviceType );
                }

                inDownloadService.OnInit += HandleServiceInit;
            }

            downloadService[(int)serviceType] = inDownloadService;
            uploadService[(int)serviceType] = inUploadService;
            removeService[(int)serviceType] = inRemoveService;
        }

        public void IsUpdateAvailable( string filePath,
            Action<MetadataResultContainer> completeHandler,
            API.StorageService? serviceType = null )
        {
            if ( !CheckDownloadServiceExist( serviceType, out int storageIndex ) )
            {
                completeHandler.Dispatch( new MetadataResultContainer(
                    new StorageResultContainer( filePath, StorageErrorMessages.STORAGE_NOT_INITIALIZED ) ) );
                return;
            }

            downloadService[storageIndex].GetUpdateInfo( filePath, completeHandler );
        }

        public void UploadFile( object objectToUpload,
            string pathToFile,
            Action<StorageResultContainer> completeHandler,
            API.StorageService? serviceType = null )
        {
            if ( !StorageUtils.TrySaveObject( objectToUpload, pathToFile ) )
            {
                completeHandler.Dispatch( new StorageResultContainer( pathToFile,
                    $"{StorageErrorMessages.ERROR_DURING_SAVING}.({pathToFile})" ) );
                return;
            }

            if ( !CheckUploadServiceExist( serviceType, out int storageIndex ) )
            {
                completeHandler.Dispatch( new StorageResultContainer( pathToFile,
                    StorageErrorMessages.STORAGE_NOT_INITIALIZED ) );
                return;
            }

            uploadService[storageIndex].UploadFile( pathToFile, objectToUpload, completeHandler );
        }

        public void RemoveFile( string pathToFile,
            Action<StorageResultContainer> completeHandler,
            API.StorageService? serviceType = null )
        {
            if ( !CheckRemoveServiceExist( serviceType, out int storageIndex ) )
            {
                completeHandler.Dispatch( new StorageResultContainer( pathToFile,
                    StorageErrorMessages.STORAGE_NOT_INITIALIZED ) );
                return;
            }

            File.Delete( StorageUtils.GetLocalFilePath( pathToFile ) );
            removeService[storageIndex].RemoveFile( pathToFile, completeHandler );
        }

        public void GetFileBytes(
            string filePath,
            Action<ObjectResultContainer<byte[]>> completeHandler,
            bool forceDownload,
            API.StorageService? serviceType )
        {
            if ( IsFileDownloadInProgress( filePath, completeHandler ) )
                return;

            completeHandler += OnGetFileComplete;

            bool serviceExist = CheckDownloadServiceExist( serviceType, out int storageServiceIndex );

            if ( !forceDownload && FileExist( filePath, serviceExist, storageServiceIndex, out string storageFilePath) )
            {
                HLog.Log( logPrefix, "Using previously downloaded FileBytes" );
                localStorageService.GetFileBytes( filePath, completeHandler, storageFilePath );
                return;
            }

            if ( !serviceExist )
            {
                completeHandler.Dispatch( new ObjectResultContainer<byte[]>(
                    new StorageResultContainer( filePath, StorageErrorMessages.STORAGE_NOT_INITIALIZED ) ) );
                return;
            }

            HLog.Log( logPrefix, "Download new FileBytes" );
            downloadService[storageServiceIndex].GetFileBytes( filePath, completeHandler );
        }

        public void GetTexture(
            string filePath,
            Action<ObjectResultContainer<Texture2D>> completeHandler,
            bool forceDownload,
            API.StorageService? serviceType )
        {
            if ( IsFileDownloadInProgress( filePath, completeHandler ) )
                return;

            completeHandler += OnGetFileComplete;

            bool serviceExist = CheckDownloadServiceExist( serviceType, out int storageServiceIndex );

            if ( !forceDownload && FileExist( filePath, serviceExist, storageServiceIndex, out string storageFilePath) )
            {
                HLog.Log( logPrefix, "Using previously downloaded Texture" );
                localStorageService.GetTexture( filePath, completeHandler , storageFilePath);
                return;
            }

            if ( !serviceExist )
            {
                completeHandler.Dispatch( new ObjectResultContainer<Texture2D>(
                    new StorageResultContainer( filePath, StorageErrorMessages.STORAGE_NOT_INITIALIZED ) ) );
                return;
            }

            HLog.Log( logPrefix, "Download new Texture" );
            downloadService[storageServiceIndex].GetTexture( filePath, completeHandler );

        }

        public void GetAudioClip(
            string filePath,
            Action<ObjectResultContainer<AudioClip>> completeHandler,
            bool forceDownload,
            API.StorageService? serviceType )
        {
            if ( IsFileDownloadInProgress( filePath, completeHandler ) )
                return;

            completeHandler += OnGetFileComplete;

            bool serviceExist = CheckDownloadServiceExist( serviceType, out int storageServiceIndex );

            if ( !forceDownload && FileExist( filePath, serviceExist, storageServiceIndex, out string storageFilePath) )
            {
                HLog.Log( logPrefix, "Using previously downloaded Audio" );
                localStorageService.GetAudioClip( filePath, completeHandler, storageFilePath );
                return;
            }

            if ( !serviceExist )
            {
                completeHandler.Dispatch( new ObjectResultContainer<AudioClip>(
                    new StorageResultContainer( filePath, StorageErrorMessages.STORAGE_NOT_INITIALIZED ) ) );
                return;
            }

            HLog.Log( logPrefix, "Download new Audio" );
            downloadService[storageServiceIndex].GetAudioClip( filePath, completeHandler );

        }

        public void GetAssetBundle(
            string filePath,
            Action<ObjectResultContainer<AssetBundle>> completeHandler,
            bool forceDownload,
            API.StorageService? serviceType )
        {
            if ( IsFileDownloadInProgress( filePath, completeHandler ) )
                return;

            completeHandler += OnGetFileComplete;

            bool serviceExist = CheckDownloadServiceExist( serviceType, out int storageServiceIndex );

            if ( !forceDownload && FileExist( filePath, serviceExist, storageServiceIndex, out string storageFilePath) )
            {
                HLog.Log( logPrefix, "Using previously downloaded Asset Bundle" );
                localStorageService.GetAssetBundle( filePath, completeHandler , storageFilePath);
                return;
            }

            if ( !serviceExist )
            {
                completeHandler.Dispatch( new ObjectResultContainer<AssetBundle>(
                    new StorageResultContainer( filePath, StorageErrorMessages.STORAGE_NOT_INITIALIZED ) ) );
                return;
            }

            HLog.Log( logPrefix, "Download new Asset Bundle" );
            downloadService[storageServiceIndex].GetAssetBundle( filePath, completeHandler );
        }

        public bool IsServiceInitialized( API.StorageService storageService )
        {
            return downloadService[(int)storageService] != null && downloadService[(int)storageService].IsInitialized;
        }

        void HandleServiceInit( API.StorageService service )
        {
            OnInit.Dispatch( service );
        }

        bool CheckDownloadServiceExist( API.StorageService? serviceType, out int storageIndex )
        {
            if ( serviceType == null )
                serviceType = mainService;
            storageIndex = serviceType != null ? (int)serviceType : -1;
            return storageIndex >= 0 && downloadService[storageIndex] != null;
        }

        bool CheckUploadServiceExist( API.StorageService? serviceType, out int storageIndex )
        {
            if ( serviceType == null )
                serviceType = mainService;
            storageIndex = serviceType != null ? (int)serviceType : -1;
            return storageIndex >= 0 && downloadService[storageIndex] != null;
        }

        bool CheckRemoveServiceExist( API.StorageService? serviceType, out int storageIndex )
        {
            if ( serviceType == null )
                serviceType = mainService;
            storageIndex = serviceType != null ? (int)serviceType : -1;
            return storageIndex >= 0 && downloadService[storageIndex] != null;
        }

        bool FileExist( string filePath, bool serviceExist, int storageServiceIndex, out string storageFilePath)
        {
            storageFilePath = serviceExist ? downloadService[storageServiceIndex].GetRemotePath(filePath) : filePath;
            return File.Exists( StorageUtils.GetLocalFilePath( storageFilePath ) );
        }

        bool IsFileDownloadInProgress<T>(
            string filePath,
            Action<ObjectResultContainer<T>> completeHandler ) where T : class
        {
            if ( downloadsInProgressSet.Contains( filePath ) )
            {
                if ( multipleDownloadHelper.ContainsKey( filePath ) )
                    multipleDownloadHelper[filePath] =
                        Delegate.Combine( multipleDownloadHelper[filePath], completeHandler );
                else
                    multipleDownloadHelper[filePath] = completeHandler;

                HLog.Log( logPrefix, $"{StorageErrorMessages.DOWNLOAD_ALREADY_IN_PROGRESS}: {filePath}" );
                return true;
            }

            downloadsInProgressSet.Add( filePath );
            return false;
        }

        void OnGetFileComplete<T>( ObjectResultContainer<T> result ) where T : class
        {
            string path = result.StorageResultContainer.PathToFile;
            HLog.Log( logPrefix, $"Get data {path} result IsSuccess {result.IsSuccess}" );
            downloadsInProgressSet.Remove( path );

            if ( !multipleDownloadHelper.ContainsKey( path ) )
                return;

            multipleDownloadHelper[path].DynamicInvoke( result );
            multipleDownloadHelper.Remove( path );
        }
    }
}