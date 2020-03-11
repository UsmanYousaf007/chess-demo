using System.Text;
using Firebase.Storage;
using HUF.Auth.API;
using HUF.Storage.API;
using HUF.Storage.API.Structs;
using HUF.StorageFirebase.API;
using HUF.StorageFirebase.Implementation.ActionHandlers;
using HUF.Utils.Configs.API;
using HUF.Utils.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.StorageFirebase.Implementation
{
    public class FirebaseStorageRemoveService : IRemoveService
    {
        FirebaseStorage storageReference;
        readonly string logPrefix;

        public FirebaseStorageRemoveService(FirebaseStorage storageReference)
        {
            this.storageReference = storageReference;
            logPrefix = $"[{GetType().Name}]";
        }

        public void RemoveFile(string pathToFile, UnityAction<StorageResultContainer> completeHandler)
        {
            if (!HAuth.IsSignedIn(AuthServiceName.FIREBASE))
            {
                completeHandler.Dispatch(new StorageResultContainer(pathToFile,
                    FirebaseErrorMessages.FIREBASE_NOT_SIGNED_IN));
                
                Debug.LogError($"{logPrefix} {FirebaseErrorMessages.FIREBASE_NOT_SIGNED_IN}");
                return;
            }
            
            var config = HConfigs.GetConfig<FirebaseStorageConfig>();
            if (config == null || storageReference == null)
            {
                var errorMessage = storageReference == null
                    ? FirebaseErrorMessages.FIREBASE_STORAGE_MISSING
                    : FirebaseErrorMessages.CONFIG_MISSING_ERROR;
                
                completeHandler.Dispatch(new StorageResultContainer(pathToFile, errorMessage));
                
                Debug.LogError($"{logPrefix} {errorMessage}");
                return;
            }

            var firebasePathBuilder = new StringBuilder();
            firebasePathBuilder
                .Append(HAuth.GetUserId(AuthServiceName.FIREBASE))
                .Append('/')
                .Append(pathToFile);
            var firebasePath = firebasePathBuilder.ToString();

            var databaseUrlPath = StorageUtils.GetDatabaseUrlPath(config.DatabaseAddress, firebasePath);
            var fileReference = storageReference.GetReferenceFromUrl(databaseUrlPath);
            var storageRemoveHandler = new StorageRemoveHandler(firebasePath, completeHandler);
            storageRemoveHandler.StartRemove(fileReference);
        }


        public void Dispose()
        {
            storageReference = null;
        }
    }
}