using System;
using HUF.Storage.Runtime.API.Services;
using HUF.Storage.Runtime.Implementation.ActionHandlers;
using HUF.Storage.Runtime.Implementation.Structs;
using HUF.Utils.Runtime.Extensions;
using UnityEngine;

namespace HUF.Storage.Runtime.Implementation
{
    public class LocalStorageService : IDownloadService
    {
        public event Action<API.StorageService> OnInit;

        public bool IsInitialized => true;

        public void GetFileBytes(string fileId, Action<ObjectResultContainer<byte[]>> completeHandler)
        {
            new BytesLocalHandler(fileId, completeHandler).DownloadFile();
        }

        public void GetTexture(string fileId, Action<ObjectResultContainer<Texture2D>> completeHandler)
        {
            new TextureLocalHandler(fileId, completeHandler).DownloadFile();
        }

        public void GetAudioClip(string fileId, Action<ObjectResultContainer<AudioClip>> completeHandler)
        {
            new AudioClipLocalHandler(fileId, completeHandler).DownloadFile();
        }

        public void GetAssetBundle(string fileId, Action<ObjectResultContainer<AssetBundle>> completeHandler)
        {
            new AssetBundleLocalHandler(fileId, completeHandler).DownloadFile();
        }

        public void GetFileBytes(string fileId, Action<ObjectResultContainer<byte[]>> completeHandler, string filePath)
        {
            new BytesLocalHandler(fileId, completeHandler, filePath).DownloadFile();
        }

        public void GetTexture(string fileId, Action<ObjectResultContainer<Texture2D>> completeHandler, string filePath)
        {
            new TextureLocalHandler(fileId, completeHandler, filePath).DownloadFile();
        }

        public void GetAudioClip(string fileId, Action<ObjectResultContainer<AudioClip>> completeHandler, string filePath)
        {
            new AudioClipLocalHandler(fileId, completeHandler, filePath).DownloadFile();
        }

        public void GetAssetBundle(string fileId, Action<ObjectResultContainer<AssetBundle>> completeHandler, string filePath)
        {
            new AssetBundleLocalHandler(fileId, completeHandler, filePath).DownloadFile();
        }

        public void GetUpdateInfo(string fileId, Action<MetadataResultContainer> completeHandler)
        {
            completeHandler.Dispatch(new MetadataResultContainer(new StorageResultContainer(fileId)));
        }

        public string GetRemotePath( string fileId )
        {
            return fileId;
        }
    }
}