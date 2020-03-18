using HUF.Storage.API.Structs;
using HUF.Utils.Extensions;
using JetBrains.Annotations;
using UnityEngine.Events;

namespace HUF.Storage.API.Models
{
    public class HStorageBytes : IObjectStorage<byte[]>
    {
        readonly StorageModel storage;

        public HStorageBytes(StorageModel storageModel)
        {
            storage = storageModel;
        }

        /// <summary>
        /// Gets bytes either from remote or local storage. Complete handler is method that will handle download process after
        /// its completed. If success Container will have path to file and byte[]. If failure Container will have path to file
        /// and failure reason. 
        /// </summary>
        /// <param name="filePath">Path to file (database part added automatically)</param>
        /// <param name="resultHandler">Download completion handler</param>
        [PublicAPI]
        public void Get(string filePath, UnityAction<ObjectResultContainer<byte[]>> resultHandler)
        {
            GetBytes(filePath, resultHandler, false);
        }

        /// <summary>
        /// Downloads bytes from remote storage. Complete handler is method that will handle download process after
        /// its completed. If success Container will have path to file and byte[]. If failure Container will have path to file
        /// and failure reason. 
        /// </summary>
        /// <param name="filePath">Path to file (database part added automatically)</param>
        /// <param name="resultHandler">Download completion handler</param>
        [PublicAPI]
        public void Download(string filePath, UnityAction<ObjectResultContainer<byte[]>> resultHandler)
        {
            GetBytes(filePath, resultHandler, true);
        }

        void GetBytes(string filePath, UnityAction<ObjectResultContainer<byte[]>> resultHandler, bool forceDownload)
        {
            if (storage?.DownloadService == null)
                resultHandler.Dispatch(new ObjectResultContainer<byte[]>(
                    new StorageResultContainer(filePath, StorageErrorMessages.STORAGE_NOT_INITIALIZED)));
            else
                storage?.DownloadService?.GetFileBytes(filePath, resultHandler, forceDownload);
        }
    }
}