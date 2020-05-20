using System;
using HUF.Storage.Runtime.API.Structs;
using UnityEngine.Events;

namespace HUF.Storage.Runtime.API
{
    public interface IUploadService : IDisposable
    {
        void UploadFile(string pathToFile, object objectToUpload, UnityAction<StorageResultContainer> completeHandler);
    }
}