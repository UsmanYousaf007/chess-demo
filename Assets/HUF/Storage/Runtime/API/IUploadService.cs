using System;
using HUF.Storage.Runtime.Implementation.Structs;

namespace HUF.Storage.Runtime.API.Services
{
    public interface IUploadService : IDisposable
    {
        void UploadFile(string pathToFile, object objectToUpload, Action<StorageResultContainer> completeHandler);
    }
}