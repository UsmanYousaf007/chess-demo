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
        /// Gets the audio clip either from the remote or local storage. The handler will be called after the process 
        /// is completed. If it was successful, the container will contain the path to the file and the audio clip.
        /// If it has failed, the container will contain the path to the file and the failure reason. 
        /// </summary>
        /// <param name="filePath">A path to the file (database part added automatically).</param>
        /// <param name="resultHandler">A download completion handler.</param>
        [PublicAPI]
        public void Get(string filePath, UnityAction<ObjectResultContainer<AudioClip>> resultHandler)
        {
            GetAudioClip(filePath, resultHandler, false);
        }

        /// <summary>
        /// Downloads the audio clip from the remote storage. The handler will be called after the download process
        /// is completed. If it was successful, the container will contain the path to the file and the audio clip.
        /// If it has failed, the container will contain the path to the file and the failure reason. 
        /// </summary>
        /// <param name="filePath">A path to the file (database part added automatically).</param>
        /// <param name="resultHandler">A download completion handler.</param>
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