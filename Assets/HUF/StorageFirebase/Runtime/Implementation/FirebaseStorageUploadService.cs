using System;
using System.Text;
using Firebase.Storage;
#if HUF_AUTH_FIREBASE
using HUF.Auth.Runtime.API;
#endif
using HUF.Storage.Runtime.API.Services;
using HUF.Storage.Runtime.Implementation;
using HUF.Storage.Runtime.Implementation.Structs;
using HUF.StorageFirebase.Runtime.API;
using HUF.StorageFirebase.Runtime.Implementation.ActionHandlers;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;

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
            Action<StorageResultContainer> completeHandler )
        {
#if HUF_AUTH_FIREBASE
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
#else
            HLog.LogError( logPrefix, "Firebase Auth package is needed to upload files!" );
#endif
        }

        public void Dispose()
        {
            storageReference = null;
        }
    }
}