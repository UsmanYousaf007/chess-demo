using System;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Storage;
using HUF.InitFirebase.Runtime;
using HUF.Storage.Runtime.Implementation.ActionHandlers;
using HUF.Storage.Runtime.Implementation.Structs;

namespace HUF.StorageFirebase.Runtime.Implementation.ActionHandlers
{
    public class BytesDownloadHandler : BytesLocalHandler
    {
        public BytesDownloadHandler( string fileId, Action<ObjectResultContainer<byte[]>> completeHandler )
            : base( fileId, completeHandler ) { }

        public void DownloadFile( StorageReference storageReference )
        {
            storageReference.GetBytesAsync( int.MaxValue ).ContinueWithOnMainThread( HandleDownloadComplete );
        }

        void HandleDownloadComplete( Task<byte[]> task )
        {
            if ( task.IsFaulted || task.IsCanceled )
            {
                var fullError = task.Exception.GetFullErrorMessage();
                SendHandlerFail( fullError );
                return;
            }

            SendHandlerSuccess( task.Result, task.Result );
        }
    }
}