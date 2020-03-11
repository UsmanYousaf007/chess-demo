using System;
using HUF.Storage.API.Structs;
using HUF.Storage.Implementation.ActionHandlers;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.Storage.API
{
    public interface IDownloadService : IDisposable
    {
        void GetFileBytes(string filePath, UnityAction<ObjectResultContainer<byte[]>> completeHandler,
            bool forceDownload = false);
        void GetTexture(string filePath, UnityAction<ObjectResultContainer<Texture2D>> completeHandler,
            bool forceDownload = false);
        void GetAudioClip(string filePath, UnityAction<ObjectResultContainer<AudioClip>> completeHandler,
            bool forceDownload = false);
        void GetAssetBundle(string filePath, UnityAction<ObjectResultContainer<AssetBundle>> completeHandler,
            bool forceDownload = false);
        void GetUpdateInfo(string filePath, UnityAction<MetadataResultContainer> completeHandler);
    }
}