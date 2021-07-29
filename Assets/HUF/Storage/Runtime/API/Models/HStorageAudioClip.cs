using System;
using HUF.Storage.Runtime.API.Services;
using HUF.Storage.Runtime.Implementation.Structs;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.Storage.Runtime.API.Models
{
    public class HStorageAudioClip : IObjectStorage<AudioClip>
    {
        readonly Implementation.StorageService storageService;

        public HStorageAudioClip(Implementation.StorageService storageServiceService)
        {
            storageService = storageServiceService;
        }

        /// <summary>
        /// Gets the audio clip either from a remote or a local storage. The handler is raised after the process is completed.
        /// If it succeeds, the container has a path to the file and the audio clip.
        /// If it fails, the container has a path to the file and a failure reason.
        /// </summary>
        /// <param name="filePath">A path to the file.</param>
        /// <param name="resultHandler">A download completion handler.</param>
        [PublicAPI]
        public void Get(string filePath, Action<ObjectResultContainer<AudioClip>> resultHandler)
        {
            GetAudioClip(filePath, resultHandler, false);
        }

        /// <summary>
        /// Gets the audio clip either from a remote or a local storage. The handler is raised after the process is completed.
        /// If it succeeds, the container has a path to the file and the audio clip.
        /// If it fails, the container has a path to the file and a failure reason.
        /// </summary>
        /// <param name="filePath">A path to the file.</param>
        /// <param name="resultHandler">A download completion handler.</param>
        /// <param name="serviceType">A storage service to use.</param>
        [PublicAPI]
        public void Get(string filePath, Action<ObjectResultContainer<AudioClip>> resultHandler, StorageService serviceType)
        {
            GetAudioClip(filePath, resultHandler, false);
        }

        /// <summary>
        /// Downloads the audio clip from a remote storage service. The handler is raised after the download process is completed.
        /// If a download is successful, the container has a path to the file and the audio clip.
        /// If the download fails, the container has the path to the file and a failure reason.
        /// </summary>
        /// <param name="filePath">A path to the file.</param>
        /// <param name="resultHandler">A download completion handler.</param>
        [PublicAPI]
        public void Download(string filePath, Action<ObjectResultContainer<AudioClip>> resultHandler)
        {
            GetAudioClip(filePath, resultHandler, true);
        }

        /// <summary>
        /// Downloads the audio clip from a remote storage service. The handler is raised after the download process is completed.
        /// If a download is successful, the container has a path to the file and the audio clip.
        /// If the download fails, the container has the path to the file and a failure reason.
        /// </summary>
        /// <param name="filePath">A path to the file..</param>
        /// <param name="resultHandler">A download completion handler.</param>
        /// <param name="serviceType">A storage service to use.</param>
        [PublicAPI]
        public void Download(string filePath, Action<ObjectResultContainer<AudioClip>> resultHandler, StorageService serviceType)
        {
            GetAudioClip(filePath, resultHandler, true);
        }

        void GetAudioClip(string filePath, Action<ObjectResultContainer<AudioClip>> resultHandler,
            bool forceDownload, StorageService? serviceType = null)
        {
            storageService.GetAudioClip(filePath, resultHandler, forceDownload, serviceType);
        }
    }
}