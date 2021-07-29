using System;
using HUF.Storage.Runtime.API.Services;
using HUF.Storage.Runtime.Implementation.Structs;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.Storage.Runtime.API.Models
{
    public class HStorageAssetBundle : IObjectStorage<AssetBundle>
    {
        readonly Implementation.StorageService storageService;

        public HStorageAssetBundle(Implementation.StorageService storageServiceService)
        {
            storageService = storageServiceService;
        }

        /// <summary>
        /// Gets the asset bundle either from a remote or a local storage. The handler is raised after the process is completed.
        /// If it succeeds, the container has a path to the file and the asset bundle.
        /// If it fails, the container has a path to the file and a failure reason.
        /// </summary>
        /// <param name="filePath">A path to the file.</param>
        /// <param name="resultHandler">A download completion handler.</param>
        [PublicAPI]
        public void Get(string filePath, Action<ObjectResultContainer<AssetBundle>> resultHandler)
        {
            GetAssetBundle(filePath, resultHandler, false);
        }

        /// <summary>
        /// Gets the asset bundle either from a remote or a local storage. The handler is raised after the process is completed.
        /// If it succeeds, the container has a path to the file and the asset bundle.
        /// If it fails, the container has a path to the file and a failure reason.
        /// </summary>
        /// <param name="filePath">A path to the file.</param>
        /// <param name="resultHandler">A download completion handler.</param>
        /// <param name="serviceType">A storage service to use.</param>
        [PublicAPI]
        public void Get(string filePath, Action<ObjectResultContainer<AssetBundle>> resultHandler, StorageService serviceType)
        {
            GetAssetBundle(filePath, resultHandler, false, serviceType);
        }

        /// <summary>
        /// Downloads the asset bundle from a remote storage service. The handler is raised after the download process is completed.
        /// If a download is successful, the container has a path to the file and the asset bundle.
        /// If the download fails, the container has a path to the file and a failure reason.
        /// </summary>
        /// <param name="filePath">A path to the file.</param>
        /// <param name="resultHandler">A download completion handler.</param>
        [PublicAPI]
        public void Download(string filePath, Action<ObjectResultContainer<AssetBundle>> resultHandler)
        {
            GetAssetBundle(filePath, resultHandler, true);
        }

        /// <summary>
        /// Downloads the asset bundle from a remote storage service. The handler is raised after the download process is completed.
        /// If a download is successful, the container has a path to the file and the asset bundle.
        /// If the download fails, the container has the path to the file and a failure reason.
        /// </summary>
        /// <param name="filePath">A path to the file.</param>
        /// <param name="resultHandler">A download completion handler.</param>
        /// <param name="serviceType">A storage service to use.</param>
        [PublicAPI]
        public void Download(string filePath, Action<ObjectResultContainer<AssetBundle>> resultHandler, StorageService serviceType)
        {
            GetAssetBundle(filePath, resultHandler, true, serviceType);
        }

        void GetAssetBundle(string filePath, Action<ObjectResultContainer<AssetBundle>> resultHandler,
            bool forceDownload, StorageService? serviceType = null)
        {
            storageService.GetAssetBundle(filePath, resultHandler, forceDownload, serviceType);
        }


    }
}