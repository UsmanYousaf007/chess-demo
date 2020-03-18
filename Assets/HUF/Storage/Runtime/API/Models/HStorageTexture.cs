using HUF.Storage.API.Structs;
using HUF.Utils.Extensions;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.Storage.API.Models
{
    public class HStorageTexture : IObjectStorage<Texture2D>
    {
        readonly StorageModel storage;

        public HStorageTexture(StorageModel storageModel)
        {
            storage = storageModel;
        }

        /// <summary>
        /// Gets texture either from remote or local storage. Complete handler is method that will handle download process after
        /// its completed. If success Container will have path to file and texture2d. If failure Container will have path to file
        /// and failure reason. 
        /// </summary>
        /// <param name="filePath">Path to file (database part added automatically)</param>
        /// <param name="resultHandler">Download completion handler</param>
        [PublicAPI]
        public void Get(string filePath, UnityAction<ObjectResultContainer<Texture2D>> resultHandler)
        {
            GetTexture2D(filePath, resultHandler, false);
        }

        /// <summary>
        /// Gets texture from remote storage. Complete handler is method that will handle download process after
        /// its completed. If success Container will have path to file and texture2d. If failure Container will have path to file
        /// and failure reason. 
        /// </summary>
        /// <param name="filePath">Path to file (database part added automatically)</param>
        /// <param name="resultHandler">Download completion handler</param>
        [PublicAPI]
        public void Download(string filePath, UnityAction<ObjectResultContainer<Texture2D>> resultHandler)
        {
            GetTexture2D(filePath, resultHandler, true);
        }

        void GetTexture2D(string filePath, UnityAction<ObjectResultContainer<Texture2D>> resultHandler,
            bool forceDownload)
        {
            if (storage?.DownloadService == null)
                resultHandler.Dispatch(new ObjectResultContainer<Texture2D>(
                    new StorageResultContainer(filePath, StorageErrorMessages.STORAGE_NOT_INITIALIZED)));
            else
                storage.DownloadService.GetTexture(filePath, resultHandler, forceDownload);
        }
    }
}