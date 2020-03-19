using HUF.Storage.API;
using HUF.Storage.API.Structs;
using HUF.Utils.Extensions;
using UnityEngine.Events;

namespace HUF.Storage.Implementation.Models
{
    public class StorageUploadModel : IUploadService
    {
        IUploadService uploadService;

        public StorageUploadModel(IUploadService uploadService)
        {
            this.uploadService = uploadService;
        }

        public void UploadFile(string pathToFile, object objectToUpload,
            UnityAction<StorageResultContainer> completeHandler)
        {
            if (!StorageUtils.TrySaveObject(objectToUpload, pathToFile))
            {
                completeHandler.Dispatch(new StorageResultContainer(pathToFile,
                    $"{StorageErrorMessages.ERROR_DURING_SAVING}.({pathToFile})"));
                return;
            }
            uploadService.UploadFile(pathToFile, objectToUpload, completeHandler);
        }

        public void Dispose()
        {
            uploadService.Dispose();
            uploadService = null;
        }
    }
}