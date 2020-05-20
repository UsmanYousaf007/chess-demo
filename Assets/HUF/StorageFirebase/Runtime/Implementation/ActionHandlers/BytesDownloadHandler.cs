using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Storage;
using HUF.InitFirebase.Runtime;
using HUF.Storage.Runtime.API.Structs;
using HUF.Storage.Runtime.Implementation.ActionHandlers;
using UnityEngine.Events;

namespace HUF.StorageFirebase.Runtime.Implementation.ActionHandlers
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