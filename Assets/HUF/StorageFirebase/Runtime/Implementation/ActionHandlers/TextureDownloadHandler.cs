using System;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Storage;
using HUF.InitFirebase.Runtime;
using HUF.Storage.Runtime.Implementation.ActionHandlers;
using HUF.Storage.Runtime.Implementation.Structs;
using HUF.Utils.Runtime;
using UnityEngine;

namespace HUF.StorageFirebase.Runtime.Implementation.ActionHandlers
{
    public class TextureDownloadHandler : TextureLocalHandler
    {
        public TextureDownloadHandler( string fileId, Action<ObjectResultContainer<Texture2D>> completeHandler )
            : base( fileId, completeHandler ) { }

        public void DownloadFile( StorageReference storageReference )
        {
            storageReference.GetDownloadUrlAsync().ContinueWithOnMainThread( HandleGetUriCompleted );
        }

        void HandleGetUriCompleted( Task<Uri> task )
        {
            if ( task.IsFaulted || task.IsCanceled )
            {
                var errorMessage = task.Exception.GetFullErrorMessage();
                SendHandlerFail( errorMessage );
                return;
            }

            CoroutineManager.StartCoroutine( DownloadFromUrl( task.Result ) );
        }
    }
}