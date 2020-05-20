using System.Text;
using Firebase.Storage;
using HUF.Auth.Runtime.API;
using HUF.Storage.Runtime.API;
using HUF.Storage.Runtime.API.Structs;
using HUF.StorageFirebase.Runtime.API;
using HUF.StorageFirebase.Runtime.Implementation.ActionHandlers;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.StorageFirebase.Runtime.Implementation
{
    public class FirebaseStorageUploadService : IUploadService
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(FirebaseStorageUploadService) );

        FirebaseStorage storageReference;

        public FirebaseStorageUploadService( FirebaseStorage storageReference )
        {
            this.storageReference = storageReference;
        }

        public void UploadFile( string pathToFile,
            object objectToUpload,
            UnityAction<StorageResultContainer> completeHandler )
        {
            if ( !HAuth.IsSignedIn( AuthServiceName.FIREBASE ) )
            {
                completeHandler.Dispatch( new StorageResultContainer( pathToFile,
                    FirebaseErrorMessages.FIREBASE_NOT_SIGNED_IN ) );
                HLog.LogError( logPrefix, $"{FirebaseErrorMessages.FIREBASE_NOT_SIGNED_IN}" );
                return;
            }

            var config = HConfigs.GetConfig<FirebaseStorageConfig>();

            if ( config == null || storageReference == null )
            {
                var errorMessage = storageReference == null
                    ? FirebaseErrorMessages.FIREBASE_STORAGE_MISSING
                    : FirebaseErrorMessages.CONFIG_MISSING_ERROR;
                completeHandler.Dispatch( new StorageResultContainer( pathToFile, errorMessage ) );
                HLog.LogError( logPrefix, errorMessage );
                return;
            }

            var firebasePath = new StringBuilder( HAuth.GetUserId( AuthServiceName.FIREBASE ) )
                .Append( '/' )
                .Append( pathToFile )
                .ToString();
            var storageFileUrl = StorageUtils.GetDatabaseUrlPath( config.DatabaseAddress, firebasePath );
            var fileReference = storageReference.GetReferenceFromUrl( storageFileUrl );
            var storageUploadHandler = new StorageUploadHandler( completeHandler );
            storageUploadHandler.UploadFile( fileReference, objectToUpload );
        }

        public void Dispose()
        {
            storageReference = null;
        }
    }
}