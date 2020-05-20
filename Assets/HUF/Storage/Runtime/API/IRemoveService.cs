using System;
using HUF.Storage.Runtime.API.Structs;
using UnityEngine.Events;

namespace HUF.Storage.Runtime.API
{
    public interface IRemoveService : IDisposable
    {
        void RemoveFile(string pathToFile, UnityAction<StorageResultContainer> completeHandler);
    }
}