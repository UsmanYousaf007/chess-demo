using System;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Storage;
using HUF.InitFirebase.Runtime;
using HUF.Storage.Runtime.API.Structs;
using HUF.Storage.Runtime.Implementation.ActionHandlers;
using HUF.Utils.Runtime;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.StorageFirebase.Runtime.Implementation.ActionHandlers
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