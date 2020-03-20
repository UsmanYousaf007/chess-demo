using System;
using HUF.Storage.API.Structs;
using UnityEngine.Events;

namespace HUF.Storage.API
{
    public interface IRemoveService : IDisposable
    {
        void RemoveFile(string pathToFile, UnityAction<StorageResultContainer> completeHandler);
    }
}