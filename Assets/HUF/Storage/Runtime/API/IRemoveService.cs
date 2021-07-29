using System;
using HUF.Storage.Runtime.Implementation.Structs;

namespace HUF.Storage.Runtime.API.Services
{
    public interface IRemoveService : IDisposable
    {
        void RemoveFile(string pathToFile, Action<StorageResultContainer> completeHandler);
    }
}