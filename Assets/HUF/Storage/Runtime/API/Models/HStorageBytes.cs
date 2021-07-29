using System;
using HUF.Storage.Runtime.API.Services;
using HUF.Storage.Runtime.Implementation.Structs;
using JetBrains.Annotations;

namespace HUF.Storage.Runtime.API.Models
{
    public class HStorageBytes : IObjectStorage<byte[]>
    {
        readonly Implementation.StorageService storageService;

        public HStorageBytes(Implementation.StorageService storageServiceService)
        {
            storageService = storageServiceService;
        }

        /// <summary>
        /// Gets the bytes either from a remote or a local storage. The handler is raised after the process is completed.
        /// If it succeeds, the container has a path to the file and the bytes.
        /// If it fails, the container has a path to the file and a failure reason.
        /// </summary>
        /// <param name="filePath">A path to the file.</param>
        /// <param name="resultHandler">A download completion handler.</param>
        [PublicAPI]
        public void Get(string filePath, Action<ObjectResultContainer<byte[]>> resultHandler)
        {
            GetBytes(filePath, resultHandler, false);
        }

        /// <summary>
        /// Gets the bytes either from a remote or a local storage. The handler is raised after the process is completed.
        /// If it succeeds, the container has a path to the file and the bytes.
        /// If it fails, the container has a path to the file and a failure reason.
        /// </summary>
        /// <param name="filePath">A path to the file.</param>
        /// <param name="resultHandler">A download completion handler.</param>
        /// <param name="serviceType">A storage service to use.</param>
        [PublicAPI]
        public void Get(string filePath, Action<ObjectResultContainer<byte[]>> resultHandler, StorageService serviceType)
        {
            GetBytes(filePath, resultHandler, false);
        }

        /// <summary>
        /// Downloads the bytes from a remote storage service. The handler is raised after the download process is completed.
        /// If a download is successful, the container has a path to the file and the bytes.
        /// If the download fails, the container has the path to the file and a failure reason.
        /// </summary>
        /// <param name="filePath">A path to the file.</param>
        /// <param name="resultHandler">A download completion handler.</param>
        [PublicAPI]
        public void Download(string filePath, Action<ObjectResultContainer<byte[]>> resultHandler)
        {
            GetBytes(filePath, resultHandler, true);
        }

        /// <summary>
        /// Downloads the bytes from a remote storage service. The handler is raised after the download process is completed.
        /// If a download is successful, the container has a path to the file and the bytes.
        /// If the download fails, the container has the path to the file and a failure reason.
        /// </summary>
        /// <param name="filePath">A path to the file.</param>
        /// <param name="resultHandler">A download completion handler.</param>
        /// <param name="serviceType">A storage service to use.</param>
        [PublicAPI]
        public void Download(string filePath, Action<ObjectResultContainer<byte[]>> resultHandler, StorageService serviceType)
        {
            GetBytes(filePath, resultHandler, true);
        }

        void GetBytes(string filePath, Action<ObjectResultContainer<byte[]>> resultHandler, bool forceDownload, StorageService? serviceType = null)
        {
            storageService.GetFileBytes(filePath, resultHandler, forceDownload, serviceType);
        }
    }
}