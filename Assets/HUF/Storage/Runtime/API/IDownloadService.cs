using System;
using HUF.Storage.Runtime.Implementation.Structs;
using UnityEngine;

namespace HUF.Storage.Runtime.API.Services
{
    public interface IDownloadService
    {
        event Action<StorageService> OnInit;

        bool IsInitialized { get; }
        void GetFileBytes( string filePath, Action<ObjectResultContainer<byte[]>> completeHandler );
        void GetTexture( string filePath, Action<ObjectResultContainer<Texture2D>> completeHandler );
        void GetAudioClip( string filePath, Action<ObjectResultContainer<AudioClip>> completeHandler );
        void GetAssetBundle( string filePath, Action<ObjectResultContainer<AssetBundle>> completeHandler );
        void GetUpdateInfo( string filePath, Action<MetadataResultContainer> completeHandler );

        string GetRemotePath( string filePath );
    }
}