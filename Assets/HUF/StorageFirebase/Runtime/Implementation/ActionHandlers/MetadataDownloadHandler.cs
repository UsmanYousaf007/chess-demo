using System;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Storage;
using HUF.InitFirebase.Runtime;
using HUF.Storage.Runtime.API;
using HUF.Storage.Runtime.API.Structs;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.PlayerPrefs;
using UnityEngine.Events;

namespace HUF.StorageFirebase.Runtime.Implementation.ActionHandlers
{
    public class MetadataDownloadHandler
    {
        readonly UnityAction<MetadataResultContainer> completeHandler;
        string PathToFile { get; }
        StorageMetadata metadata;

        public MetadataDownloadHandler(string pathToFile, UnityAction<MetadataResultContainer> completeHandler = null)
        {
            PathToFile = pathToFile;
            this.completeHandler = completeHandler;
        }

        public void GetMetadata(StorageReference metadataReference)
        {
            metadataReference.GetMetadataAsync().ContinueWithOnMainThread(HandleMetadataDownloadCompleted);
        }

        void HandleMetadataDownloadCompleted(Task<StorageMetadata> metadataTask)
        {
            if (metadataTask.IsFaulted || metadataTask.IsCanceled)
            {
                var fullError = metadataTask.Exception.GetFullErrorMessage();
                completeHandler?.Dispatch(
                    new MetadataResultContainer(new StorageResultContainer(PathToFile, fullError)));
                return;
            }

            metadata = metadataTask.Result;
            MetadataDownloadCompleted();
        }

        void MetadataDownloadCompleted()
        {
            var lastKnownDate = GetLastKnownDate(StorageUtils.GetMetadataPrefsPath(PathToFile));
            var isUpdateAvailable = metadata.UpdatedTimeMillis.CompareTo(lastKnownDate) > 0;
            if (isUpdateAvailable)
            {
                HPlayerPrefs.SetString(StorageUtils.GetMetadataPrefsPath(PathToFile),
                    metadata.UpdatedTimeMillis.Ticks.ToString());
            }

            var storageResultContainer = new StorageResultContainer(PathToFile);
            var metadataResultContainer = new MetadataResultContainer(storageResultContainer, isUpdateAvailable); 
            completeHandler?.Dispatch(metadataResultContainer);
        }

        static DateTime GetLastKnownDate(string pathToPrefs)
        {
            return HPlayerPrefs.HasKey(pathToPrefs)
                ? new DateTime(HPlayerPrefs.GetLong(pathToPrefs))
                : DateTime.MinValue;
        }
    }
}