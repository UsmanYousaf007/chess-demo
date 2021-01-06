using HUF.Storage.Runtime.API.Structs;
using HUF.Utils.Runtime.Extensions;
using JetBrains.Annotations;
using UnityEngine.Events;

namespace HUF.Storage.Runtime.API.Models
{
    public class HStorageBytes : IObjectStorage<byte[]>
    {
        readonly StorageModel storage;

        public HStorageBytes(StorageModel storageModel)
        {
            storage = storageModel;
        }

        /// <summary>
        /// Gets the bytes array either from the remote or local storage. The handler will be called after the process 
        /// is completed. If it was successful, the container will contain the path to the file and the bytes array.
        /// If it has failed, the container will contain the path to the file and the failure reason. 
        /// </summary>
        /// <param name="filePath">A path to the file (database part added automatically).</param>
        /// <param name="resultHandler">A download completion handler.</param>
        [PublicAPI]
        public void Get(string filePath, UnityAction<ObjectResultContainer<byte[]>> resultHandler)
        {
            GetBytes(filePath, resultHandler, false);
        }

        /// <summary>
        /// Downloads the bytes array from the remote storage. The handler will be called after the download process
        /// is completed. If it was successful, the container will contain the path to the file and the bytes array.
        /// If it has failed, the container will contain the path to the file and the failure reason. 
        /// </summary>
        /// <param name="filePath">A path to the file (database part added automatically).</param>
        /// <param name="resultHandler">A download completion handler.</param>
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