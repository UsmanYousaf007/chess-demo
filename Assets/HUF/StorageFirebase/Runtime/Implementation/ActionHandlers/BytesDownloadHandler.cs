using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Storage;
using HUF.InitFirebase;
using HUF.Storage.API;
using HUF.Storage.API.Structs;
using HUF.Storage.Implementation.ActionHandlers;
using UnityEngine.Events;

namespace HUF.StorageFirebase.Implementation.ActionHandlers
{
    public class BytesDownloadHandler : BytesLocalHandler
    {
        public BytesDownloadHandler(string filePath, UnityAction<ObjectResultContainer<byte[]>> completeHandler) 
            : base(filePath, completeHandler)
        {
        }

        public void DownloadFile(StorageReference bytesFileReference)
        {
            bytesFileReference.GetBytesAsync(int.MaxValue).ContinueWithOnMainThread(HandleDownloadComplete);
        }

        void HandleDownloadComplete(Task<byte[]> task)
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                var fullError = task.Exception.GetFullErrorMessage();
                SendHandlerFail(fullError);
                return;
            }
            SendHandlerSuccess(task.Result, task.Result);
        }
    }
}