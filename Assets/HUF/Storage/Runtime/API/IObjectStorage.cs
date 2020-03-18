using HUF.Storage.API.Structs;
using UnityEngine.Events;

namespace HUF.Storage.API
{
    public interface IObjectStorage<T> where T : class
    {
        void Get(string filePath, UnityAction<ObjectResultContainer<T>> resultHandler);
        void Download(string filePath, UnityAction<ObjectResultContainer<T>> resultHandler);
    }
}