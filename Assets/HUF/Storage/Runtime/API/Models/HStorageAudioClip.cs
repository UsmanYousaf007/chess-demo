using HUF.Storage.Runtime.API.Structs;
using HUF.Utils.Runtime.Extensions;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.Storage.Runtime.API.Models
{
    public class HStorageAudioClip : IObjectStorage<AudioClip>
    {
        readonly StorageModel storage;

        public HStorageAudioClip(StorageModel storageModel)
        {
            storage = storageModel;
        }

        /// <summary>
        /// Gets audio clip either from remote or local storage. Complete handler is method that will handle download process after
        /// its completed. If success Container will have path to file and audio clip. If failure Container will have path to file
        /// and failure reason. 
        /// </summary>
        /// <param name="filePath">Path to file (database part added automatically)</param>
        /// <param name="resultHandler">Download completion handler</param>
        [PublicAPI]
        public void Get(string filePath, UnityAction<ObjectResultContainer<AudioClip>> resultHandler)
        {
            GetAudioClip(filePath, resultHandler, false);
        }

        /// <summary>
        /// Downloads audio clip from remote storage. Complete handler is method that will handle download process after
        /// its completed. If success Container will have path to file and audio clip. If failure Container will have path to file
        /// and failure reason. 
        /// </summary>
        /// <param name="filePath">Path to file (database part added automatically)</param>
        /// <param name="resultHandler">Download completion handler</param>
        [PublicAPI]
        public void Download(string filePath, UnityAction<ObjectResultContainer<AudioClip>> resultHandler)
        {
            GetAudioClip(filePath, resultHandler, true);
        }

        void GetAudioClip(string filePath, UnityAction<ObjectResultContainer<AudioClip>> resultHandler,
            bool forceDownload)
        {
            if (storage?.DownloadService == null)
                resultHandler.Dispatch(new ObjectResultContainer<AudioClip>(
                    new StorageResultContainer(filePath, StorageErrorMessages.STORAGE_NOT_INITIALIZED)));
            else
                storage?.DownloadService?.GetAudioClip(filePath, resultHandler, forceDownload);
        }
    }
}