using HUF.Storage.Runtime.API.Structs;
using UnityEngine.Events;

namespace HUF.Storage.Runtime.API
{
    public interface IObjectStorage<T> where T : class
    {
        void Get(string filePath, UnityAction<ObjectResultContainer<T>> resultHandler);
        void Download(string filePath, UnityAction<ObjectResultContainer<T>> resultHandler);
    }
}