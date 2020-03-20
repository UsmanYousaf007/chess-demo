using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Storage;
using HUF.InitFirebase;
using HUF.Storage.API;
using HUF.Storage.API.Structs;
using HUF.Storage.Implementation.ActionHandlers;
using HUF.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace HUF.StorageFirebase.Implementation.ActionHandlers
{
    public class AssetBundleDownloadHandler : AssetBundleLocalHandler
    {
        public AssetBundleDownloadHandler(string filePath, UnityAction<ObjectResultContainer<AssetBundle>> completeHandler) 
            : base(filePath, completeHandler)
        {
        }

        public void DownloadFile(StorageReference assetBundleReference, bool forceDownload)
        {
            if (forceDownload)
                TryGetAssetBundle(Path.GetFileName(FilePath))?.Unload(false);
            assetBundleReference.GetDownloadUrlAsync().ContinueWithOnMainThread(HandleUriDownloadComplete);
        }
        
        void HandleUriDownloadComplete(Task<Uri> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                SendHandlerFail(task.Exception.GetFullErrorMessage());
                return;
            }
            CoroutineManager.StartCoroutine(DownloadFile(task.Result));
        }

        IEnumerator DownloadFile(Uri uri)
        {
            var request = UnityWebRequest.Get(uri);
            yield return request.SendWebRequest();

            if (request.isHttpError || request.isNetworkError)
            {
                SendHandlerFail(request.error);
                yield break;
            }
            var assetBundle = AssetBundle.LoadFromMemory(request.downloadHandler.data);
            SendHandlerSuccess(assetBundle, request.downloadHandler.data);
        }
    }
}