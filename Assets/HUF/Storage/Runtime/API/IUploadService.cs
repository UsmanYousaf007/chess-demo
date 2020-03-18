using System;
using HUF.Storage.API.Structs;
using UnityEngine.Events;

namespace HUF.Storage.API
{
    public interface IUploadService : IDisposable
    {
        void UploadFile(string pathToFile, object objectToUpload, UnityAction<StorageResultContainer> completeHandler);
    }
}