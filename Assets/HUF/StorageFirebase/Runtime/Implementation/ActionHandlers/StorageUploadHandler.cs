using System;
using System.Text;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Storage;
using HUF.Storage.Runtime.Implementation.Structs;
using HUF.StorageFirebase.Runtime.API;
using HUF.Utils.Runtime.Extensions;

namespace HUF.StorageFirebase.Runtime.Implementation.ActionHandlers
{
    public class StorageUploadHandler
    {
        readonly Action<StorageResultContainer> completeHandler;

        public StorageUploadHandler( Action<StorageResultContainer> completeHandler )
        {
            this.completeHandler = completeHandler;
        }

        public void UploadFile( StorageReference fileReference, object objectToUpload )
        {
            byte[] byteData;

            if ( objectToUpload.GetType() == typeof(byte[]) )
            {
                byteData = objectToUpload as byte[];
            }
            else
            {
                byteData = objectToUpload.SerializeToByteArray();
            }

            fileReference.PutBytesAsync( byteData ).ContinueWithOnMainThread( HandleUploadComplete );
        }

        void HandleUploadComplete( Task<StorageMetadata> task )
        {
            if ( task.IsFaulted || task.IsCanceled )
            {
                var errorMessageBuilder = new StringBuilder();

                errorMessageBuilder
                    .Append( FirebaseErrorMessages.UPLOAD_FAILED_OR_CANCELED )
                    .Append( task.Exception?.Message );
                var errorMessage = errorMessageBuilder.ToString();
                completeHandler.Dispatch( new StorageResultContainer( task.Result.Path, errorMessage ) );
            }
            else
            {
                completeHandler.Dispatch( new StorageResultContainer( task.Result.Path ) );
            }
        }
    }
}