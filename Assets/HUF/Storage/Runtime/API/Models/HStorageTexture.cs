using System;
using HUF.Storage.Runtime.API.Services;
using HUF.Storage.Runtime.Implementation.Structs;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.Storage.Runtime.API.Models
{
    public class HStorageTexture : IObjectStorage<Texture2D>
    {
        readonly Implementation.StorageService storageService;

        public HStorageTexture(Implementation.StorageService storageServiceService)
        {
            storageService = storageServiceService;
        }

        /// <summary>
        /// Gets the texture either from a remote or a local storage. The handler is raised after the process is completed.
        /// If it succeeds, the container has a path to the file and the texture.
        /// If it fails, the container has a path to the file and a failure reason.
        /// </summary>
        /// <param name="filePath">A path to the file.</param>
        /// <param name="resultHandler">A download completion handler.</param>
        [PublicAPI]
        public void Get(string filePath, Action<ObjectResultContainer<Texture2D>> resultHandler)
        {
            GetTexture2D(filePath, resultHandler, false);
        }

        /// <summary>
        /// Gets the texture either from a remote or a local storage. The handler is raised after the process is completed.
        /// If it succeeds, the container has a path to the file and the texture.
        /// If it fails, the container has a path to the file and a failure reason.
        /// </summary>
        /// <param name="filePath">A path to the file.</param>
        /// <param name="resultHandler">A download completion handler.</param>
        /// <param name="serviceType">A storage service to use.</param>
        [PublicAPI]
        public void Get(string filePath, Action<ObjectResultContainer<Texture2D>> resultHandler, StorageService serviceType)
        {
            GetTexture2D(filePath, resultHandler, false, serviceType);
        }

        /// <summary>
        /// Downloads the texture from a remote storage service. The handler is raised after the download process is completed.
        /// If a download is successful, the container has a path to the file and the texture.
        /// If the download fails, the container has the path to the file and a failure reason.
        /// </summary>
        /// <param name="filePath">A path to the file.</param>
        /// <param name="resultHandler">A download completion handler.</param>
        [PublicAPI]
        public void Download(string filePath, Action<ObjectResultContainer<Texture2D>> resultHandler)
        {
            GetTexture2D(filePath, resultHandler, true);
        }

        /// <summary>
        /// Downloads the texture from a remote storage service. The handler is raised after the download process is completed.
        /// If a download is successful, the container has a path to the file and the texture.
        /// If the download fails, the container has the path to the file and a failure reason.
        /// </summary>
        /// <param name="filePath">A path to the file.</param>
        /// <param name="resultHandler">A download completion handler.</param>
        /// <param name="serviceType">A storage service to use.</param>
        [PublicAPI]
        public void Download(string filePath, Action<ObjectResultContainer<Texture2D>> resultHandler, StorageService serviceType)
        {
            GetTexture2D(filePath, resultHandler, true);
        }

        void GetTexture2D(string filePath, Action<ObjectResultContainer<Texture2D>> resultHandler,
            bool forceDownload, StorageService? serviceType = null)
        {
            storageService.GetTexture(filePath, resultHandler, forceDownload, serviceType);
        }
    }
}