using HUF.Storage.API.Structs;
using HUF.Utils.Extensions;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.Storage.API.Models
{
    public class HStorageAssetBundle : IObjectStorage<AssetBundle>
    {
        readonly StorageModel storage;

        public HStorageAssetBundle(StorageModel storageModel)
        {
            storage = storageModel;
        }

        /// <summary>
        /// Gets asset bundle either from remote or local storage. Complete handler is method that will handle download process after
        /// its completed. If success Container will have path to file and asset bundle. If failure Container will have path to file
        /// and failure reason. 
        /// </summary>
        /// <param name="filePath">Path to file (database part added automatically)</param>
        /// <param name="resultHandler">Download completion handler</param>
        [PublicAPI]
        public void Get(string filePath, UnityAction<ObjectResultContainer<AssetBundle>> resultHandler)
        {
            GetAssetBundle(filePath, resultHandler, false);
        }

        /// <summary>
        /// Gets asset bundle from remote storage. Complete handler is method that will handle download process after
        /// its completed. If success Container will have path to file and asset bundle. If failure Container will have path to file
        /// and failure reason. 
        /// </summary>
        /// <param name="filePath">Path to file (database part added automatically)</param>
        /// <param name="resultHandler">Download completion handler</param>
        [PublicAPI]
        public void Download(string filePath, UnityAction<ObjectResultContainer<AssetBundle>> resultHandler)
        {
            GetAssetBundle(filePath, resultHandler, true);
        }

        void GetAssetBundle(string filePath, UnityAction<ObjectResultContainer<AssetBundle>> resultHandler,
            bool forceDownload)
        {
            if (storage?.DownloadService == null)
                resultHandler.Dispatch(new ObjectResultContainer<AssetBundle>(
                    new StorageResultContainer(filePath, StorageErrorMessages.STORAGE_NOT_INITIALIZED)));
            else
                storage?.DownloadService?.GetAssetBundle(filePath, resultHandler, forceDownload);
        }
    }
}