using HUF.Storage.Runtime.API.Structs;
using HUF.Utils.Runtime.Extensions;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.Storage.Runtime.API.Models
{
    public class HStorageAssetBundle : IObjectStorage<AssetBundle>
    {
        readonly StorageModel storage;

        public HStorageAssetBundle(StorageModel storageModel)
        {
            storage = storageModel;
        }

        /// <summary>
        /// Gets the asset bundle either from the remote or local storage. The handler will be called after the process 
        /// is completed. If it was successful, the container will contain the path to the file and the asset bundle.
        /// If it has failed, the container will contain the path to the file and the failure reason. 
        /// </summary>
        /// <param name="filePath">A path to the file (database part added automatically).</param>
        /// <param name="resultHandler">A download completion handler.</param>
        [PublicAPI]
        public void Get(string filePath, UnityAction<ObjectResultContainer<AssetBundle>> resultHandler)
        {
            GetAssetBundle(filePath, resultHandler, false);
        }

        /// <summary>
        /// Downloads the asset bundle from the remote storage. The handler will be called after the download process
        /// is completed. If it was successful, the container will contain the path to the file and the asset bundle.
        /// If it has failed, the container will contain the path to the file and the failure reason. 
        /// </summary>
        /// <param name="filePath">A path to the file (database part added automatically).</param>
        /// <param name="resultHandler">A download completion handler.</param>
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