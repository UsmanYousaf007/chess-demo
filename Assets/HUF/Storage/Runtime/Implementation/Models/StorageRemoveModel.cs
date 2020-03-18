using System.IO;
using HUF.Storage.API;
using HUF.Storage.API.Structs;
using UnityEngine.Events;

namespace HUF.Storage.Implementation.Models
{
    public class StorageRemoveModel : IRemoveService
    {
        readonly IRemoveService removeService;

        public StorageRemoveModel(IRemoveService removeService)
        {
            this.removeService = removeService;
        }

        public void RemoveFile(string pathToFile, UnityAction<StorageResultContainer> completeHandler)
        {
            File.Delete(StorageUtils.GetLocalFilePath(pathToFile));
            removeService.RemoveFile(pathToFile, completeHandler);
        }

        public void Dispose()
        {
            removeService.Dispose();
        }
    }
}