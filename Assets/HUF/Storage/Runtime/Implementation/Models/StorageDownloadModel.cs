using System;
using System.Collections.Generic;
using System.IO;
using HUF.Storage.API;
using HUF.Storage.API.Structs;
using HUF.Utils.Runtime.Logging;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.Storage.Implementation.Models
{
    public class StorageDownloadModel : IDownloadService
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix(HStorage.logPrefix, nameof(StorageDownloadModel) );
        readonly IDownloadService downloadService;
        readonly LocalStorageService localStorageService;
        readonly HashSet<string> downloadsInProgressSet;
        readonly Dictionary<string,Delegate> multipleDownloadHelper = new Dictionary<string,Delegate>();

        public StorageDownloadModel(IDownloadService downloadService)
        {
            this.downloadService = downloadService;
            localStorageService = new LocalStorageService();
            downloadsInProgressSet = new HashSet<string>();
        }

        public void GetFileBytes(
            string filePath,
            UnityAction<ObjectResultContainer<byte[]>> completeHandler,
            bool forceDownload = false)
        {
            if (IsFileDownloadInProgress(filePath, completeHandler))
            {
                return;
            }

            completeHandler += OnGetFileComplete;

            if (!forceDownload && FileExist(filePath))
            {
                HLog.Log(logPrefix,"Using previously downloaded FileBytes");
                localStorageService.GetFileBytes(filePath, completeHandler);
            }
            else
            {
                HLog.Log(logPrefix,"Download new FileBytes");
                downloadService.GetFileBytes(filePath, completeHandler, forceDownload);
            }
        }

        public void GetTexture(
            string filePath,
            UnityAction<ObjectResultContainer<Texture2D>> completeHandler,
            bool forceDownload = false)
        {
            if (IsFileDownloadInProgress(filePath, completeHandler))
            {
                return;
            }

            completeHandler += OnGetFileComplete;

            if (!forceDownload && FileExist(filePath))
            {
                HLog.Log(logPrefix,"Using previously downloaded Texture");
                localStorageService.GetTexture(filePath, completeHandler);
            }
            else
            {
                HLog.Log(logPrefix,"Download new Texture");
                downloadService.GetTexture(filePath, completeHandler, forceDownload);
            }
        }

        public void GetAudioClip(
            string filePath,
            UnityAction<ObjectResultContainer<AudioClip>> completeHandler,
            bool forceDownload = false)
        {
            if (IsFileDownloadInProgress(filePath, completeHandler))
            {
                return;
            }

            completeHandler += OnGetFileComplete;

            if (!forceDownload && FileExist(filePath))
            {
                HLog.Log(logPrefix,"Using previously downloaded Audio");
                localStorageService.GetAudioClip(filePath, completeHandler);
            }
            else
            {
                HLog.Log(logPrefix,"Download new Audio");
                downloadService.GetAudioClip(filePath, completeHandler, forceDownload);
            }
        }

        public void GetAssetBundle(
            string filePath,
            UnityAction<ObjectResultContainer<AssetBundle>> completeHandler,
            bool forceDownload = false)
        {
            if (IsFileDownloadInProgress(filePath, completeHandler))
            {
                return;
            }

            completeHandler += OnGetFileComplete;

            if (!forceDownload && FileExist(filePath))
            {
                HLog.Log(logPrefix,"Using previously downloaded Asset Bundle");
                localStorageService.GetAssetBundle(filePath, completeHandler);
            }
            else
            {
                HLog.Log(logPrefix,"Download new Asset Bundle");
                downloadService.GetAssetBundle(filePath, completeHandler, forceDownload);
            }
        }

        public void GetUpdateInfo(string filePath, UnityAction<MetadataResultContainer> completeHandler)
        {
            downloadService.GetUpdateInfo(filePath, completeHandler);
        }

        static bool FileExist(string filePath)
        {
            return File.Exists(StorageUtils.GetLocalFilePath(filePath));
        }

        bool IsFileDownloadInProgress<T>(
            string filePath,
            UnityAction<ObjectResultContainer<T>> completeHandler) where T : class
        {

            if (downloadsInProgressSet.Contains(filePath))
            {
                if (multipleDownloadHelper.ContainsKey(filePath))
                {
                    multipleDownloadHelper[filePath] = Delegate.Combine(multipleDownloadHelper[filePath], completeHandler);
                }
                else
                {
                    multipleDownloadHelper[filePath] = completeHandler;
                }
                HLog.Log(logPrefix,$"{StorageErrorMessages.DOWNLOAD_ALREADY_IN_PROGRESS}: {filePath}");
                return true;
            }
            downloadsInProgressSet.Add(filePath);
            return false;
        }

        void OnGetFileComplete<T>(ObjectResultContainer<T> result) where T : class
        {
            string path = result.StorageResultContainer.PathToFile;
            HLog.Log(logPrefix,$"Get data {path} result IsSuccess {result.IsSuccess}");
            downloadsInProgressSet.Remove(path);

            if (multipleDownloadHelper.ContainsKey(path))
            {
                multipleDownloadHelper[path].DynamicInvoke(result);
                multipleDownloadHelper.Remove(path);
            }
        }

        public void Dispose()
        {
            downloadService.Dispose();
        }
    }
}