using System;
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

namespace HUF.StorageFirebase.Implementation.ActionHandlers
{
    public class TextureDownloadHandler : TextureLocalHandler
    {
        public TextureDownloadHandler(string filePath, UnityAction<ObjectResultContainer<Texture2D>> completeHandler) 
            : base(filePath, completeHandler)
        {
        }

        public void DownloadFile(StorageReference textureReference)
        {
            textureReference.GetDownloadUrlAsync().ContinueWithOnMainThread(HandleGetUriCompleted);
        }

        void HandleGetUriCompleted(Task<Uri> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                var errorMessage = task.Exception.GetFullErrorMessage();
                SendHandlerFail(errorMessage);
                return;
            }
            CoroutineManager.StartCoroutine(DownloadFromUrl(task.Result));
        }
        
    }
}