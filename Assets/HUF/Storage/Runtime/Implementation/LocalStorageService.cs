using HUF.Storage.API;
using HUF.Storage.API.Structs;
using HUF.Storage.Implementation.ActionHandlers;
using HUF.Utils.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.Storage.Implementation
{
    public class LocalStorageService : IDownloadService
    {   
        public void GetFileBytes(string filePath, UnityAction<ObjectResultContainer<byte[]>> completeHandler,
            bool forceDownload = false)
        {
            new BytesLocalHandler(filePath, completeHandler).ReadLocalFile();
        }

        public void GetTexture(string filePath, UnityAction<ObjectResultContainer<Texture2D>> completeHandler,
            bool forceDownload = false)
        {
            new TextureLocalHandler(filePath, completeHandler).ReadLocalFile();
        }

        public void GetAudioClip(string filePath, UnityAction<ObjectResultContainer<AudioClip>> completeHandler,
            bool forceDownload = false)
        {
            new AudioClipLocalHandler(filePath, completeHandler).ReadLocalFile();
        }

        public void GetAssetBundle(string filePath, UnityAction<ObjectResultContainer<AssetBundle>> completeHandler,
            bool forceDownload = false)
        {
            new AssetBundleLocalHandler(filePath, completeHandler).ReadLocalFile();
        }

        public void GetUpdateInfo(string filePath, UnityAction<MetadataResultContainer> completeHandler)
        {
            completeHandler.Dispatch(new MetadataResultContainer(new StorageResultContainer(filePath)));
        }

        public void Dispose()
        {
        }
    }
}